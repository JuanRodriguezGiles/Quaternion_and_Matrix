using UnityEngine;
public class _QuaternionTester : MonoBehaviour
{
    public Transform t;
    public _Quaternion myQuaternion1 = new _Quaternion(0.53f, 0.20f, 0.19f, 0.79f);
    public _Quaternion myQuaternion2 = new _Quaternion(5, 6, 7, 8);
    private Quaternion notMyQuaternion1 = new Quaternion(0.53f, 0.20f, 0.19f, 0.79f);
    private Quaternion notMyQuaternion2 = new Quaternion(5, 6, 7, 8);
    private Matrix4x4 meme;
    void Start()
    {
       
    }

    void Update()
    {
        //myQuaternion1 = new _Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z,
        //    transform.rotation.w);
        //myQuaternion2 = new _Quaternion(t.rotation.x, t.rotation.y, t.rotation.z,
        //    t.rotation.w);

        //notMyQuaternion1 = transform.rotation;
        //notMyQuaternion2 = t.rotation;

        //Debug.Log(myQuaternion1.eulerAngles);
        //Debug.Log(notMyQuaternion1.eulerAngles);

        Vector3 relativePos = t.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
}