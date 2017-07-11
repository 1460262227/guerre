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

    public override Quaternion ShowRotation { get { return Body.localRotation; } set { Body.localRotation = value; } }
}
