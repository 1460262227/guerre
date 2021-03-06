﻿using System.Collections;
using System.Collections.Generic;
using System;

namespace Swift
{
    public static class BufferExtension
    {
        public static void WriteObj(this IWriteableBuffer w, ISerializable v)
        {
            w.Write(v != null ? true : false);
            if (v != null)
                v.Serialize(w);
        }

        public static void WriteObj(this IWriteableBuffer w, ISerializable[] arr)
        {
            w.Write(arr != null ? true : false);
            if (arr != null)
            {
                w.Write(arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    WriteObj(w, arr[i]);
                }
            }
                
        }

        public static T ReadObj<T>(this IReadableBuffer r) where T : class, ISerializable, new()
        {
            bool hasValue = r.ReadBool();
            if (!hasValue)
                return null;
            else
            {
                T v = new T();
                v.Deserialize(r);
                return v;
            }
        }

        public static T[] ReadObjArr<T>(this IReadableBuffer r) where T : class, ISerializable, new()
        {
            bool hasValue = r.ReadBool();
            if (!hasValue)
                return null;
            else
            {
                int len = r.ReadInt();
                T[] arr = new T[len];
                for (int i = 0; i < len; i++)
                {
                    arr[i] = ReadObj<T>(r);
                }
                return arr;
            }
        }
    }
}