using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;

public class SelAirplaneUI : UIBase {

    public Image[] Options;
    public GameObject[] OptionsBg;
    public Text Money;
    public Text Tips;

    int curSel = 0;

    void Awake()
    {
        GameCore.Instance.OnIn += Hide;
    }

    private void Start()
    {
        curSel = 0;
        UpdateSel();
        Tips.text = "";
    }
    
	public void OnSel(int n)
    {
        curSel = n;
        UpdateSel();
    }

    void UpdateSel()
    {
        var me = GameCore.Instance.Me;
        Money.text = me == null ? "0" : me.Money.ToString();
        FC.For(Options.Length, (i) =>
        {
            Options[i].color = i == curSel ? Color.white : new Color(0, 0, 0, 0);
            var unlocked = me != null && me.Level >= i;
            Options[i].gameObject.SetActive(unlocked);
            OptionsBg[i].gameObject.SetActive(!unlocked);
        });
    }

    public void Unlock(int lv)
    {
        APIs.Request("LoginMgr", "unlock", (buff) =>
        {
            buff.Write(lv);
        }, (data) =>
        {
            var ok = data.ReadBool();
            if (ok)
            {
                var me = GameCore.Instance.Me;
                me.Money = data.ReadInt();
                me.Level = data.ReadInt();
                UpdateSel();
            }

            Tips.text = ok ? "解锁成功" : "金币不足，解锁失败";
        }, null);
    }

    public void JoinGame()
    {
        GameCore.Instance.CurSelAirplane = curSel;
        APIs.Send("GameRoom/test", "Join", (buff) => { buff.Write(curSel); });
    }
}
