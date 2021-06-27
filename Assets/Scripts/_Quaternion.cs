using System;
using UnityEngine;
[Serializable]
public struct _Quaternion : IEquatable<_Quaternion>
{
    #region VARIABLES
    public float x;
    public float y;
    public float z;
    public float w;
    #endregion

    #region CONSTANTS
    public const float epsilon = 1E-06f;
    #endregion

    #region STATIC PROPERTIES
    public static _Quaternion identity => new _Quaternion(0.0f, 0.0f, 0.0f, 1);
    #endregion

    #region PROPERTIES
    public Vector3 eulerAngles
    {
        get
        {
            double toDegrees = 180 / Math.PI;

            double roll = toDegrees * Math.Atan2(2 * (w * x + y * z), 1 - 2 * (x * x + y * y));

            double sinp = 2 * (w * y - z * x);
            double pitch = toDegrees * Math.Asin(sinp);
            if (Math.Abs(sinp) >= 1)
                pitch = Math.PI / 2 * Math.Sign(sinp);

            double yaw = toDegrees * Math.Atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z));

            return new Vector3((float)roll, (float)pitch, (float)yaw);
        }
        set
        {
            float cy = Mathf.Cos(value.z * 0.5f);
            float sy = Mathf.Sin(value.z * 0.5f);
            float cp = Mathf.Cos(value.y * 0.5f);
            float sp = Mathf.Sin(value.y * 0.5f);
            float cr = Mathf.Cos(value.x * 0.5f);
            float sr = Mathf.Sin(value.x * 0.5f);

            _Quaternion q;

            q.w = cr * cp * cy + sr * sp * sy;
            q.x = sr * cp * cy - cr * sp * sy;
            q.y = cr * sp * cy + sr * cp * sy;
            q.z = cr * cp * sy - sr * sp * cy;

            this = q;
        }
    }
    public _Quaternion normalized => Normalize(this);
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
                    return 0;
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
                    break;
            }
        }
    }
    #endregion

    #region CONSTRUCTOR
    public _Quaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    #endregion

    #region PUBLIC METHODS
    public void Set(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        this = FromToRotation(fromDirection, toDirection);
    }
    public void SetLookRotation(Vector3 view, Vector3 up)
    {
        this = LookRotation(view, up);
    }
    public void ToAngleAxis(out float angle, out Vector3 axis)
    {
        Normalize(this);
        angle = 2.0f * (float)Math.Acos(w);
        float den = (float)Math.Sqrt(1.0 - w * w);
        if (den > 0.0001f)
            axis = new Vector3(x, y, z) / den;
        else
            axis = new Vector3(1, 0, 0);
        angle *= (float)(180 / Math.PI);
    }
    #endregion

    #region STATIC METHODS
    public static float Angle(_Quaternion a, _Quaternion b)
    {
        float num = Dot(a, b);
        return (float)((double)Mathf.Acos(Mathf.Min(Mathf.Abs(num), 1f)) * 2.0 * 57.2957801818848);
    }
    public static _Quaternion AngleAxis(float angle, Vector3 axis)
    {
        axis.Normalize();
        float rad = angle * Mathf.Deg2Rad * 0.5f;
        axis *= Mathf.Sin(rad);
        return new _Quaternion(axis.x, axis.y, axis.z, Mathf.Cos(rad));
    }
    public static float Dot(_Quaternion a, _Quaternion b)
    {
        return (float)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z +
                        (double)a.w * (double)b.w);
    }
    public static _Quaternion Euler(float x, float y, float z)
    {
        _Quaternion qx = identity;
        _Quaternion qy = identity;
        _Quaternion qz = identity;

        float sinAngle = 0.0f;
        float cosAngle = 0.0f;

        sinAngle = Mathf.Sin(Mathf.Deg2Rad * y * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * y * 0.5f);
        qy.Set(0, sinAngle, 0, cosAngle);

        sinAngle = Mathf.Sin(Mathf.Deg2Rad * x * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * x * 0.5f);
        qx.Set(sinAngle, 0, 0, cosAngle);

        sinAngle = Mathf.Sin(Mathf.Deg2Rad * z * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * z * 0.5f);
        qz.Set(0, 0, sinAngle, cosAngle);

        return qy * qx * qz;
    }
    public static _Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        Vector3 axis = Vector3.Cross(fromDirection, toDirection);
        float angle = Vector3.Angle(fromDirection, toDirection);
        return AngleAxis(angle, axis.normalized);
    }
    public static _Quaternion Inverse(_Quaternion rotation)
    {
        _Quaternion q;
        q.w = rotation.w;
        q.x = -rotation.x;
        q.y = -rotation.y;
        q.z = -rotation.z;
        return q;
    }
    public static _Quaternion Lerp(_Quaternion a, _Quaternion b, float t)
    {
        if (t > 1) t = 1;
        if (t < 0) t = 0;
        return Slerp(a, b, t);
    }
    public static _Quaternion LerpUnclamped(_Quaternion a, _Quaternion b, float t)
    {
        return Slerp(a, b, t);
    }
    public static _Quaternion LookRotation(Vector3 forward, Vector3 upwards)
    {
        forward = Vector3.Normalize(forward);
        Vector3 right = Vector3.Normalize(Vector3.Cross(upwards, forward));
        upwards = Vector3.Cross(forward, right);
        float m00 = right.x;
        float m01 = right.y;
        float m02 = right.z;
        float m10 = upwards.x;
        float m11 = upwards.y;
        float m12 = upwards.z;
        float m20 = forward.x;
        float m21 = forward.y;
        float m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        _Quaternion quaternion = new _Quaternion();
        if (num8 > 0f)
        {
            float num = Mathf.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            float num7 = Mathf.Sqrt(((1f + m00) - m11) - m22);
            float num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            float num6 = Mathf.Sqrt(((1f + m11) - m00) - m22);
            float num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        float num5 = Mathf.Sqrt(((1f + m22) - m00) - m11);
        float num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;

        return quaternion;
    }
    public static _Quaternion Normalize(_Quaternion q)
    {
        float div = Mathf.Sqrt(Dot(q, q));
        q.x /= div;
        q.y /= div;
        q.z /= div;
        q.w /= div;
        return q;
    }
    public static _Quaternion RotateTowards(_Quaternion from, _Quaternion to, float maxDegreesDelta)
    {
        float num = Angle(from, to);
        if (num == 0f)
        {
            return to;
        }
        float t = Math.Min(1f, maxDegreesDelta / num);
        return SlerpUnclamped(from, to, t);
    }
    public static _Quaternion Slerp(_Quaternion a, _Quaternion b, float t)
    {
        if (t > 1) t = 1;
        if (t < 0) t = 0;
        return SlerpUnclamped(a, b, t);
    }
    public static _Quaternion SlerpUnclamped(_Quaternion a, _Quaternion b, float t)
    {
        // if either input is zero, return the other.
        if (a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w == 0.0f)
            return b.x * b.x + b.y * b.y + b.z * b.z + b.w * b.w == 0.0f ? identity : b;
        if (b.x * b.x + b.y * b.y + b.z * b.z + b.w * b.w == 0.0f)
            return a;

        float cosHalfAngle = a.w * b.w + Vector3.Dot(new Vector3(a.x, a.y, a.z), new Vector3(b.x, b.y, b.z));

        if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
        {
            // angle = 0.0f, so just return one input.
            return a;
        }
        if (cosHalfAngle < 0.0f)
        {
            b.x = -b.x;
            b.y = -b.y;
            b.z = -b.z;
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

        _Quaternion result = new _Quaternion(blendA * a.x + blendB * b.x, blendA * a.y + blendB * b.y,
            blendA * a.z + blendB * b.z, blendA * a.w + blendB * b.w);

        return result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w > 0.0f
            ? Normalize(result)
            : identity;
    }
    #endregion

    #region Operators
    public static _Quaternion operator *(_Quaternion lhs, _Quaternion rhs)
    {
        _Quaternion q = identity;
        Vector3 vLhs = new Vector3(lhs.x, lhs.y, lhs.z);
        Vector3 vRhs = new Vector3(rhs.x, rhs.y, rhs.z);
        q.w = lhs.w * rhs.w - Vector3.Dot(vLhs, vRhs);
        Vector3 newV = rhs.w * vLhs + lhs.w * vRhs + Vector3.Cross(vLhs, vRhs);
        q.x = newV.x;
        q.y = newV.y;
        q.z = newV.z;
        return q;
    }
    public static Vector3 operator *(_Quaternion rotation, Vector3 point)
    {
        float num1 = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num1;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num1;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        Vector3 result;
        result.x = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.x + ((double)num7 - (double)num12) * (double)point.y + ((double)num8 + (double)num11) * (double)point.z);
        result.y = (float)(((double)num7 + (double)num12) * (double)point.x + (1.0 - ((double)num4 + (double)num6)) * (double)point.y + ((double)num9 - (double)num10) * (double)point.z);
        result.z = (float)(((double)num8 - (double)num11) * (double)point.x + ((double)num9 + (double)num10) * (double)point.y + (1.0 - ((double)num4 + (double)num5)) * (double)point.z);
        return result;
    }
    public static bool operator ==(_Quaternion lhs, _Quaternion rhs)
    {
        return Dot(lhs, rhs) > 0.999999f;
    }
    public static bool operator !=(_Quaternion lhs, _Quaternion rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    public override string ToString()
    {
        return "X:" + x + " Y:" + y + " Z:" + z + " W:" + w;
    }
    public bool Equals(_Quaternion other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }
    public override bool Equals(object obj)
    {
        return obj is _Quaternion other && Equals(other);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            hashCode = (hashCode * 397) ^ w.GetHashCode();
            return hashCode;
        }
    }
}