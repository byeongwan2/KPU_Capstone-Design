using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour {

    public List<Transform> wayPoints;

    private NavMeshAgent agent;

    private float m_patrolSpeed;
    private float m_traceSpeed;

    [SerializeField]
    private bool patrolling =true ;
    public bool pPatrolling
    {
        get { return patrolling; }
        set
        {
            patrolling = value;
            if (patrolling)
            {

                agent.speed = m_patrolSpeed;
                MoveWayPoint(Random.Range(0, wayPoints.Count));
            }
            
        }
    }
    private Vector3 traceTarget;
    public Vector3 pTraceTarget
    {
        get { return traceTarget; }
        set
        {
            traceTarget = value;
            agent.speed = m_traceSpeed;
            TraceTarget(traceTarget);
        }
    }

    public void Init(string _str,float _patrolSpeed, float _traceSpeed)
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = _patrolSpeed;
        var group = GameObject.Find(_str);
        group.GetComponentsInChildren<Transform>(wayPoints);
        wayPoints.RemoveAt(0);

        m_patrolSpeed = _patrolSpeed;
        m_traceSpeed = _traceSpeed;
        MoveWayPoint(0);
        this.pPatrolling = true;
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()              
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        patrolling = false;
    }

    void MoveWayPoint(int _nextIdx)
    {
        if (agent.isPathStale) return;
        agent.destination = wayPoints[_nextIdx].position;
        agent.isStopped = false;
    }

    void Update()
    {
        if (patrolling == false) return;
        if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <=0.5f)
        {
            int nextIdx =  Random.Range(0,wayPoints.Count);
            MoveWayPoint(nextIdx);
        }
    }

    public Vector3 temp_Debug_Dest()        //디버깅용 임시 함수 나중에 삭제
    {

        return agent.destination;
    }
}
