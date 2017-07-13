using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;
using Swift;
using Swift.Math;
using System.Linq;

// 一个游戏世界代表一局游戏
public class GameWorld : MonoBehaviour
{
    public static float FrameSec = 0.05f;
    public static int FrameMS = (int)(FrameSec * 1000);

    // 场景根节点
    public Transform SceneRoot = null;

    // 飞机模型
    public GameObject[] AirplaneModels = null;

    // 子弹模型
    public GameObject[] BulletModels = null;

    // 道具
    public GameObject[] Items = null;

    // 主摄像机
    public MainCamera MainCamera = null;

    // 地图大小
    public Vector2 WorldSize = new Vector2(10, 10);

    // 当前世界移动物体
    Dictionary<string, MovableObjectController> movingObjControllers = new Dictionary<string, MovableObjectController>();

    // 客户端的游戏世界时间流失
    int timeElapsed = 0;

    private void Update()
    {
        OnTimeElapsed(Time.deltaTime);
    }

    // 添加一个物体
    public void AddObject(MovableObjectInfo obj)
    {
        var id = obj.ID;
        var type = obj.Type;

        if (movingObjControllers.ContainsKey(id))
            throw new Exception("object id conflicted: " + id);

        // 添加模型
        GameObject go = null;

        if (type == "Airplane/0")
            go = Instantiate(AirplaneModels[0].gameObject) as GameObject;
        else if (type == "Airplane/1")
            go = Instantiate(AirplaneModels[1].gameObject) as GameObject;
        else if (type == "Airplane/2")
            go = Instantiate(AirplaneModels[2].gameObject) as GameObject;
        else if (type == "SmallBullet")
            go = Instantiate(BulletModels[0].gameObject) as GameObject;
        else if (type == "Medicine")
            go = Instantiate(Items[0].gameObject) as GameObject;
        else if (type == "Coin")
            go = Instantiate(Items[1].gameObject) as GameObject;
        else if (type == "Lightning")
            go = Instantiate(Items[2].gameObject) as GameObject;
        else
            throw new Exception("unknown object type: " + type);

        go.SetActive(true);
        go.transform.SetParent(SceneRoot, false);

        // 设置属性
        var oc = go.GetComponent<MovableObjectController>();
        oc.MO = obj;
        oc.UpdateImmediately();
        movingObjControllers[id] = oc;

        // 自己进入房间
        var gc = GameCore.Instance;
        if (gc.Me.ID == id)
        {
            MainCamera.Target = oc.transform;
            gc.MeOC = oc;
            gc.MyKills = 0;
            gc.OnIn.SC();
        }
    }

    // 移除物体
    public void DelObject(string id)
    {
        if (!movingObjControllers.ContainsKey(id))
            throw new Exception("airplane id not exists: " + id);

        var a = movingObjControllers[id];
        movingObjControllers.Remove(id);

        var go = a.gameObject;
        go.transform.SetParent(null);
        Destroy(go);

        // 自己被移出房间
        if (GameCore.Instance.Me.ID == id)
        {
            MainCamera.Target = null;
            GameCore.Instance.MeOC = null;
            GameCore.Instance.OnOut.SC();
        }
    }

    // 等待执行的指令列表
    List<List<Action>> commanders = new List<List<Action>>();

    // 当前指令位置
    int timeNumBase = 0; // 上一次同步房间状态时的时间编号，客户端只能收到此后的消息
    int curCmdIndex = 0;

    // 处理一批指令，固定间隔时间
    void ProcessCommands()
    {
        if (curCmdIndex >= commanders.Count)
            return;

        var acts = commanders[curCmdIndex];
        foreach (var act in acts)
            if (act != null)
                act();
    }

    // 推动表现
    int timeTime = 1; // 加速播放的倍数
    public void OnTimeElapsed(Fix64 te)
    {
        te *= timeTime;

        // 推动物体平滑表现
        foreach (var mo in movingObjControllers.Values)
            mo.UpdateSmoothly((float)te);

        timeElapsed += (int)(te * 1000);

        // 固定间隔时间处理一次指令，处理至少一条服务器指令
        while (commanders.Count > curCmdIndex + 1 && timeElapsed >= FrameMS)
        {
            // 推动物体平滑表现
            foreach (var mo in movingObjControllers.Values)
                mo.UpdateSmoothly(FrameSec);

            // 处理指令
            ProcessCommands();

            // 打印调试信息
            //Debug.Log("== t == " + (curCmdIndex + timeNumBase));

            // 推动物体逻辑
            foreach (var mc in movingObjControllers.Values)
            {
                mc.ProcessLogic(FrameSec);

                //if (mc.MO.CollisionType == "Airplane")
                //    Debug.Log("v = " + mc.Velocity);

                mc.ProcessMove(FrameSec);

                // if (mc.MO.CollisionType == "Airplane")
                //  Debug.Log(" " + mc.ID + ": (" + mc.Pos.x + ", " + mc.Pos.y + ") : " + mc.Dir);
            }

            timeElapsed -= FrameMS;
            curCmdIndex++;
            timeTime = commanders.Count - curCmdIndex; // 延迟了就加速追
        }
    }

    #region 所有可接收的指令接口，需要指明指令编号，共用编号的一组指令按生成的先后顺序执行

    // 根据指令编号获取对应指令列表
    List<Action> RetrieveCmds(int t)
    {
        if (t == commanders.Count - 1)
            return commanders[commanders.Count - 1];
        else if (t == commanders.Count)
        {
            var cmdLst = new List<Action>();
            commanders.Add(cmdLst);
            return cmdLst;
        }

        throw new Exception("invalid command time number for command list = " + commanders.Count + " and retrieve: " + t);
    }

    // 获取制定 ID 的物体
    public MovableObjectController GetByID(string id)
    {
        return movingObjControllers[id] as MovableObjectController;
    }

    // 增加飞机
    public void Add(int t, MovableObjectInfo obj)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { AddObject(obj); });
    }

    // 移除飞机
    public void Del(int t, string id)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { DelObject(id); });
    }

    // 给定时间编号开始，到一定时间编号长度内，空指令
    public void Dumb(int t, int tLen = 1)
    {
        FC.For(t, t + tLen, (i) => { RetrieveCmds(i).Add(null); });
    }

    // 同步房间状态
    public void SyncRoomStatus(int t, MovableObjectInfo[] objs)
    {
        timeNumBase = t;
        commanders.Clear();

        // 插入一条哑指令作为起始
        Dumb(0);

        FC.For(objs.Length, (i) =>
        {
            Add(0, objs[i]);
        });
    }

    // 设置飞机方向
    public void SetDir(int t, string id, Fix64 dir)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { var mc = movingObjControllers[id]; mc.Dir = dir; });
    }

    // 设置飞机转向指定方向
    public void Turn2(int t, string id, Vec2 toDir, Fix64 tv)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() =>
        {
            var mo = movingObjControllers[id];
            mo.Turn2Dir = toDir;
            mo.TurnV = tv;
        });
    }

    // 执行碰撞操作
    public void Collision(int t, string id1, string id2)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() =>
        {
            var obj1 = movingObjControllers[id1];
            var obj2 = movingObjControllers[id2];
            if (!Guerre.Collider.DoCollision(obj1.MO, obj2.MO))
                throw new Exception("detect collision disparity !! " + obj1.MO.CollisionType + " <=> " + obj2.MO.CollisionType);
        });
    }

    #endregion

    void OnOp(string op, Action<int, IReadableBuffer> cb)
    {
        var mh = GameCore.Instance.Get<MessageHandler>();
        mh.OnOp("GameRoom/" + op, (conn, data) =>
        {
            var t = data.ReadInt();
            t -= timeNumBase;
            cb(t, data);
        });
    }

    public void OnGameCoreInitialized()
    {
        var mh = GameCore.Instance.Get<MessageHandler>();
        mh.OnOp("GameRoom/SyncRoom", (conn, data) =>
        {
            var t = data.ReadInt();
            var cnt = data.ReadInt();
            var objs = new MovableObjectInfo[cnt];
            FC.For(cnt, (i) =>
            {
                objs[i] = new MovableObjectInfo();
                objs[i].Deserialize(data);
            });

            // 正在同步房间信息
            GameCore.Instance.RoomSynchonizing.SC();
            SyncRoomStatus(t, objs);

            // 击杀信息
            int killCnt = data.ReadInt();
            FC.For(killCnt, (i) =>
            {
                var k = data.ReadString();
                var n = data.ReadInt();
                GameCore.Instance.OnKill(k, n);
            });
        });

        OnOp("GameTimeFowardStep", (t, data) => { Dumb(t); });
        OnOp("AddIn", (t, data) =>
        {
            var obj = new MovableObjectInfo();
            obj.Deserialize(data);

            Add(t, obj);
        });
        OnOp("RemoveOut", (t, data) => { var id = data.ReadString(); Del(t, id);});
        OnOp("Turn2", (t, data) =>
        {
            var id = data.ReadString();
            var dirTo = new Vec2(data.ReadFix64(), data.ReadFix64());
            var tv = data.ReadFix64();
            Turn2(t, id, dirTo, tv);
        });
        OnOp("Collision", (t, data) =>
        {
            var k1 = data.ReadString();
            var k2 = data.ReadString();
            Collision(t, k1, k2);
        });
        OnOp("Killing", (t, data) =>
        {
            var killer = data.ReadString();
            GameCore.Instance.OnKill(killer, 1);
        });
        OnOp("SpeedUp", (t, data) =>
        {
            var id = data.ReadString();
            var cmds = RetrieveCmds(t);
            cmds.Add(() =>
            {
                var obj = movingObjControllers[id];
                obj.SpeedUp();
                obj.Mp = 0;
            });
        });
        OnOp("Jump", (t, data) =>
        {
            var id = data.ReadString();
            var cmds = RetrieveCmds(t);
            cmds.Add(() =>
            {
                var obj = movingObjControllers[id];
                obj.MoveForwardOnDir(MathEx.Sqrt(obj.Mp));
                obj.Mp = 0;
                obj.UpdateImmediately();
            });
        });
        OnOp("ShieldOn", (t, data) =>
        {
            var id = data.ReadString();
            var cmds = RetrieveCmds(t);
            cmds.Add(() =>
            {
                var obj = movingObjControllers[id];
                obj.Sheild = obj.Mp;
                obj.Mp = 0;
                obj.UpdateImmediately();
            });
        });
    }
}
