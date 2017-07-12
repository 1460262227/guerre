﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;

public class StatisticsUI : MonoBehaviour {

    public Text[] Ranks;
    public Text MyKills;

    // 击杀和被击杀统计
    Dictionary<string, int> Kills = new Dictionary<string, int>();

    private void Awake()
    {
        GameCore.Instance.OnKill += OnKill;
        GameCore.Instance.RoomSynchonizing += () =>
        {
            Kills.Clear();
            foreach (var r in Ranks)
                r.text = "";
        };
    }

    public void OnKill(string killer, int n)
    {
        var gc = GameCore.Instance;
        var ks = Kills.ContainsKey(killer) ? Kills[killer] : 0;
        Kills[killer] = ks + n;

        var keys = Kills.SortKeysByValues(false);
        FC.For(Ranks.Length, (i) =>
        {
            Ranks[i].text = keys.Length > i ? Kills[keys[i]] + ": " + keys[i] : "";
        });

        gc.MyKills = Kills.ContainsKey(gc.Me.ID) ? Kills[gc.Me.ID] : 0;
        MyKills.text = gc.MyKills.ToString();
    }
}
