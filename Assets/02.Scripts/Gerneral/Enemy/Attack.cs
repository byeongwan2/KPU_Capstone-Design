using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Attack : MonoBehaviour
{
    NavMeshAgent agent;
    Wound target;
    private int attackDamage = 10;
    float rotateDegree;
    public void Init_Target(MoveObject _target)
    {
        target = _target.GetComponent<Wound>();
    }
    public void Setting(NavMeshAgent _agent,int _attackDamage)
    {
        agent = _agent;
        attackDamage = _attackDamage;
    }

    public void Init()
    {
        agent.isStopped = true;
        rotateDegree = Mathf.Atan2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z) * Mathf.Rad2Deg;
    }
    
    public void Work()      //계속호출  //임시
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 1.0f);
    }

    public bool Condition()
    {

        if ( Mathf.Abs(f-d) < 0.1f)               //목표로 한 방향에서 현재방향에 많이가까워졌을떄의 조건을 걸면됨
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    float f= 10.0f;
    float d = 5.0f;
    public void Work_Dir()
    {
        agent.isStopped = true;
        f = Quaternion.Euler(0.0f, rotateDegree, 0.0f).y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 5.0f);
        d = transform.rotation.y;
   
    }

    public void Send_Damage()
    {
        target.GetDamage(attackDamage);
    }
}
