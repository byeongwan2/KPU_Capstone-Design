using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Attack : MonoBehaviour
{
    NavMeshAgent agent;
    MoveObject target;
    private int attackDamage = 10;
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }
    public void Init(NavMeshAgent _agent,int _attackDamage)
    {
        agent = _agent;
        attackDamage = _attackDamage;
    }
    
    public bool Work()
    {
        agent.isStopped = true;
        float rotateDegree = Mathf.Atan2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
        return true;
    }
}
