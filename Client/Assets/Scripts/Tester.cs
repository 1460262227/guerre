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
        GW.Add(0, me.ID, "Airplane", Vec2.Zero, 1f, MathEx.Up, Vec2.Zero, 0, 10, 10, 10);
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
