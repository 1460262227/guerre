using System.Collections;
using System.Collections.Generic;
using Swift.Math;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 飞机表现控制
/// </summary>
public class AirplaneController : MovableObjectController
{
    // 血条
    Transform HpBar;

    // 能量条
    Transform MpBar;

    // 国旗
    public SpriteRenderer NationalFlag;

    // 机身
    public SpriteRenderer Body;
    public Transform BodyTransform;

    // 机身特殊效果
    public AirplaneSprite AS;

    void Start()
    {
        HpBar = transform.FindChild("Root/HpBar/bar");
        MpBar = transform.FindChild("Root/MpBar/bar");

        var lv = Level;
        NationalFlag.sprite = Resources.Load<Sprite>("Sprites/National/" + lv.ToString());
        Body.sprite = Resources.Load<Sprite>("Sprites/Airplanes/" + lv.ToString());
    }

    private void LateUpdate()
    {
        var turn2Dir = TurnV == 0 ? Dir : Turn2Dir.Dir();
        var nowDir = Dir;
        var da = (turn2Dir - nowDir).RangeInPi().Clamp(-1, 1);
        AS.TurnAngle = ((float)(da / MathEx.Pi) + AS.TurnAngle * 4) / 5;

        RefreshHpBar();
        RefreshMpBar();
    }

    public void RefreshHpBar()
    {
        var r = (float)(Hp / MaxMp);
        HpBar.localScale = new Vector3(r, 1, 1);
        HpBar.localPosition = new Vector3(-(1 - r) / 4, 0, 0);
    }

    public void RefreshMpBar()
    {
        var v = (Speeding > 0 ? Speeding : (Sheild > 0 ? Sheild : Mp));
        var r = (float)(v / MaxMp);
        MpBar.localScale = new Vector3(r, 1, 1);
        MpBar.localPosition = new Vector3(-(1 - r) / 4, 0, 0);
    }

    public override Quaternion ShowRotation { get { return BodyTransform.localRotation; } set { BodyTransform.localRotation = value; } }
}
