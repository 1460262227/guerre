// A custom completely managed implementation of UnityEngine.Quat
// Base is decompiled UnityEngine.Quat
// Doesn't implement methods marked Obsolete
// Does implicit coversions to and from UnityEngine.Quat

// Uses code from:
// https://raw.githubusercontent.com/mono/opentk/master/Source/OpenTK/Math/Quat.cs
// http://answers.unity3d.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html
// http://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
// http://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha

using System;
using Swift;

//
// Summary:
//     ///
//     Quaternions are used to represent rotations.
//     ///
//[DefaultMember("Item")]
namespace Swift.Math
{
    public struct Quat : IEquatable<Quat>
    {
        const float radToDeg = (float)(180.0 / MathEx.Pi);
        const float degToRad = (float)(MathEx.Pi / 180.0);

        public const float kEpsilon = 1E-06f; // should probably be used in the 0 tests in LookRotation or Slerp
        public Vec3 xyz
        {
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
            get
            {
                return new Vec3(x, y, z);
            }
        }
        /// <summary>
        ///   <para>X component of the Quat. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float x;
        /// <summary>
        ///   <para>Y component of the Quat. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float y;
        /// <summary>
        ///   <para>Z component of the Quat. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float z;
        /// <summary>
        ///   <para>W component of the Quat. Don't modify this directly unless you know quaternions inside out.</para>
        /// </summary>
        public float w;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    case 3:
                        return this.w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quat index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quat index!");
                }
            }
        }
        /// <summary>
        ///   <para>The identity rotation (RO).</para>
        /// </summary>
        public static Quat identity
        {
            get
            {
                return new Quat(0f, 0f, 0f, 1f);
            }
        }
        /// <summary>
        ///   <para>Returns the euler angle representation of the rotation.</para>
        /// </summary>
        public Vec3 eulerAngles
        {
            get
            {
                return Quat.Internal_ToEulerRad(this) * radToDeg;
            }
            set
            {
                this = Quat.Internal_FromEulerRad(value * degToRad);
            }
        }





        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quat to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            xyz *= scale;
            w *= scale;
        }

        #endregion


        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quat Normalize(Quat q)
        {
            Quat result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quat q, out Quat result)
        {
            float scale = 1.0f / q.Length;
            result = new Quat(q.xyz * scale, q.w * scale);
        }

        #endregion


        /// <summary>
        ///   <para>Constructs new Quat with given x,y,z,w components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Construct a new Quat from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quat(Vec3 v, float w)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = w;
        }


        /// <summary>
        ///   <para>Set x, y, z and w components of an existing Quat.</para>
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        /// <param name="new_w"></param>
        public void Set(float new_x, float new_y, float new_z, float new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }
        /// <summary>
        ///   <para>The dot product between two rotations.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Dot(Quat a, Quat b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates /angle/ degrees around /axis/.</para>
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public static Quat AngleAxis(float angle, Vec3 axis)
        {
            return Quat.INTERNAL_CALL_AngleAxis(angle, ref axis);
        }
        private static Quat INTERNAL_CALL_AngleAxis(float degress, ref Vec3 axis)
        {
            if (axis.LengthSquared() == 0.0f)
                return identity;

            Quat result = identity;
            var radians = degress * degToRad;
            radians *= 0.5f;
            axis.Normalize();
            axis = axis * (float)System.Math.Sin(radians);
            result.x = axis.x;
            result.y = axis.y;
            result.z = axis.z;
            result.w = (float)System.Math.Cos(radians);

            return Normalize(result);
        }
        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            Quat.Internal_ToAxisAngleRad(this, out axis, out angle);
            angle *= radToDeg;
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            return RotateTowards(LookRotation(fromDirection), LookRotation(toDirection), float.MaxValue);
        }
        /// <summary>
        ///   <para>Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            this = Quat.FromToRotation(fromDirection, toDirection);
        }
        /// <summary>
        ///   <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
        /// </summary>
        /// <param name="forward">The direction to look in.</param>
        /// <param name="upwards">The vector that defines in which direction up is.</param>
        public static Quat LookRotation(Vec3 forward, Vec3 upwards)
        {
            return Quat.INTERNAL_CALL_LookRotation(ref forward, ref upwards);
        }

        public static Quat LookRotation(Vec3 forward)
        {
            Vec3 up = Vec3.Up;
            return Quat.INTERNAL_CALL_LookRotation(ref forward, ref up);
        }

        // from http://answers.unity3d.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html
        private static Quat INTERNAL_CALL_LookRotation(ref Vec3 forward, ref Vec3 up)
        {

            forward = Vec3.Normalize(forward);
            Vec3 right = Vec3.Normalize(Vec3.Cross(up, forward));
            up = Vec3.Cross(forward, right);
            var m00 = right.x;
            var m01 = right.y;
            var m02 = right.z;
            var m10 = up.x;
            var m11 = up.y;
            var m12 = up.z;
            var m20 = forward.x;
            var m21 = forward.y;
            var m22 = forward.z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quat();
            if (num8 > 0f)
            {
                var num = (float)System.Math.Sqrt(num8 + 1f);
                quaternion.w = num * 0.5f;
                num = 0.5f / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)System.Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.x = 0.5f * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)System.Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5f * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)System.Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5f * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }

        public void SetLookRotation(Vec3 view)
        {
            Vec3 up = Vec3.Up;
            this.SetLookRotation(view, up);
        }
        /// <summary>
        ///   <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
        /// </summary>
        /// <param name="view">The direction to look in.</param>
        /// <param name="up">The vector that defines in which direction up is.</param>
        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            this = Quat.LookRotation(view, up);
        }
        /// <summary>
        ///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat Slerp(Quat a, Quat b, float t)
        {
            return Quat.INTERNAL_CALL_Slerp(ref a, ref b, t);
        }

        private static Quat INTERNAL_CALL_Slerp(ref Quat a, ref Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
        }
        /// <summary>
        ///   <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is not clamped.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat SlerpUnclamped(Quat a, Quat b, float t)
        {
            return Quat.INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
        }

        private static Quat INTERNAL_CALL_SlerpUnclamped(ref Quat a, ref Quat b, float t)
        {
            // if either input is zero, return the other.
            if (a.LengthSquared == 0.0f)
            {
                if (b.LengthSquared == 0.0f)
                {
                    return identity;
                }
                return b;
            }
            else if (b.LengthSquared == 0.0f)
            {
                return a;
            }


            float cosHalfAngle = a.w * b.w + Vec3.Dot(a.xyz, b.xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return a;
            }
            else if (cosHalfAngle < 0.0f)
            {
                b.xyz = -b.xyz;
                b.w = -b.w;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - t;
                blendB = t;
            }

            Quat result = new Quat(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return identity;
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is clamped to the range [0, 1].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat Lerp(Quat a, Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return INTERNAL_CALL_Slerp(ref a, ref b, t); // TODO: use lerp not slerp, "Because quaternion works in 4D. Rotation in 4D are linear" ???
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is not clamped.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat LerpUnclamped(Quat a, Quat b, float t)
        {
            return INTERNAL_CALL_Slerp(ref a, ref b, t);
        }
        /// <summary>
        ///   <para>Rotates a rotation /from/ towards /to/.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
        {
            float num = Quat.Angle(from, to);
            if (num == 0f)
            {
                return to;
            }
            float t = (float)System.Math.Min(1f, maxDegreesDelta / num);
            return Quat.SlerpUnclamped(from, to, t);
        }
        /// <summary>
        ///   <para>Returns the Inverse of /rotation/.</para>
        /// </summary>
        /// <param name="rotation"></param>
        public static Quat Inverse(Quat rotation)
        {
            float lengthSq = rotation.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                return new Quat(rotation.xyz * -i, rotation.w * i);
            }
            return rotation;
        }
        /// <summary>
        ///   <para>Returns a nicely formatted string of the Quat.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", new object[]
            {
                this.x,
                this.y,
                this.z,
                this.w
            });
        }
        /// <summary>
        ///   <para>Returns a nicely formatted string of the Quat.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", new object[]
            {
                this.x.ToString(format),
                this.y.ToString(format),
                this.z.ToString(format),
                this.w.ToString(format)
            });
        }
        /// <summary>
        ///   <para>Returns the angle in degrees between two rotations /a/ and /b/.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Angle(Quat a, Quat b)
        {
            float f = Quat.Dot(a, b);
            return (float)System.Math.Acos(System.Math.Min(System.Math.Abs(f), 1f)) * 2f * radToDeg;
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat Euler(double x, double y, double z)
        {
            return Quat.Internal_FromEulerRad(new Vec3((float)x, (float)y, (float)z) * degToRad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat Euler(float x, float y, float z)
        {
            return Quat.Internal_FromEulerRad(new Vec3(x, y, z) * degToRad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="euler"></param>
        public static Quat Euler(Vec3 euler)
        {
            return Quat.Internal_FromEulerRad(euler * degToRad);

        }

        // from http://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        private static Vec3 Internal_ToEulerRad(Quat rotation)
        {
            float sqw = rotation.w * rotation.w;
            float sqx = rotation.x * rotation.x;
            float sqy = rotation.y * rotation.y;
            float sqz = rotation.z * rotation.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = rotation.x * rotation.w - rotation.y * rotation.z;
            Vec3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = 2f * (float)System.Math.Atan2(rotation.y, rotation.x);
                v.x = MathEx.Pi / 2;
                v.z = 0;
                return NormalizeAngles(v * MathEx.Rad2Deg);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * (float)System.Math.Atan2(rotation.y, rotation.x);
                v.x = -MathEx.Pi / 2;
                v.z = 0;
                return NormalizeAngles(v * MathEx.Rad2Deg);
            }
            Quat q = new Quat(rotation.w, rotation.z, rotation.x, rotation.y);
            v.y = (float)System.Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = (float)System.Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = (float)System.Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
            return NormalizeAngles(v * MathEx.Rad2Deg);
        }
        private static Vec3 NormalizeAngles(Vec3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        // from http://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha
        private static Quat Internal_FromEulerRad(Vec3 euler)
        {
            var yaw = euler.x;
            var pitch = euler.y;
            var roll = euler.z;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)System.Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)System.Math.Cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)System.Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)System.Math.Cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)System.Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)System.Math.Cos((double)yawOver2);
            Quat result;
            result.x = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            result.z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.w = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            return result;

        }
        private static void Internal_ToAxisAngleRad(Quat q, out Vec3 axis, out float angle)
        {
            if (System.Math.Abs(q.w) > 1.0f)
                q.Normalize();


            angle = 2.0f * (float)System.Math.Acos(q.w); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
            if (den > 0.0001f)
            {
                axis = q.xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = new Vec3(1, 0, 0);
            }
        }



        #region Obsolete methods
        /*
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerRotation(float x, float y, float z)
        {
            return Quat.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerRotation(Vec3 euler)
        {
            return Quat.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(float x, float y, float z)
        {
            this = Quat.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerRotation(Vec3 euler)
        {
            this = Quat.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quat.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vec3 ToEuler()
        {
            return Quat.Internal_ToEulerRad(this);
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerAngles(float x, float y, float z)
        {
            return Quat.Internal_FromEulerRad(new Vec3(x, y, z));
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat EulerAngles(Vec3 euler)
        {
            return Quat.Internal_FromEulerRad(euler);
        }
        [Obsolete("Use Quat.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void ToAxisAngle(out Vec3 axis, out float angle)
        {
            Quat.Internal_ToAxisAngleRad(this, out axis, out angle);
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(float x, float y, float z)
        {
            this.SetEulerRotation(new Vec3(x, y, z));
        }
        [Obsolete("Use Quat.Euler instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetEulerAngles(Vec3 euler)
        {
            this = Quat.EulerRotation(euler);
        }
        [Obsolete("Use Quat.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public static Vec3 ToEulerAngles(Quat rotation)
        {
            return Quat.Internal_ToEulerRad(rotation);
        }
        [Obsolete("Use Quat.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
        public Vec3 ToEulerAngles()
        {
            return Quat.Internal_ToEulerRad(this);
        }
        [Obsolete("Use Quat.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public static Quat AxisAngle(Vec3 axis, float angle)
        {
            return Quat.INTERNAL_CALL_AxisAngle(ref axis, angle);
        }
        private static Quat INTERNAL_CALL_AxisAngle(ref Vec3 axis, float angle)
        {
        }
        [Obsolete("Use Quat.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        public void SetAxisAngle(Vec3 axis, float angle)
        {
            this = Quat.AxisAngle(axis, angle);
        }
        */
        #endregion
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }
        public override bool Equals(object other)
        {
            if (!(other is Quat))
            {
                return false;
            }
            Quat quaternion = (Quat)other;
            return this.x.Equals(quaternion.x) && this.y.Equals(quaternion.y) && this.z.Equals(quaternion.z) && this.w.Equals(quaternion.w);
        }

        public bool Equals(Quat other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
        }

        public static Quat operator *(Quat lhs, Quat rhs)
        {
            return new Quat(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }
        public static Vec3 operator *(Quat rotation, Vec3 point)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vec3 result;
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }
        public static bool operator ==(Quat lhs, Quat rhs)
        {
            return Quat.Dot(lhs, rhs) > 0.999999f;
        }
        public static bool operator !=(Quat lhs, Quat rhs)
        {
            return Quat.Dot(lhs, rhs) <= 0.999999f;
        }
    }
}
