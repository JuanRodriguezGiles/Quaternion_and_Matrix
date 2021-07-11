using System;
using UnityEngine;
public struct _Matrix4x4 : IEquatable<_Matrix4x4>
{
    public float m00;
    public float m10;
    public float m20;
    public float m30;
    public float m01;
    public float m11;
    public float m21;
    public float m31;
    public float m02;
    public float m12;
    public float m22;
    public float m32;
    public float m03;
    public float m13;
    public float m23;
    public float m33;

    public static _Matrix4x4 zero = new _Matrix4x4(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
    public static Matrix4x4 identity = new Matrix4x4(new Vector4(1f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 1f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 1f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1f));

    public _Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
    {
        this.m00 = column0.x;
        this.m01 = column1.x;
        this.m02 = column2.x;
        this.m03 = column3.x;
        this.m10 = column0.y;
        this.m11 = column1.y;
        this.m12 = column2.y;
        this.m13 = column3.y;
        this.m20 = column0.z;
        this.m21 = column1.z;
        this.m22 = column2.z;
        this.m23 = column3.z;
        this.m30 = column0.w;
        this.m31 = column1.w;
        this.m32 = column2.w;
        this.m33 = column3.w;
    }
    public static _Matrix4x4 Translate(Vector3 vector)
    {
        Vector4 column0 = new Vector4(1, 0, 0, 0);
        Vector4 column1 = new Vector4(0, 1, 0, 0);
        Vector4 column2 = new Vector4(0, 0, 1, 0);
        Vector4 column3 = new Vector4(vector.x, vector.y, vector.z, 1);
        return new _Matrix4x4(column0, column1, column2, column3);
    }
    public static _Matrix4x4 Scale(Vector3 vector)
    {
        Vector4 column0 = new Vector4(vector.x, 0, 0, 0);
        Vector4 column1 = new Vector4(0, vector.y, 0, 0);
        Vector4 column2 = new Vector4(0, 0, vector.z, 0);
        Vector4 column3 = new Vector4(0, 0, 0, 1);
        return new _Matrix4x4(column0, column1, column2, column3);
    }
    public static _Matrix4x4 Rotate(Quaternion q)
    {
        Vector3 angles = q.eulerAngles;
        angles *= Mathf.PI / 180;

        Vector4 column0 = new Vector4(1, 0, 0, 0);
        Vector4 column1 = new Vector4(0, Mathf.Cos(-angles.x), -Mathf.Sin(-angles.x), 0);
        Vector4 column2 = new Vector4(0, Mathf.Sin(-angles.x), Mathf.Cos(-angles.x), 0);
        Vector4 column3 = new Vector4(0, 0, 0, 1);
        _Matrix4x4 xMatrix = new _Matrix4x4(column0, column1, column2, column3);

        column0 = new Vector4(Mathf.Cos(-angles.y), 0, Mathf.Sin(-angles.y), 0);
        column1 = new Vector4(0, 1, 0, 0);
        column2 = new Vector4(-Mathf.Sin(-angles.y), 0, Mathf.Cos(-angles.y), 0);
        column3 = new Vector4(0, 0, 0, 1);
        _Matrix4x4 yMatrix = new _Matrix4x4(column0, column1, column2, column3);

        column0 = new Vector4(Mathf.Cos(-angles.z), -Mathf.Sin(-angles.z), 0, 0);
        column1 = new Vector4(Mathf.Sin(-angles.z), Mathf.Cos(-angles.z), 0, 0);
        column2 = new Vector4(0, 0, 1, 0);
        column3 = new Vector4(0, 0, 0, 1);
        _Matrix4x4 zMatrix = new _Matrix4x4(column0, column1, column2, column3);

        return xMatrix * yMatrix * zMatrix;
    }
    public static _Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
    {
        _Matrix4x4 T = Translate(pos);
        _Matrix4x4 R = Rotate(q);
        _Matrix4x4 S = Scale(s);

        return T * S * R;
    }
    public static _Matrix4x4 operator *(_Matrix4x4 lhs, _Matrix4x4 rhs)
    {
        _Matrix4x4 m;
        m.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
        m.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
        m.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
        m.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
        m.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
        m.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
        m.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
        m.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
        m.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
        m.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
        m.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
        m.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
        m.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
        m.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
        m.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
        m.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
        return m;
    }
    public static Vector4 operator *(_Matrix4x4 lhs, Vector4 vector)
    {
        Vector4 res;
        res.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
        res.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
        res.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
        res.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
        return res;
    }
    public Vector4 GetColumn(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector4(this.m00, this.m10, this.m20, this.m30);
            case 1:
                return new Vector4(this.m01, this.m11, this.m21, this.m31);
            case 2:
                return new Vector4(this.m02, this.m12, this.m22, this.m32);
            case 3:
                return new Vector4(this.m03, this.m13, this.m23, this.m33);
            default:
                return Vector4.zero;
        }
    }
    public Vector4 GetRow(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector4(this.m00, this.m01, this.m02, this.m03);
            case 1:
                return new Vector4(this.m10, this.m11, this.m12, this.m13);
            case 2:
                return new Vector4(this.m20, this.m21, this.m22, this.m23);
            case 3:
                return new Vector4(this.m30, this.m31, this.m32, this.m33);
            default:
                return Vector4.zero;
        }
    }
    public static bool operator ==(_Matrix4x4 lhs, _Matrix4x4 rhs) => lhs.GetColumn(0) == rhs.GetColumn(0) &&
                                                                      lhs.GetColumn(1) == rhs.GetColumn(1) &&
                                                                      lhs.GetColumn(2) == rhs.GetColumn(2) &&
                                                                      lhs.GetColumn(3) == rhs.GetColumn(3);
    public static bool operator !=(_Matrix4x4 lhs, _Matrix4x4 rhs) => !(lhs == rhs);
    public bool Equals(_Matrix4x4 other)
    {
        throw new NotImplementedException();
    }
    public override bool Equals(object obj)
    {
        return obj is _Matrix4x4 other && Equals(other);
    }
    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}