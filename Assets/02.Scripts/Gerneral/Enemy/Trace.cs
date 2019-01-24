using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Trace : MonoBehaviour
{
    NavMeshAgent agent;
    MoveObject target;
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }
    public void Init(NavMeshAgent _agent,float _speed)
    {
        agent = _agent;
        agent.speed = _speed;
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
            if (_dis == 6.0f) 
            Debug.Log("sss");
            return true;

        }
        agent.isStopped = false;
        return false;
    }


    

}
