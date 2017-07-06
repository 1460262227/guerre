using UnityEngine;
using System.Collections;
using Swift;

public class GameDriver : MonoBehaviour
{
    public const int MaxFrameRate = 40;
    public const int MinFrameRate = 30;
    
    // AssetBundle 使用开关
    public bool UseABSwitch = true;

    void Start()
    {
        // 初始化游戏核心
        GameCore.Instance.Init();
        BroadcastMessage("OnGameCoreInitialized");
    }

    void FixedUpdate()
    {
        int te = (int)(Time.fixedDeltaTime * 1000);
        GameCore.Instance.OnTimeElapsed(te);
    }
}
