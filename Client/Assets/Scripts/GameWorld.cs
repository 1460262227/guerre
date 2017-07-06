using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;
using Swift;
using Swift.Math;

// 一个游戏世界代表一局游戏
public class GameWorld : MonoBehaviour {

    // 场景根节点
    public Transform SceneRoot = null;

    // 飞机模型
    public Airplane[] AirplaneModels = null;

    // 当前世界移动物体
    Dictionary<string, MovableObject> movingObjs = new Dictionary<string, MovableObject>();

    // 客户端的游戏世界时间流失
    int timeElapsed = 0;

    private void Update()
    {
        OnTimeElapsed(Time.deltaTime);
    }

    // 添加一架飞机
    public void AddAirplane(string id, Vec2 pos, float velocity, float dir, Vec2 dirTo, float turnV)
    {
        if (movingObjs.ContainsKey(id))
            throw new Exception("airplane id conflicted: " + id);

        // 添加模型
        var go = Instantiate(AirplaneModels[0].gameObject) as GameObject;
        go.SetActive(true);
        go.transform.SetParent(SceneRoot, false);

        // 设置飞机属性
        var a = go.GetComponent<Airplane>();
        a.ID = id;
        a.Pos = pos;
        a.Dir = dir;
        a.Velocity = velocity;
        a.Turn2Dir = dirTo;
        a.TurnV = turnV;
        movingObjs[id] = a;
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
    int timeTime = 1; // 加速播放的倍数

    // 处理一批指令，间隔固定为 100 毫秒
    bool ProcessCommands()
    {
        if (curCmdIndex >= commanders.Count)
            return false;

        var acts = commanders[curCmdIndex];
        foreach (var act in acts)
            if (act != null)
                act();

        return true;
    }

    // 推动表现
    public void OnTimeElapsed(float te)
    {
        // 每 100ms 处理一次指令
        if (timeElapsed >= 100)
        {
            if (ProcessCommands())
            {
                timeElapsed -= 100;
                curCmdIndex++;

                // 延迟了就加速追
                timeTime = commanders.Count - curCmdIndex + 1;
            }
            else
                return;
        }

        te = te * timeTime;
        timeElapsed += (int)(te * 1000);

        // 推动飞机运动
        foreach (var mo in movingObjs.Values)
            mo.MoveForward(te);
    }

    #region 所有可接收的指令接口，需要指明指令编号，共用编号的一组指令认为是同时发生

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

    // 获取制定 ID 的飞机
    public Airplane GetByID(string id)
    {
        return movingObjs[id] as Airplane;
    }

    // 增加飞机
    public void Add(int t, string id, string type, Vec2 pos, float velocity, float dir, Vec2 dirTo, float tv)
    {
        var cmds = RetrieveCmds(t);
        if (type == "Airplane")
         cmds.Add(() => { AddAirplane(id, pos, velocity, dir, dirTo, tv); });
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
    public void SyncRoomStatus(int t, string[] ids, string[] types, Vec2[] poses, float[] vs, float[] dirs, Vec2[] dirTos, float[] turnVs)
    {
        timeNumBase = t;
        commanders.Clear();

        FC.For(ids.Length, (i) =>
        {
            var id = ids[i];
            var type = types[i];
            var pos = poses[i];
            var dir = dirs[i];
            var v = vs[i];
            var dirTo = dirTos[i];
            var tv = turnVs[i];
            Add(0, id, type, pos, dir, v, dirTo, tv);
        });
    }

    // 设置飞机方向
    public void SetDir(int t, string id, float dir)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { var mo = movingObjs[id]; mo.Dir = dir; });
    }

    // 设置飞机转向指定方向
    public void Turn2(int t, string id, Vec2 toDir, float tv)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() =>
        {
            var mo = movingObjs[id];
            mo.Turn2Dir = toDir;
            mo.TurnV = tv;
        });
    }

    #endregion

    void OnOp(string op, Action<int, IReadableBuffer> cb)
    {
        var mh = GameCore.Instance.Get<MessageHandler>();
        mh.OnOp("GameRoom/" + op, (conn, data) =>
        {
            var t = data.ReadInt() - timeNumBase;
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
            var vs = new float[cnt];
            var dirs = new float[cnt];
            var dirTos = new Vec2[cnt];
            var turnVs = new float[cnt];
            FC.For(cnt, (i) =>
            {
                ids[i] = data.ReadString();
                types[i] = data.ReadString();
                poses[i] = new Vec2(data.ReadFloat(), data.ReadFloat());
                vs[i] = data.ReadFloat();
                dirs[i] = data.ReadFloat();
                dirTos[i] = new Vec2(data.ReadFloat(), data.ReadFloat());
                turnVs[i] = data.ReadFloat();
            });

            SyncRoomStatus(t, ids, types, poses, vs, dirs, dirTos, turnVs);
        });

        OnOp("GameTimeFowardStep", (t, data) => { Dumb(t); });
        OnOp("AddIn", (t, data) =>
        {
            var id = data.ReadString();
            var type = data.ReadString();
            var pos = new Vec2(data.ReadFloat(), data.ReadFloat());
            var v = data.ReadFloat();
            var dir = data.ReadFloat();
            var dirTo = new Vec2(data.ReadFloat(), data.ReadFloat());
            var tv = data.ReadFloat();

            Add(t, id, type, pos, v, dir, dirTo, tv);
        });
        OnOp("RemoveOut", (t, data) => { var id = data.ReadString(); Del(t, id);});
        OnOp("Turn2", (t, data) =>
        {
            var id = data.ReadString();
            var dirTo = new Vec2(data.ReadFloat(), data.ReadFloat());
            var tv = data.ReadFloat();
            Turn2(t, id, dirTo, tv);
        });
    }
}
