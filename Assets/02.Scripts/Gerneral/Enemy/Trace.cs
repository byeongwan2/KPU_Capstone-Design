using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Trace : Move_Monster
{
    MoveObject target;
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }


    public bool Work()
    {
        Debug.Log("bbb");
        agent.destination = target.transform.position;
        return true;
    }

    public bool Condition(float _dis)
    {
        if (_dis > Check.Distance(target.transform.position,this.transform.position))
        {
            if (_dis == 3.0f) Debug.Log("cc");
            return true;

        }
        if (_dis == 3.0f) Debug.Log("tt");
        return false;
    }


    

}
