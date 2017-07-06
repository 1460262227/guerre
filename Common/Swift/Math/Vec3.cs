using System;
using System.Collections.Generic;
using System.Text;

namespace Swift.Math
{
    // 3D 浮点向量
    public struct Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3(float vx, float vy, float vz)
        {
            x = vx;
            y = vy;
            z = vz;
        }

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public static Vec3 operator + (Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vec3 operator - (Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vec3 operator *(Vec3 v, float scale)
        {
            return new Vec3(v.x * scale , v.y * scale, v.z * scale);
        }

        public static Vec3 operator *(float scale, Vec3 v)
        {
            return new Vec3(v.x * scale, v.y * scale, v.z * scale);
        }

        public static Vec3 operator /(Vec3 v, float scale)
        {
            return new Vec3(v.x / scale, v.y / scale, v.z / scale);
        }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.x, -v.y, -v.z);
        }

        public static bool operator == (Vec3 v1, Vec3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public static bool operator !=(Vec3 v1, Vec3 v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            return this == (Vec3)obj;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Normalize()
        {
            var len = Length;
            if (System.Math.Abs(len) > float.Epsilon)
            {
                x /= len;
                y /= len;
                z /= len;
            }
            else
            {
                x = 1;
                y = 0;
                z = 0;
            }
        }
        public static void DistanceSquared(ref Vec3 value1, ref Vec3 value2, out float result)
        {
            result = (value1.x - value2.x) * (value1.x - value2.x) +
                     (value1.y - value2.y) * (value1.y - value2.y) +
                     (value1.z - value2.z) * (value1.z - value2.z);
        }

        public float LengthSquared()
        {
            float result;
            DistanceSquared(ref this, ref Zero, out result);
            return result;
        }

        public static Vec3 Cross(Vec3 v1, Vec3 v2)
        {
            Cross(ref v1, ref v2, out v1);
            return v1;
        }

        public static void Cross(ref Vec3 v1, ref Vec3 v2, out Vec3 r)
        {
            r = new Vec3(v1.y * v2.z - v2.y * v1.z,
                            -(v1.x * v2.z - v2.x * v1.z),
                            v1.x * v2.y - v2.x * v1.y);
        }

        public static Vec3 Normalize(Vec3 vector)
        {
            Normalize(ref vector, out vector);
            return vector;
        }

        public static void Normalize(ref Vec3 value, out Vec3 result)
        {
            float factor = value.Length;
            factor = 1f / factor;
            result.x = value.x * factor;
            result.y = value.y * factor;
            result.z = value.z * factor;
        }

        public static float Dot(Vec3 v1, Vec3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static void Dot(ref Vec3 v1, ref Vec3 v2, out float r)
        {
            r = v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static Vec3 Zero = new Vec3(0, 0, 0);
        public static Vec3 Left = new Vec3(-1, 0, 0);
        public static Vec3 Right = new Vec3(1, 0, 0);
        public static Vec3 Up = new Vec3(0, 1, 0);
        public static Vec3 Down = new Vec3(0, -1, 0);
        public static Vec3 Forward = new Vec3(0, 0, 1);
        public static Vec3 Backward = new Vec3(0, 0, -1);
    }
}
