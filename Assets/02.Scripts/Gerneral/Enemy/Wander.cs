using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//방황하는 스크립트 플레이어를 발견하기전에는 돌아댕긴다
public class Wander : MonoBehaviour
{
    int pointIndex = 0;
    public List<Transform> wayPoints;
    NavMeshAgent agent;
    [SerializeField]
    bool check_Destination = false;
    float speed;

    public void Setting(NavMeshAgent _agent, float _speed)
    {
        agent = _agent;
        agent.speed = _speed;
        speed = _speed;
        agent.destination = wayPoints[pointIndex].position;
        check_Destination = false;
    }

    public void Init()
    {
        agent.destination = wayPoints[pointIndex].position;
        agent.speed = speed;
        check_Destination = false;
    }

    public void Work()
    {
        if (check_Destination) return;
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f )
        {
            pointIndex = pointIndex == 0 ? 1 : 0;
            agent.destination = wayPoints[pointIndex].position;
            check_Destination = true;
            Change_After_Time_Bool(2.0f);
            return;
        }

    }
    public void Change_After_Time_Bool(float _time)
    {
        StartCoroutine(Reset_Bool(_time));
       
    }

    IEnumerator Reset_Bool(float _time)
    {
        yield return new WaitForSeconds(_time);
        check_Destination = false;
    }
}
