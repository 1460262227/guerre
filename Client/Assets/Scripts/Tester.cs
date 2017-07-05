using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;
using Swift;
using Swift.Math;

public class Tester : MonoBehaviour {

    public GameWorld GW = null;
    public Joystick JS = null;

	// Use this for initialization
	void Start () {
        GW.Add(0, "me", 0, Vec2.Zero, MathEx.Up, 1f);
        StartCoroutine(PushTime(1));
    }

    IEnumerator PushTime(int start)
    {
        int t = start;
        while (true)
        {
            if (JS.CurrentPos != Vector2.zero)
            {
                var toDir = new Vec2(JS.CurrentPos.x, JS.CurrentPos.y);
                toDir.Normalize();
                GW.Turn2(t, "me", toDir, 5);
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
