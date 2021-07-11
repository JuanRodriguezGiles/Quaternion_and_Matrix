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

            //Computar X -roll- en base a formula de relacion (Atan2(2(q0q1 + q2q3),(1-2*(q1^2 + q2^2))
            double roll = toDegrees * Math.Atan2(2 * (w * x + y * z), 1 - 2 * (x * x + y * y));

            //Computar Y -pitch- en base a formula de relacion (Asin(2(q0q2-q3q1))
            double sinp = 2 * (w * y - z * x);
            double pitch = toDegrees * Math.Asin(sinp);
            //Checkear gimbal lock cuando Y se acerca a +-90 grados 
            if (Math.Abs(sinp) >= 1)
                pitch = Math.PI / 2 * Math.Sign(sinp);
            //Computar Z -yaw- en base a formula de relacion (Atan2(2(q0q3 +q1q2),1-2(q2^2+q3^2)
            double yaw = toDegrees * Math.Atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z));

            return new Vector3((float)roll, (float)pitch, (float)yaw);
        }
        set
        {
        //Combinar la representacion en quaternion de las rotaciones en angulos Eulers se obtiene una secuencia de Z -yaw-, Y -pitch-,
        //X -roll-
        //https://wikimedia.org/api/rest_v1/media/math/render/svg/d081a6bf413a578916923e963434fba9d8a162a7

        //Computo funciones angulares
            float cy = Mathf.Cos(value.z * 0.5f);
            float sy = Mathf.Sin(value.z * 0.5f);
            float cp = Mathf.Cos(value.y * 0.5f);
            float sp = Mathf.Sin(value.y * 0.5f);
            float cr = Mathf.Cos(value.x * 0.5f);
            float sr = Mathf.Sin(value.x * 0.5f);

            _Quaternion q;
        //Obtengo cada compononete 
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
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                case 3:
                    return w;
                default:
                    return 0;
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
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
        //Se define un eje y un angulo que determina cuanto se va a rotar alrededor del eje

        //Normalizar en caso de que ya no este normalizado
        Normalize(this);
        //Computo angulo en base a la W del quaternion
        angle = 2.0f * (float)Math.Acos(w);
        //Computo numero denormalizado
        float den = (float) Math.Sqrt(1.0 - w * w);
        if (den > 0.0001f)
            axis = new Vector3(x, y, z) / den;
        else
            axis = new Vector3(1, 0, 0); //Cuando el angulo da 0

        //Conversion a grados
        angle *= (float)(180 / Math.PI);
    }
    #endregion

    #region STATIC METHODS
    public static float Angle(_Quaternion a, _Quaternion b)
    {
        //Devuelve la diferencia entre dos rotaciones en angulos
        //Obtener producto punto
        float num = Dot(a, b);
        //Retorna el numero combertido a grados o 1 si la diferencia es muy chica (57 == 180/PI)
        return (float)((double)Mathf.Acos(Mathf.Min(Mathf.Abs(num), 1f)) * 2.0 * 57.2957801818848);
    }
    public static _Quaternion AngleAxis(float angle, Vector3 axis)
    {
        //Crea una rotacion que rota X cantidad de angulos alrededor del eje
        //Normalizo el eje
        axis.Normalize();
        //convierto el angulo a radianes
        float rad = angle * Mathf.Deg2Rad * 0.5f;
        //computo el seno
        axis *= Mathf.Sin(rad);
        //Devuelvo nuevo quaternion con el eje de rotacion y el nuevo escalar en base al angulo
        return new _Quaternion(axis.x, axis.y, axis.z, Mathf.Cos(rad));
    }
    public static float Dot(_Quaternion a, _Quaternion b)
    {
        return (float)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z +
                        (double)a.w * (double)b.w);
    }
    public static _Quaternion Euler(float x, float y, float z)
    {
        //Devuelvo en quaternion que rota z grados en el eje Z, x grados en el eje X e y grados en el eje Y.
        _Quaternion qx = identity;
        _Quaternion qy = identity;
        _Quaternion qz = identity;

        float sinAngle = 0.0f;
        float cosAngle = 0.0f;

        //Realizo rotacion del eje y con conversion a radianes y computo la W del mismo
        sinAngle = Mathf.Sin(Mathf.Deg2Rad * y * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * y * 0.5f);
        qy.Set(0, sinAngle, 0, cosAngle);

        //Realizo rotacion del eje x con conversion a radianes y computo la W del mismo
        sinAngle = Mathf.Sin(Mathf.Deg2Rad * x * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * x * 0.5f);
        qx.Set(sinAngle, 0, 0, cosAngle);

        //Realizo rotacion del eje z con conversion a radianes y computo la W del mismo
        sinAngle = Mathf.Sin(Mathf.Deg2Rad * z * 0.5f);
        cosAngle = Mathf.Cos(Mathf.Deg2Rad * z * 0.5f);
        qz.Set(0, 0, sinAngle, cosAngle);

        return qy * qx * qz;
    }
    public static _Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        //Crea una rotacion que rota desde from hasta to
        //Modifica la direccion de un vector para que luego de haber rotado este este mirando la direccion proporcionada

        //Computo el eje de rotacion con producto cruz
        Vector3 axis = Vector3.Cross(fromDirection, toDirection);
        //Obtengo el angulo de rotacion
        float angle = Vector3.Angle(fromDirection, toDirection);
        //Creo la rotacion
        return AngleAxis(angle, axis.normalized);
    }
    public static _Quaternion Inverse(_Quaternion rotation)
    {
        // Separar los componentes del quaternion
        Vector3 vector = new Vector3(
            rotation.x, rotation.y, rotation.z);
        float scalar = rotation.w;

        // Invertir xyz
        vector = -vector;

      
        _Quaternion inverseRotation = new _Quaternion(vector.x, vector.y, vector.z, scalar);
        return inverseRotation;
    }
    public static _Quaternion Lerp(_Quaternion a, _Quaternion b, float t)
    {
        //Interpolacion linear, sigue una linea recta entre rotaciones para asegurar una velocidad angular constante

        //Clamp entre 0 y 1
        if (t > 1) t = 1;
        if (t < 0) t = 0;
        // Separar compononetes de los quaterniones
        Vector3 aVector = new Vector3(a.x, a.y, a.z);
        float aScalar = a.w;
        Vector3 bVector = new Vector3(b.x, b.y, b.z);
        float bScalar = b.w;

        // Calculo los valores del quaternion final/objetivo
        Vector3 targetVector = (1 - t) * aVector + t * bVector;
        float targetScalar = (1 - t) * aScalar + t * bScalar;

        // Normalizar resultados
        float factor = Mathf.Sqrt(targetVector.sqrMagnitude + targetScalar * targetScalar);
        targetVector /= factor;
        targetScalar /= factor;

        // Retornar quaternion con lerp realizado
        return new _Quaternion(targetVector.x, targetVector.y, targetVector.z, targetScalar);
    }
    public static _Quaternion LerpUnclamped(_Quaternion a, _Quaternion b, float t)
    {
        // Separar compononetes de los quaterniones
        Vector3 aVector = new Vector3(a.x, a.y, a.z);
        float aScalar = a.w;
        Vector3 bVector = new Vector3(b.x, b.y, b.z);
        float bScalar = b.w;

        // Calculo los valores del quaternion final/objetivo
        Vector3 targetVector = (1 - t) * aVector + t * bVector;
        float targetScalar = (1 - t) * aScalar + t * bScalar;

        // Normalizar resultados
        float factor = Mathf.Sqrt(targetVector.sqrMagnitude + targetScalar * targetScalar);
        targetVector /= factor;
        targetScalar /= factor;

        // Retornar quaternion con lerp realizado
        return new _Quaternion(targetVector.x, targetVector.y, targetVector.z, targetScalar);
    }
    public static _Quaternion LookRotation(Vector3 forward, Vector3 upwards)
    {
        //Crea una rotacion que orienta un objeto de manera que so forward encare el forward de la direccion
        //objetivo. El up del objeto va a tener el mismo up que su objetivo.

        //Normalizar forard
        forward = Vector3.Normalize(forward);
        //Saco producto cruz que va a represntar mi rotacion
        Vector3 right = Vector3.Normalize(Vector3.Cross(upwards, forward));
        //Computo mi nuevo up
        upwards = Vector3.Cross(forward, right);
        //Defino componentes de matriz 3x3
        float m00 = right.x;
        float m01 = right.y;
        float m02 = right.z;
        float m10 = upwards.x;
        float m11 = upwards.y;
        float m12 = upwards.z;
        float m20 = forward.x;
        float m21 = forward.y;
        float m22 = forward.z;

        //Computo num8 en base a diagonal de la matriz
        float num8 = (m00 + m11) + m22;
        _Quaternion quaternion = new _Quaternion();
        //Si es mayor a 0 el escalar es en base a num8
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
        //Si el 00 de la matriz es mayor al 11 y 22
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
        //Saco factor de normalize
        float div = Mathf.Sqrt(Dot(q, q));
        q.x /= div;
        q.y /= div;
        q.z /= div;
        q.w /= div;
        return q;
    }
    public static _Quaternion RotateTowards(_Quaternion from, _Quaternion to, float maxDegreesDelta)
    {
        //El quaternion from se rota hacia to en pasos de maxDegreesDelta
        
        //computo diferencia de angulos y si es 0 retorno to
        float num = Angle(from, to);
        if (num == 0f)
        {
            return to;
        }
        //Seteo la t en base a maxDegreesDelta
        float t = Math.Min(1f, maxDegreesDelta / num);
        return SlerpUnclamped(from, to, t);
    }
    public static _Quaternion Slerp(_Quaternion a, _Quaternion b, float t)
    {
        //Clampeo t
        if (t > 1) t = 1;
        if (t < 0) t = 0;
        return SlerpUnclamped(a, b, t);
    }
    public static _Quaternion SlerpUnclamped(_Quaternion a, _Quaternion b, float t)
    {
        // Si alguno de los 2 es cero retornar el otro
        if (a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w == 0.0f)
            return b.x * b.x + b.y * b.y + b.z * b.z + b.w * b.w == 0.0f ? identity : b;
        if (b.x * b.x + b.y * b.y + b.z * b.z + b.w * b.w == 0.0f)
            return a;

        //Utilizo formula del medio angulo para evaluar funcion
        float cosHalfAngle = a.w * b.w + Vector3.Dot(new Vector3(a.x, a.y, a.z), new Vector3(b.x, b.y, b.z));

        if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
        {
            //fuera de rango asi que retorno uno de los dos quaterniones
            return a;
        }
        if (cosHalfAngle < 0.0f)
        {
            //Si es negativo invierto los valores
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
            //Realizo slerp par angulos grandes
            //Computo medio angulo
            float halfAngle = (float)Math.Acos(cosHalfAngle);
            //computo el seno
            float sinHalfAngle = (float)Math.Sin(halfAngle);
            float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
            //computo ambos "blends" 
            blendA = (float)Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
            blendB = (float)Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
        }
        else
        {
            // Si el angulo es muy chico hago lerp normal
            blendA = 1.0f - t;
            blendB = t;
        }

        //Creo quaternion en base a los dos "blends"
        _Quaternion result = new _Quaternion(blendA * a.x + blendB * b.x, blendA * a.y + blendB * b.y,
            blendA * a.z + blendB * b.z, blendA * a.w + blendB * b.w);

        //Si es menor que 0 retorno identidad, sino lo retorno normalizado
        return result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w > 0.0f
            ? Normalize(result)
            : identity;
    }
    #endregion

    #region Operators
    public static _Quaternion operator *(_Quaternion lhs, _Quaternion rhs)
    {
        // Separo componentes quaternion
        Vector3 parentVector = new Vector3(
            lhs.x, lhs.y, lhs.z);
        float parentScalar = lhs.w;

        Vector3 childVector = new Vector3(
            rhs.x, rhs.y, rhs.z);
        float childScalar = rhs.w;

        // Realizo formula de multplicacion
        Vector3 targetVector = parentScalar * childVector
                               + childScalar * parentVector
                               + Vector3.Cross(parentVector, childVector);
        float targetScalar = parentScalar * childScalar
                             - Vector3.Dot(parentVector, childVector);

        // Creo nuevo quaternion
        _Quaternion targetRotation = new _Quaternion(
            targetVector.x, targetVector.y, targetVector.z, targetScalar);

        return targetRotation;
    }
    public static Vector3 operator *(_Quaternion rotation, Vector3 point)
    {
        // Creo quaternion en base al vector dado
        _Quaternion positionQuaternion = new _Quaternion(
            point.x, point.y, point.z, 0);
        //Invierto el quaternion dado
        _Quaternion inverseRotation = Inverse(rotation);

        // Calculo la nueva posicion
        _Quaternion newPositionQuat
            = rotation * positionQuaternion * inverseRotation;

        // Guardo resultado
        Vector3 newPosition = new Vector3(
            newPositionQuat.x, newPositionQuat.y, newPositionQuat.z);

        return newPosition;
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