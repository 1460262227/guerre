﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;

public class SelAirplaneUI : UIBase {

    public Image[] Options;

    int curSel = 0;

    void Awake()
    {
        GameCore.Instance.OnIn += Hide;
    }

    private void Start()
    {
        curSel = 0;
        UpdateSel();
    }
    
	public void OnSel(int n)
    {
        curSel = n;
        UpdateSel();
    }

    void UpdateSel()
    {
        FC.For(Options.Length, (i) => { Options[i].color = i == curSel ? Color.white : new Color(0, 0, 0, 0); });
    }

    public void JoinGame()
    {
        GameCore.Instance.CurSelAirplane = curSel;
        APIs.Send("GameRoom/test", "Join", (buff) => { buff.Write(curSel); });
    }
}