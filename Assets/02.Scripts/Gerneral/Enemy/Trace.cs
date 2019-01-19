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
        //플레이어를 따라가는 코드를 작성
    }

    public bool Condition()
    {
        if (2.0f >Check.Distance(target.transform.position,this.transform.position))
        {
            Debug.Log("프레이어가 가까이 왔다");
            return true;

        }
        Debug.Log("플레이어 나랑멀다");
        return false;
    }


    

}
