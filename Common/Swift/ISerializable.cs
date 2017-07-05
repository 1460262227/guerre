using System.Collections;
using System.Collections.Generic;
using System;

namespace Swift
{
    public interface ISerializable
    {
        void Serialize(IWriteableBuffer w);
        void Deserialize(IReadableBuffer r);
    }
}
