using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                APIs.Send("GameRoom/test", "Turn2", (buff) => { buff.Write(0); buff.Write(0); buff.Write(0); });
            }

            return;
        }

        pressedLasttime = true;
        var dir = JS.CurrentPos;
        dir.Normalize();

        APIs.Send("GameRoom/test", "Turn2", (buff) => { buff.Write(dir.x); buff.Write(dir.y); buff.Write(5f); });
    }
}
