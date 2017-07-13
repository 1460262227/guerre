using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;
using Guerre;

/// <summary>
/// 特效生成
/// </summary>
public class EffectCreator : MonoBehaviour {

    public AirplaneGhost Ghost = null;
    public void AirplaneGhost(int lv, Vec2 from, Vec2 to, float t)
    {
        var eff = Instantiate(Ghost) as AirplaneGhost;
        eff.gameObject.SetActive(true);
        eff.transform.SetParent(transform, false);
        eff.transform.localPosition = new Vector3((float)from.x, (float)from.y, 0);
        eff.StartEffect(lv, from, to, t);
    }

    public Shield Shield = null;
    public void AirplaneShield(MovableObjectController mc)
    {
        var eff = Instantiate(Shield) as Shield;
        eff.gameObject.SetActive(true);
        eff.transform.SetParent(mc.transform, false);
        eff.MC = mc;
    }
}
