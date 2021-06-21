using UnityEngine;
public class _QuaternionTester : MonoBehaviour
{
    private _Quaternion myQuaternion1 = new _Quaternion(1, 2, 3, 4);
    private _Quaternion myQuaternion2 = new _Quaternion(5, 6, 7, 8);
    private Quaternion notMyQuaternion1 = new Quaternion(1, 2, 3, 4);
    private Quaternion notMyQuaternion2 = new Quaternion(5, 6, 7, 8);
    void Start()
    {
        Debug.Log(Quaternion.FromToRotation(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
        Debug.Log(_Quaternion.FromToRotation(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
    }
}