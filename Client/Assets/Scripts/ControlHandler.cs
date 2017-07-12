using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

// 处理玩家的操控信息
public class ControlHandler : MonoBehaviour
{
    public GameWorld GW = null;
    public Joystick JS = null;

    // Update is called once per frame
    bool pressedLasttime = false;
	void Update ()
    {
        if (!JS.Pressed)
        {
            if (pressedLasttime)
            {
                pressedLasttime = false;
                APIs.Send("GameRoom/test", "Turn2", (buff) => { buff.Write(Fix64.Zero); buff.Write(Fix64.Zero); buff.Write(Fix64.Zero); });
            }

            return;
        }

        pressedLasttime = true;
        var dirV2 = JS.CurrentPos;
        dirV2.Normalize();

        var TurnV = GameCore.Instance.MeOC.MaxTurnV;
        APIs.Send("GameRoom/test", "Turn2", (buff) => { buff.Write(dirV2.x); buff.Write(dirV2.y); buff.Write(TurnV); });
        var mc = GameCore.Instance.MeOC;

        // 提前一帧
        var dt = 0.05f;
        var dd = MathEx.CalcDir4Turn2(mc.DirV2, dirV2, TurnV * dt);
        var dp = mc.Velocity * dt;

        mc.PreTurnV = TurnV;
        mc.PreDir = mc.Dir + dd;

        var dx = MathEx.Cos(mc.Dir + dd) * dp;
        var dy = MathEx.Sin(mc.Dir + dd) * dp;
        mc.PrePos = mc.Pos + new Vec2(dx, dy);
    }

    // 使用技能
    public void OnUseSkill(string skillName)
    {
        APIs.Send("GameRoom/test", "UseSkill", (buff) => { buff.Write(skillName); });
    }
}
