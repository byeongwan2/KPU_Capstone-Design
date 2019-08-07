using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//경계하다
public class Alert : MonoBehaviour
{
    Wound target;
    bool isActive = false;
    void Start()
    {

    }

    //상태가 바뀔때마다 호출되는함수
    public void Init()
    {

    }

    public void Init_Target(MoveObject _target)
    {
        target = _target.GetComponent<Wound>();
    }

    void FixedUpdate()
    {
        if (isActive == false) return;
        transform.LookAt(PrefabSystem.instance.GetCorrectPlayerTarget());
        float t = Time.realtimeSinceStartup * 0.3f;

        Vector3 vec = Vector3.zero;
        vec.y = transform.position.y;
        vec.x = transform.position.x + 0.02f * Mathf.Cos(t);
        vec.z = transform.position.z + 0.02f * Mathf.Sin(t);

        transform.position = vec;

    }
    public void Work()
    {
        isActive = true;
    }

    public void End_Work()
    {
        isActive = false;
    }
}
