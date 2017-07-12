using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;

public class RebornUI : UIBase {

    public Text Kills;

    void Awake()
    {
        var gc = GameCore.Instance;
        gc.OnIn += Hide;
        gc.OnOut += Show;
    }

    public override void Show()
    {
        base.Show();
        Kills.text = GameCore.Instance.MyKills.ToString();
    }

    public void Join()
    {
        APIs.Send("GameRoom/test", "Join", (buff) =>
        {
            var sel = GameCore.Instance.CurSelAirplane;
            buff.Write(sel);
        });
    }
}
