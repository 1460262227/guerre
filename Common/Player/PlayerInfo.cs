using System;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace Guerre
{
    /// <summary>
    /// 玩家信息
    /// </summary>
    public class PlayerInfo : SerializableData
    {
        public string ID;
        public string Name;
        public string PWD;

        public int Money;

        protected override void Sync()
        {
            BeginSync();
            {
                SyncString(ref ID);
                SyncString(ref Name);
                SyncString(ref PWD);
                SyncInt(ref Money);
            }
            EndSync();
        }
    }
}
