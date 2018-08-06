using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check  {

	public static float Distance(Transform _a, Transform _b)
    {
        return Vector3.Distance(_a.position, _b.position);
    }

    public static float Distance(GameObject _a,GameObject _b)       //그냥심심해서만든 오버로딩 ㅋㅋ
    {
        return Vector3.Distance(_a.transform.position, _b.transform.position);
    }
   
    public static void AllFreeze(Rigidbody _rb)
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll ;
    }

    public static void ResetFreeze(Rigidbody _rb)
    {
        _rb.constraints = RigidbodyConstraints.None;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
