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

    // 主摄像机
    public MainCamera MainCamera = null;

    // 地图大小
    public Vector2 WorldSize = new Vector2(10, 10);

    // 当前世界移动物体
    Dictionary<string, MovableObject> movingObjs = new Dictionary<string, MovableObject>();

    // 客户端的游戏世界时间流失
    int timeElapsed = 0;

    private void Update()
    {
        OnTimeElapsed(Time.deltaTime);
    }

    // 添加一个物体
    public void AddObject(string id, string type, Vec2 pos, Fix64 velocity, Fix64 dir, Vec2 dirTo, Fix64 maxTv, Fix64 turnV, Fix64 maxHp, Fix64 hp, Fix64 power)
    {
        if (movingObjs.ContainsKey(id))
            throw new Exception("object id conflicted: " + id);

        // 添加模型
        GameObject go = null;

        if (type == "Airplane_0")
            go = Instantiate(AirplaneModels[0].gameObject) as GameObject;
        else if (type == "Airplane_1")
            go = Instantiate(AirplaneModels[1].gameObject) as GameObject;
        else if (type == "Airplane_2")
            go = Instantiate(AirplaneModels[2].gameObject) as GameObject;
        else if (type == "Bullet")
            go = Instantiate(BulletModels[0].gameObject) as GameObject;
        else
            return;

        go.SetActive(true);
        go.transform.SetParent(SceneRoot, false);

        // 设置属性
        var a = go.GetComponent<MovableObject>();
        a.ID = id;
        a.Pos = pos;
        a.Dir = dir;
        a.Velocity = velocity;
        a.Turn2Dir = dirTo;
        a.MaxTv = maxTv;
        a.TurnV = turnV;
        a.MaxHp = maxHp;
        a.Hp = hp;
        a.Power = power;
        a.UpdateImmediately();
        movingObjs[id] = a;

        if (GameCore.Instance.Me.ID == id)
        {
            MainCamera.Target = a.transform;
            GameCore.Instance.MeObj = a;
        }
    }

    // 移除一架飞机
    public void DelAirplane(string id)
    {
        if (!movingObjs.ContainsKey(id))
            throw new Exception("airplane id not exists: " + id);

        var a = movingObjs[id];
        movingObjs.Remove(id);

        var go = a.gameObject;
        go.transform.SetParent(null);
        Destroy(go);
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
        foreach (var mo in movingObjs.Values)
            mo.UpdateSmoothly((float)te);

        timeElapsed += (int)(te * 1000);

        // // 固定间隔时间处理一次指令，处理至少一条服务器指令
        while (commanders.Count > curCmdIndex + 1 && timeElapsed >= FrameMS)
        {
            // 推动物体平滑表现
            foreach (var mo in movingObjs.Values)
                mo.UpdateSmoothly(FrameSec);

            // 处理指令
            ProcessCommands();

            // 推动物体逻辑
            foreach (var mo in movingObjs.Values)
                mo.MoveForward(FrameSec);

            // 打印调试信息
            //Debug.Log("== t == " + (curCmdIndex + timeNumBase));
            //foreach (var obj in movingObjs.Values)
            //    Debug.Log("  " + obj.ID + ": (" + obj.Pos.x + ", " + obj.Pos.y + ") : " + obj.Dir);

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
    public MovableObject GetByID(string id)
    {
        return movingObjs[id] as MovableObject;
    }

    // 增加飞机
    public void Add(int t, string id, string type, Vec2 pos, Fix64 velocity, Fix64 dir, Vec2 dirTo, Fix64 maxTv, Fix64 tv, Fix64 maxHp, Fix64 hp, Fix64 power)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { AddObject(id, type, pos, velocity, dir, dirTo, maxTv, tv, maxHp, hp, power); });
    }

    // 移除飞机
    public void Del(int t, string id)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { DelAirplane(id); });
    }

    // 给定时间编号开始，到一定时间编号长度内，空指令
    public void Dumb(int t, int tLen = 1)
    {
        FC.For(t, t + tLen, (i) => { RetrieveCmds(i).Add(null); });
    }

    // 同步房间状态
    public void SyncRoomStatus(int t, string[] ids, string[] types, Vec2[] poses, Fix64[] vs, Fix64[] dirs, Vec2[] dirTos, Fix64[] maxTurnVs, Fix64[] turnVs, Fix64[] maxHps, Fix64[] hps, Fix64[] powers)
    {
        timeNumBase = t;
        commanders.Clear();

        // 插入一条哑指令作为起始
        Dumb(0);

        FC.For(ids.Length, (i) =>
        {
            var id = ids[i];
            var type = types[i];
            var pos = poses[i];
            var v = vs[i];
            var dir = dirs[i];
            var dirTo = dirTos[i];
            var maxTv = maxTurnVs[i];
            var tv = turnVs[i];
            var maxHp = maxHps[i];
            var hp = hps[i];
            var power = powers[i];
            Add(0, id, type, pos, v, dir, dirTo, maxTv, tv, maxHp, hp, power);
        });
    }

    // 设置飞机方向
    public void SetDir(int t, string id, Fix64 dir)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { var mo = movingObjs[id]; mo.Dir = dir; });
    }

    // 设置飞机转向指定方向
    public void Turn2(int t, string id, Vec2 toDir, Fix64 tv)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() =>
        {
            var mo = movingObjs[id];
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
            var obj1 = movingObjs[id1];
            var obj2 = movingObjs[id2];
            obj1.Hp -= obj2.Power;
            obj2.Power -= obj1.Power;
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
            var ids = new string[cnt];
            var types = new string[cnt];
            var poses = new Vec2[cnt];
            var vs = new Fix64[cnt];
            var dirs = new Fix64[cnt];
            var dirTos = new Vec2[cnt];
            var maxTVs = new Fix64[cnt];
            var turnVs = new Fix64[cnt];
            var maxHps = new Fix64[cnt];
            var hps = new Fix64[cnt];
            var powers = new Fix64[cnt];
            FC.For(cnt, (i) =>
            {
                ids[i] = data.ReadString();
                types[i] = data.ReadString();
                poses[i] = new Vec2(data.ReadFix64(), data.ReadFix64());
                vs[i] = data.ReadFix64();
                dirs[i] = data.ReadFix64();
                dirTos[i] = new Vec2(data.ReadFix64(), data.ReadFix64());
                maxTVs[i] = data.ReadFix64();
                turnVs[i] = data.ReadFix64();
                maxHps[i] = data.ReadFix64();
                hps[i] = data.ReadFix64();
                powers[i] = data.ReadFix64();
            });

            SyncRoomStatus(t, ids, types, poses, vs, dirs, dirTos, maxTVs, turnVs, maxHps, hps, powers);
        });

        OnOp("GameTimeFowardStep", (t, data) => { Dumb(t); });
        OnOp("AddIn", (t, data) =>
        {
            var id = data.ReadString();
            var type = data.ReadString();
            var pos = new Vec2(data.ReadFix64(), data.ReadFix64());
            var v = data.ReadFix64();
            var dir = data.ReadFix64();
            var dirTo = new Vec2(data.ReadFix64(), data.ReadFix64());
            var maxTv = data.ReadFix64();
            var tv = data.ReadFix64();
            var maxHp = data.ReadFix64();
            var hp = data.ReadFix64();
            var power = data.ReadFix64();

            Add(t, id, type, pos, v, dir, dirTo, maxTv, tv, maxHp, hp, power);
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
    }
}
