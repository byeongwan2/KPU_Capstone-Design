using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Trace : MonoBehaviour
{
    NavMeshAgent agent;
    MoveObject target;
    float speed;
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }
    public void Setting(NavMeshAgent _agent, float _speed)              //게임실행할떄 한번만 호출 다른스크립트 Start의존
    {
        agent = _agent;
        agent.speed = _speed;
        speed = _speed;
    }

    public void Init()              //상태가 바뀔때마다 1번만 호출되는함수
    {
        agent.speed = speed;
    }

    public void Init(float _speed)
    {
        agent.speed = _speed;
    }


    public void Work()
    {
        agent.destination = target.transform.position;
    }

    public bool Condition(float _dis)
    {
        if (_dis > Check.Distance(target.transform.position,this.transform.position))
        {
            return true;

        }
        return false;
    }


    

}
