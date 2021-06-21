using System;
using UnityEngine;
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
    //eulerAngles
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
    //SetFromRotation
    //SetLookRotation
    //ToAngleAxis
    #endregion

    #region STATIC METHODS
    //Angle
    public static _Quaternion AngleAxis(float angle, Vector3 axis) //ok
    {
        axis.Normalize();
        float rad = angle * Mathf.Deg2Rad * 0.5f;
        axis *= Mathf.Sin(rad);
        return new _Quaternion(axis.x, axis.y, axis.z, Mathf.Cos(rad));
    }
    public static float Dot(_Quaternion a, _Quaternion b)
    {
        return a.x * b.x + a.y * b.y + a.z + b.z + a.w + b.w;
    }
    //Euler
    public static _Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection) //not ok
    {
        Vector3 axis = Vector3.Cross(fromDirection, toDirection);
        float angle = Vector3.Angle(fromDirection, toDirection);
        return _Quaternion.AngleAxis(angle, axis.normalized);
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
    //lerp
    //lerpuncplaped
    //lookrotation
    public static _Quaternion Normalize(_Quaternion q)
    {
        float div = Mathf.Sqrt(Dot(q, q));
        q.x /= div;
        q.y /= div;
        q.z /= div;
        q.w /= div;
        return q;
    }
    //rotate torwards
    //slerp
    //slerpuncaplmed
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
    //op ==
    #endregion

    public override string ToString()
    {
        return "x: " + x + " y: " + y + " z: " + z + " w: " + w;
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