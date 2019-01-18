using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{
    MoveObject target;

    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }

    public void Condition()
    {
        if (8.0f >Check.Distance(target.transform.position,this.transform.position))
        {
            Debug.Log("프레이어가 가까이 왔다");
        }
    }

    public void Work()
    {
        Condition();
    }

}
