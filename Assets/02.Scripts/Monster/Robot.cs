using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Robot : Enemy {
    enum UP_BODY_STATE { THROW,SHOT}
    UP_BODY_STATE eMotionState;
    BehaviorTree bt;

    Attack attack;

    Trace trace;
    float traceCoverage = 6.0f;
    bool isOther_State_Change = false;
    [SerializeField]
    string activing_Func = string.Empty;

    Rigidbody rb;
    Slider healthSlider;
    public int damage = 3;

    public GameObject hitEffect;
    public Transform hitPos;
    ChangeShader cs;

    void Start()
    {
        base.Init();

        // 자식 오브젝트의 ChangeMaterial 컴포넌트를 가지고 온다
        cs = GetComponentInChildren<ChangeShader>();

        rb = GetComponent<Rigidbody>();
        attack = GetComponent<Attack>();
        attack.Init_Target(system.pPlayer2);
        attack.Setting(agent, 5);



        trace = GetComponent<Trace>();
        trace.Init_Target(system.pPlayer2);
        trace.Setting(agent, 1.0f);
        traceCoverage = 6.0f;
        eEnemy_State = ENEMY_STATE.WALK;
        animator.SetTrigger("isWalk");
        Build_BT();

        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = 3;
        healthSlider.value = 3;
        vitality = 3;   // 체력
    }

    void Update()
    {
        if (eEnemy_State == ENEMY_STATE.DIE)
        {
            isOther_State_Change = true;
            agent.isStopped = true;
            return;
        }
        if (isOther_State_Change == false)
        {
            bt.Run();
        }
        if (eEnemy_State == ENEMY_STATE.ATTACK)      //임시방편
        {
            Rotate();
        }
        // Debug.Log(activing_Func);
    }

    void Rotate()
    {
        attack.Work_Dir();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (vitality <= 0) return;
        if (other.CompareTag("Bullet"))
        {
            vitality--;
            healthSlider.value -= 1;
            other.gameObject.SetActive(false);
            GameObject effect = Instantiate(hitEffect, hitPos.position, Quaternion.identity);    // 피격 이펙트 동적 생성
            cs.SetIsHit(true);
            cs.SetHit();
            Invoke("ChangeOriginShader", 0.05f);
            Destroy(effect, 2.0f);  // 1초후 삭제
            if (vitality <= 0)
            {
                Die();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

    }


    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Selector root = new Selector();

        Selector behaviour = new Selector();

        Sequence attack_Sequence_Node = new Sequence();

        Leaf_Node trace_Node = new Leaf_Node(Trace);
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node attack_Condition_Node = new Leaf_Node(Distance_Attack_Condition);

        attack_Sequence_Node.AddChild(attack_Condition_Node);
        attack_Sequence_Node.AddChild(attack_Node);

        behaviour.AddChild(attack_Sequence_Node);
        behaviour.AddChild(trace_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);
    }



    public RESULT Distance_Attack_Condition()
    {
        if (trace.Condition(1.2f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Trace()
    {
        if (eEnemy_State == ENEMY_STATE.WALK) trace.Work();
        if (activing_Func.Equals("Trace")) return RESULT.RUNNING;
        trace.Init();
        activing_Func = "Trace";
        animator.SetTrigger("isWalk");
        eEnemy_State = ENEMY_STATE.WALK;
        agent.isStopped = false;
        return RESULT.SUCCESS;
    }

    public RESULT Attack()
    {
        if (activing_Func.Equals("Attack")) return RESULT.RUNNING;
        attack.Init();
        attack.Work();
        activing_Func = "Attack";
        animator.SetTrigger("isAttack");
        eEnemy_State = ENEMY_STATE.ATTACK;

        agent.isStopped = true;
        isOther_State_Change = true;                //다른 상태로 바꿀수 없다
        return RESULT.SUCCESS;
    }

    new void Die()
    {
        activing_Func = "Death";

        int r = Random.Range(0, 2);
        if (r == 0)
            animator.SetTrigger("isDeath");
        else if ( r == 1)
            animator.SetTrigger("isDeath2");
        eEnemy_State = ENEMY_STATE.DIE;
        isOther_State_Change = true;
        agent.isStopped = true;
        base.Die();
    }


    void Exit_Attack()
    {
        agent.isStopped = false;
        activing_Func = string.Empty;
        isOther_State_Change = false;
    }

    void Event_SendDamage()
    {
        if (trace.Condition(1.2f))
            attack.Send_Damage();
    }

    public void WoundExplosionDamage()     //시간이없으니까 컴포넌트대신 함수로
    {
        if (vitality <= 0) return;
        vitality -= 2;
        healthSlider.value -= 2;
        Debug.Log("맞음");
        GameObject effect = Instantiate(hitEffect, hitPos.position, Quaternion.identity);    // 피격 이펙트 동적 생성
        Destroy(effect, 2.0f);  // 1초후 삭제
        if (vitality <= 0)
        {
            Die();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    // invoke에 사용하려고 만듦
    void ChangeOriginShader()
    {
        cs.SetIsHit(false);
        cs.SetOrigin();
    }
}
