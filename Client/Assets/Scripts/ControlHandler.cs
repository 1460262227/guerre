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
        var dir = JS.CurrentPos;
        dir.Normalize();

        APIs.Send("GameRoom/test", "Turn2", (buff) => { buff.Write(dir.x); buff.Write(dir.y); buff.Write((Fix64)5); });
    }

    // 使用技能
    public void OnUseSkill(string skillName)
    {
        APIs.Send("GameRoom/test", "UseSkill", (buff) => { buff.Write(skillName); });
    }
}
