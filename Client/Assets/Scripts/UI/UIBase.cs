using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    // 每次打开界面前初始化
    public virtual void Init() { }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

	public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
