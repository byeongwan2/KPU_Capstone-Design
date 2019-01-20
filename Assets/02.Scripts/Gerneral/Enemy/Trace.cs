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


    public RESULT Work()
    {
        agent.destination = target.transform.position;
        return RESULT.SUCCESS;
    }

    public bool Condition(float _dis)
    {
        if (_dis > Check.Distance(target.transform.position,this.transform.position))
        {
            if(_dis == 3.0f)Debug.Log("bbbbqq");
            return true;

        }
        return false;
    }


    

}
