using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Guerre;
using System;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    // 小圈指示当前摇杆位置
    public Transform Fg = null;

    // 背景圈
    public Transform Bg = null;

    // 当前摇杆操作位置
    public Vector2 CurrentPos = Vector2.zero;

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
    private void Update()
    {
        var divX = 0f;
        var divY = 0f;
        
        if (pressed)
        {
            var pos = Input.mousePosition - rt.position;
            var rect = bgRt.rect;
            divX = (pos.x / rect.width).Clamp(-0.5f, 0.5f);
            divY = (pos.y / rect.height).Clamp(-0.5f, 0.5f);
        }

        var v2 = new Vector2(divX, divY);
        var len = v2.magnitude;
        if (Mathf.Abs(len) > 0.5f)
            v2 = v2 / 2 / Mathf.Abs(len);

        CurrentPos = v2;
        fgRt.anchorMin = fgRt.anchorMax = CurrentPos + new Vector2(0.5f, 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
}
