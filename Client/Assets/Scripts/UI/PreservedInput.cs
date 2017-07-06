using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 利用 PlayerPrefs 保存 Input 值
/// </summary>
public class PreservedInput : MonoBehaviour {

    public string Key;
    public string DefaultValue;

    InputField input;
    string curValue;

    private void Start()
    {
        input = GetComponent<InputField>();
        if (input == null)
            return;

        if (PlayerPrefs.HasKey(Key))
            input.text = PlayerPrefs.GetString(Key);
        else
            input.text = DefaultValue;

        curValue = input.text;
    }

    private void Update()
    {
        if (input == null)
            return;

        if (input.text == curValue)
            return;

        curValue = input.text;
        PlayerPrefs.SetString(Key, curValue);
    }
}
