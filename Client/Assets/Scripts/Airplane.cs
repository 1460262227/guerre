using System.Collections;
using System.Collections.Generic;
using Swift.Math;
using UnityEngine;

/// <summary>
/// 飞机表现控制
/// </summary>
public class Airplane : MovableObject
{
    // 血条
    public Transform BloodBar;

    // 机身
    public Transform Body;

    // 机身特殊效果
    public AirplaneSprite AS;

    public override Fix64 Hp
    {
        get { return base.Hp; }
        set
        {
            base.Hp = value;
            var div = (float)(Hp / MaxHp);
            BloodBar.localScale = new Vector3(div, 1, 1);
            BloodBar.localPosition = new Vector3(-(1 - div)/4, 0, 0);
        }
    }

    private void LateUpdate()
    {
        var turn2Dir = TurnV == 0 ? Dir : Turn2Dir.Dir();
        var nowDir = Dir;
        var da = (turn2Dir - nowDir).RangeInPi().Clamp(-1, 1);
        AS.TurnAngle = ((float)(da / MathEx.Pi) + AS.TurnAngle * 4) / 5;
    }

    public override Quaternion ShowRotation { get { return Body.localRotation; } set { Body.localRotation = value; } }
}
