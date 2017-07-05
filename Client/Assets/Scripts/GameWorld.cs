﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;

// 一个游戏世界代表一局游戏
public class GameWorld : MonoBehaviour {

    public Rect WorldRect = new Rect(-1, -1, 2, 2);

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
    public void AddAirplane(string id, int type, Vec2 pos, float dir, float velocity)
    {
        if (movingObjs.ContainsKey(id))
            throw new Exception("airplane id conflicted: " + id);

        if (type < 0 || type >= AirplaneModels.Length)
            throw new Exception("no such airplane type: " + type);

        // 添加模型
        var pGo = Instantiate(AirplaneModels[type].gameObject) as GameObject;
        pGo.SetActive(true);
        pGo.transform.SetParent(SceneRoot, false);

        // 设置飞机属性
        var a = pGo.GetComponent<Airplane>();
        a.ID = id;
        a.Pos = pos;
        a.Dir = dir;
        a.Velocity = velocity;
        movingObjs[id] = a;
    }

    // 移除一架飞机
    public void DelAirplane(string id)
    {
        if (!movingObjs.ContainsKey(id))
            throw new Exception("airplane id not exists: " + id);
    }

    // 等待执行的指令列表
    List<List<Action>> commanders = new List<List<Action>>();

    // 当前指令位置
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

                // 延迟超过 1 秒了，加速播放
                timeTime = (commanders.Count - curCmdIndex >= 10) ? 10 : 1;
            }
            else
                return;
        }

        te = te * timeTime;
        timeElapsed += (int)(te * 1000);

        // 推动飞机运动
        foreach (var mo in movingObjs.Values)
        {
            mo.MoveForward(te);

            // 碰到四边自动折返
            var dv = mo.DirV2;
            if (mo.Pos.x < WorldRect.xMin)
            {
                mo.Pos = new Vec2(WorldRect.xMin * 2 - mo.Pos.x, mo.Pos.y);
                mo.DirV2 = new Vec2(-dv.x, dv.y);
            }
            else if (mo.Pos.x > WorldRect.xMax)
            {
                mo.Pos = new Vec2(WorldRect.xMax * 2 - mo.Pos.x, mo.Pos.y);
                mo.DirV2 = new Vec2(-dv.x, dv.y);
            }
            else if (mo.Pos.y < WorldRect.yMin)
            {
                mo.Pos = new Vec2(mo.Pos.x, WorldRect.yMin * 2 - mo.Pos.y);
                mo.DirV2 = new Vec2(dv.x, -dv.y);
            }
            else if (mo.Pos.y > WorldRect.yMax)
            {
                mo.Pos = new Vec2(mo.Pos.x, WorldRect.yMax * 2 - mo.Pos.y);
                mo.DirV2 = new Vec2(dv.x, -dv.y);
            }
        }
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
    public void Add(int t, string id, int type, Vec2 pos, float dir, float velocity)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { AddAirplane(id, type, pos, dir, velocity); });
    }

    // 移除飞机
    public void Del(int t, string id)
    {
        var cmds = RetrieveCmds(t);
        cmds.Add(() => { DelAirplane(id); });
    }

    // 给定时间编号开始，到一定时间编号长度内，无指令
    public void Skip(int t, int tLen = 1)
    {
        FC.For(t, t + tLen, (i) => { RetrieveCmds(i).Add(null); });
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
}
