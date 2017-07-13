using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guerre;

public class Shield : MonoBehaviour {

    public MovableObjectController MC;

    // Update is called once per frame
    SpriteRenderer sr = null;
    void Update ()
    {
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        var a = MC.Sheild / MC.MaxMp;
        if (a <= 0)
        {
            Destroy(gameObject);
            return;
        }
        else
            sr.color = new Color(1, 1, 1, (float)a);

    }
}
