using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;

public class StatisticsUI : MonoBehaviour {

    public Text[] Ranks;
    public Text MyKills;
    public Text MyMoney;

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

    private void Update()
    {
        var gc = GameCore.Instance;
        MyKills.text = gc.MyKills.ToString();
        MyMoney.text = gc.Me == null ? "0" : gc.Me.Money.ToString();
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

        if (killer == gc.Me.ID)
            gc.MyKills += n;
    }
}
