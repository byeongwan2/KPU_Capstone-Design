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
    public static float Distance(Vector3 _a, Vector3 _b)
    {
        return Vector3.Distance(_a, _b);
    }

    public static void AllFreeze(Rigidbody _rb)
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll ;
    }

    public static void ResetFreeze()
    {
       
    }

    public static float Clamp(float _data, float _max)
    {
        if (_data >= _max) _data = _max;
        return _data;
    }

 

    public static Vector3 Insert_Position_XZ(Vector3 _vec,float _y)
    {
        _vec.y = _y;
        return _vec;
    }
}

static class Define         //c# 은 #define 없다는군;;
{
    public const int MOUSE_LEFT_BUTTON = 0;
    public const int MOUSE_RIGHT_BUTTON = 1;
    public const float ADVANCE_BULLET_LIFE_TIME = 0.8f;
}
