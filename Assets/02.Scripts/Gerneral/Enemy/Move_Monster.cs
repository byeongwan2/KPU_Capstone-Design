using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Move_Monster : MonoBehaviour
{
    protected NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }

    
}
