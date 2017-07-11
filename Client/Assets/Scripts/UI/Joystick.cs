using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Guerre;
using System;
using Swift;
using Swift.Math;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    // 小圈指示当前摇杆位置
    public Transform Fg = null;

    // 背景圈
    public Transform Bg = null;

    // 当前摇杆操作位置
    public Vec2 CurrentPos = Vec2.Zero;
    public bool Pressed { get { return pressed; } }

    RectTransform rt = null;
    RectTransform fgRt = null;
    RectTransform bgRt = null;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        fgRt = Fg.GetComponent<RectTransform>();
        bgRt = Bg.GetComponent<RectTransform>();
    }

    bool pressed = false; // 摇杆是否按下
    int pointerId = -1;
    private void Update()
    {
        var divX = 0f;
        var divY = 0f;

        var moveR = 0.25f;

        if (pressed)
        {
            var pos = (pointerId < 0 ? (Vector2)Input.mousePosition : Input.GetTouch(pointerId).position) - (Vector2)rt.position;
            var rect = bgRt.rect;
            divX = (pos.x / rect.width).Clamp(-moveR, moveR);
            divY = (pos.y / rect.height).Clamp(-moveR, moveR);
        }

        var v2 = new Vector2(divX, divY);
        var len = v2.magnitude;
        if (Mathf.Abs(len) > moveR)
            v2 = v2 * moveR / Mathf.Abs(len);

        CurrentPos = new Vec2(v2.x, v2.y);
        fgRt.anchorMin = fgRt.anchorMax = v2 + new Vector2(0.5f, 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        pointerId = eventData.pointerId;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
}
