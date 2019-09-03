using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//근접공격을 하는 적군이 사용하는 컴포넌트
public class Attack : Behaviour
{
    NavMeshAgent agent;
    Wound target;
    private int attackDamage = 10;
    float rotateDegree;

    bool isReady = false;
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
        isReady = true;
        agent.isStopped = true;
        rotateDegree = Mathf.Atan2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z) * Mathf.Rad2Deg;
    }
    
    public void Work()      //계속호출  //임시
    {
        isReady = true;
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

    //방향에맞춰서 공격모션을 취함
    float f= 10.0f;
    float d = 5.0f;
    public void Work_Dir()
    {
        agent.isStopped = true;
        f = Quaternion.Euler(0.0f, rotateDegree, 0.0f).y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 5.0f);
        d = transform.rotation.y;
   
    }

    //플레이어에게 데미지를 줌
    public void Send_Damage()
    {
        target.GetDamage(attackDamage);
    }
    float timeRotate = 0.0f;
    public void FixedUpdate()
    {
        if (isReady == false) return;
        timeRotate += Time.deltaTime;
        if (timeRotate > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 4.0f);
        }
       
    }

    public override void End()
    {
        timeRotate = 0.0f;
        isReady = false;
    }
}
