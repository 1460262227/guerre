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
        GW.Add(0, "me", "Airplane", Vec2.Zero, 1f, MathEx.Up, Vec2.Zero, 0);
        StartCoroutine(PushTime(1));
    }

    IEnumerator PushTime(int start)
    {
        int t = start;
        while (true)
        {
            if (JS.CurrentPos != Vec2.Zero)
            {
                var toDir = new Vec2(JS.CurrentPos.x, JS.CurrentPos.y);
                toDir.Normalize();
                GW.Turn2(t, "me", toDir, 1);
            }
            else
                GW.Turn2(t, "me", Vec2.Zero, 0);

            t++;
            yield return new WaitForSeconds(0.1f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
