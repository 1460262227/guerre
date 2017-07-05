using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;

namespace Swift
{
    public interface IUnique
    {
        string UID { get; set; }
    }

    public class UniqueDataItem : DataItem<string>, IUnique
    {
        public UniqueDataItem() { }
        public UniqueDataItem(string id)
        {
            ID = id;
        }

        public string UID
        {
            get { return ID; }
            set { ID = value; }
        }

        protected override void Sync()
        {
            BeginSync();
                SyncString(ref ID);
            EndSync();
        }
    }
}
