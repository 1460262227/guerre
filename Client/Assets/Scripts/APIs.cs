using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Guerre;
using System;

public static class APIs
{
    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    // 发送消息给服务器
    public static Action<string, string, Action<IWriteableBuffer>> SendImpl = null;

    // 发送请求给服务器
    public static Action<string, string, Action<IWriteableBuffer>, Action<IReadableBuffer>, Action<bool>> RequestImpl = null;
}
