using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Wander : MonoBehaviour
{
    int pointIndex = 0;
    public List<Transform> wayPoints;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }
    public void Init(float _speed)
    {
        agent.speed = _speed;
    }


    public void Work()
    {
        agent.destination = wayPoints[pointIndex].position;
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            pointIndex = pointIndex == 0 ? 1 : 0;
        }
    }
    
}
