using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour {

    public List<Transform> wayPoints;
    public int nextIdx;

    private NavMeshAgent agent;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    private bool patrolling =true ;
    public bool pPatrolling
    {
        get { return patrolling; }
        set
        {
            patrolling = value;
            if (patrolling)
            {
                agent.speed = patrolSpeed;
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
            agent.speed = traceSpeed;
            TraceTarget(traceTarget);
        }
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
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = patrolSpeed;
        var group = GameObject.Find("WayPointGroup");
        group.GetComponentsInChildren<Transform>(wayPoints);
        wayPoints.RemoveAt(0);

        MoveWayPoint();
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

    public void StopNavi() {agent.isStopped = true;}        //네비를 끄는거와 정찰을 끄는건다름
    public void StartNavi() { agent.isStopped = false; }
}
