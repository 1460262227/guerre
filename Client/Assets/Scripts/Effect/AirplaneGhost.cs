using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

/// <summary>
/// 飞机残影效果
/// </summary>
public class AirplaneGhost : MonoBehaviour
{
    public GameObject SpriteModel;

    List<SpriteRenderer> srs = new List<SpriteRenderer>();
    float timeLeft;
    float timeTotal;

    public void StartEffect(int Level, Vec2 from, Vec2 to, float t)
    {
        timeLeft = t;
        timeTotal = t;
        var sprite = Resources.Load<Sprite>("Sprites/Airplanes/" + Level.ToString());
        var d = to - from;
        var dLen = d.Length;
        FC.For((int)(dLen * 10), (i) =>
        {
            var div = i / dLen / 10;
            var s = Instantiate(SpriteModel) as GameObject;
            s.SetActive(true);
            s.transform.SetParent(transform, false);
            var sr = s.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = sprite;
            var pos = div * d;
            s.transform.localPosition = new Vector3((float)pos.x, (float)pos.y, 0);
            s.transform.localRotation = Quaternion.Euler(0, 0, (float)(d.Dir() * MathEx.Rad2Deg));
            sr.color = new Color(1, 1, 1, (float)div);
            srs.Add(sr);
        });
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        timeLeft -= dt;
        if (timeLeft <= 0)
        {
            Destroy(gameObject);
            return;
        }

        var da = dt / timeTotal;
        foreach (var sr in srs.ToArray())
        {
            var a = sr.color.a;
            a -= da;
            if (a > 0)
                sr.color = new Color(1, 1, 1, a);
            else
            {
                srs.Remove(sr);
                Destroy(sr.gameObject);
            }
        }
    }
}
