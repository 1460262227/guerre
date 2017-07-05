using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public UIBase[] UIS = null;
    Dictionary<string, GameObject> uiObjs = new Dictionary<string, GameObject>();

    private void Start()
    {
        // register all ui
        foreach (var ui in UIS)
            uiObjs[ui.GetComponent<UIBase>().GetType().FullName] = ui.gameObject;
    }

    // 打开指定界面
    public T Show<T>() where T : UIBase
    {
        var ui = typeof(T).FullName;
        uiObjs[ui].SetActive(true);
        var s = uiObjs[ui].GetComponent<UIBase>() as T;
        s.Init();
        return s;
    }
}
