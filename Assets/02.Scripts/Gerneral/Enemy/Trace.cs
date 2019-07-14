using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//적군만 가지고 있는 컴포넌트
public class Trace : MonoBehaviour
{
    NavMeshAgent agent;
    MoveObject target;
    float speed;
    
    //적군의 목표를 설정    보통은 플레이어가 타겟
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }

    //게임실행할떄 한번만 호출     
    public void Setting(NavMeshAgent _agent, float _speed)              
    {
        agent = _agent;
        agent.speed = _speed;
        speed = _speed;
    }

    //상태가 바뀔때마다 호출되는함수
    public void Init()             
    {
        agent.speed = speed;
    }

    public void Init(float _speed)
    {
        agent.speed = _speed;
    }

    //실제 추적을 실행하는 함수     네비게이션 에이전트 컴포넌트 포함
    public void Work()
    {
        agent.destination = target.transform.position;
    }

    //타겟과의 거리를 재고 추적할지 말지 결정
    public bool Condition(float _dis)
    {
        if (_dis > Check.Distance(target.transform.position,this.transform.position))
        {
            return true;

        }
        return false;
    }


    

}
