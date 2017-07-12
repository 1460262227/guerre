using System.Collections;
using System.Collections.Generic;
using Swift.Math;
using UnityEngine;

/// <summary>
/// 飞机表现控制
/// </summary>
public class AirplaneController : MovableObjectController
{
    // 血条
    public Transform BloodBar;

    // 机身
    public Transform Body;

    // 机身特殊效果
    public AirplaneSprite AS;

    private void LateUpdate()
    {
        var turn2Dir = MO.TurnV == 0 ? MO.Dir : MO.Turn2Dir.Dir();
        var nowDir = MO.Dir;
        var da = (turn2Dir - nowDir).RangeInPi().Clamp(-1, 1);
        AS.TurnAngle = ((float)(da / MathEx.Pi) + AS.TurnAngle * 4) / 5;
    }

    public override Quaternion ShowRotation { get { return Body.localRotation; } set { Body.localRotation = value; } }
}
