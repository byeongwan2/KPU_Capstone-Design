using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour {

    public List<Transform> wayPoints;
    public int nextIdx = 0;

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
                MoveWayPoint();
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

    public void DataInput(float _patrolSpeed,float _traceSpeed)
    {
        m_patrolSpeed = _patrolSpeed;
        m_traceSpeed = _traceSpeed;
    }

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = m_patrolSpeed;
        var group = GameObject.Find("WayPointGroup");
        group.GetComponentsInChildren<Transform>(wayPoints);
        wayPoints.RemoveAt(0);

        MoveWayPoint();
        StartNavi();
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

    void Start()
    {
        
    }

    void MoveWayPoint()
    {
        if (agent.isPathStale) return;
        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false;
    }

    void Update()
    {
        if (patrolling == false) return;
        if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <=0.5f)
        {
            nextIdx = ++nextIdx % wayPoints.Count;
            MoveWayPoint();
        }
    }

    public void StopNavi() {agent.isStopped = true;}        //네비를 끄는거와 정찰을 끄는건다름        //잠깐 네비종료
    public void StartNavi() { agent.isStopped = false; }
}
