using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;
using Swift;
using Swift.Math;

public class Tester : MonoBehaviour {

    public GameObject LoginUIObj = null;
    public ControlHandler CH = null;
    public GameWorld GW = null;
    public Joystick JS = null;

	// Use this for initialization
	void Start () {
        LoginUIObj.SetActive(false);
        CH.enabled = false;
        var me = new PlayerInfo();
        me.ID = "me";
        GameCore.Instance.Me = me;

        var a = new MovableObjectInfo();
        a.ID = me.ID;
        a.Type = "Airplane";
        a.Level = 0;
        a.Pos = Vec2.Zero;
        a.Velocity = 1;
        a.MaxTurnV = 1;
        a.TurnV = 0;
        a.Dir = MathEx.Up;
        a.Turn2Dir = Vec2.Zero;
        a.Hp = a.MaxHp = 10;
        a.Mp = a.MaxMp = 10;
        GW.Add(0, a);

        StartCoroutine(PushTime(1));
    }

    IEnumerator PushTime(int start)
    {
        int t = start;
        var me = GameCore.Instance.Me;
        while (true)
        {
            if (JS.CurrentPos != Vec2.Zero)
            {
                var toDir = new Vec2(JS.CurrentPos.x, JS.CurrentPos.y);
                toDir.Normalize();
                GW.Turn2(t, me.ID, toDir, 1);
            }
            else
                GW.Turn2(t, me.ID, Vec2.Zero, 0);

            t++;
            yield return new WaitForSeconds(0.05f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
