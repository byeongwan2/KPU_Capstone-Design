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

    public void Work()
    {
        Condition();
    }

    public void Condition()
    {
        if (1.0f >Check.Distance(target.transform.position,this.transform.position))
        {
            Debug.Log("프레이어가 가까이 왔다");
        }
    }

    

}
