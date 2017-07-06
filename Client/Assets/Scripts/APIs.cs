using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Guerre;
using System;

public static class APIs
{
    // 发送消息给服务器
    public static Action<string, string, Action<IWriteableBuffer>> SendImpl = null;

    // 发送请求给服务器
    public static Action<string, string, Action<IWriteableBuffer>, Action<IReadableBuffer>, Action<bool>> RequestImpl = null;
}
