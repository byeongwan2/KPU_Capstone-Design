using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Drone : Enemy
{
    BehaviorTree bt;
    Rigidbody rb;
    ChangeShader cs;

    // 해시값 추출
    private readonly int hashDeath = Animator.StringToHash("isDeath");
    private readonly int hashAttack = Animator.StringToHash("isAttack");    
    
    [SerializeField]
    bool isOther_State_Change = false;

    // 행동트리 변수
    Wander wander;
    Drone_Attack attack;
    Drone_Trace trace;
    EnemyFOV enemyFOV;      // 시야각 및 추적 반경 제어 변수
    
        
    // 기능
    [SerializeField]
    bool isMust_Trace = false;
    [SerializeField]
    string activing_Func = string.Empty; // Running을 위한 문자열 비교
    public GameObject hitEffect;        // 피격 효과 프리팹
    public Transform hitPos;    

    void Start()
    {
        // 자식 오브젝트의 ChangeMaterial 컴포넌트를 가지고 온다
        cs = GetComponentInChildren<ChangeShader>();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        attack = GetComponent<Drone_Attack>();
        trace = GetComponent<Drone_Trace>();
        enemyFOV = GetComponent<EnemyFOV>();
                
        Build_BT();
    }
        
    void Update()
    {        
        if (eEnemy_State == ENEMY_STATE.DIE)
            return;
        
        if (isOther_State_Change == false)
        {
            bt.Run();
        }      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (vitality <= 0) return;
        if (other.CompareTag("Bullet"))
        {
            vitality--;                         
            other.gameObject.SetActive(false);
            GameObject effect = Instantiate(hitEffect, hitPos.position, Quaternion.identity);
            cs.SetIsHit(true);
            cs.SetHit();
            Invoke("ChangeOriginShader", 0.05f);
            Destroy(effect, 2.0f);
        }
    }
    
    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        
        // level 0
        Selector root = new Selector();

        // level 1
        Sequence death_Sequence = new Sequence();
        Selector behaviour_Selector = new Selector();
        //Leaf_Node isPlayerDead = new Leaf_Node();        

        // level 2
        Leaf_Node isHpZero = new Leaf_Node(IsHpZero);
        Leaf_Node die = new Leaf_Node(Die);
        Sequence attack_Sequence = new Sequence();
        Sequence trace_Sequence = new Sequence();
        //Leaf_Node wander = new Leaf_Node(Wander);

        // level 3
        Leaf_Node isAttackRange = new Leaf_Node(IsAttackRange);
        Leaf_Node attack = new Leaf_Node(Attack);
        Leaf_Node isTraceRange = new Leaf_Node(IsTraceRange);
        Leaf_Node trace = new Leaf_Node(Trace);

        // 노드 연결

        // level 0 - 1
        root.AddChild(death_Sequence);
        root.AddChild(behaviour_Selector);
        //root.AddChild(isPlayerDead);

        // level 1 - 2
        death_Sequence.AddChild(isHpZero);
        death_Sequence.AddChild(die);

        behaviour_Selector.AddChild(attack_Sequence);
        behaviour_Selector.AddChild(trace_Sequence);
        //behaviour_Selector.AddChild(wander);

        // level 2 - 3
        attack_Sequence.AddChild(isAttackRange);
        attack_Sequence.AddChild(attack);

        trace_Sequence.AddChild(isTraceRange);
        trace_Sequence.AddChild(trace);

        bt = new BehaviorTree(root);    // 트리가 완성되면 Drone 행동트리 멤버변수에 적용
    }
    
    public RESULT IsHpZero()
    {
        if(vitality <= 0)
        {
            return RESULT.SUCCESS;
        }
        else
        {
            return RESULT.FAIL;
        }
    }

    public RESULT Die() // Die 액션
    {
        if (activing_Func.Equals("Death")) return RESULT.RUNNING;
        activing_Func = "Death";
        animator.SetTrigger(hashDeath);
        eEnemy_State = ENEMY_STATE.DIE;
        isOther_State_Change = true;
        //agent.isStopped = true;
        return RESULT.SUCCESS;
    }
            
    public RESULT IsAttackRange()
    {
        if (enemyFOV.isAttackPlayer())
        {            
            return RESULT.SUCCESS;
        }
        else
            return RESULT.FAIL;
    }

    public RESULT Attack()
    {        
        activing_Func = "Attack";
        animator.SetTrigger(hashAttack);
        attack.DroneRotate();
        attack.Attack();
        eEnemy_State = ENEMY_STATE.ATTACK;
        return RESULT.SUCCESS;
    }

    public RESULT IsTraceRange()
    {
        if (!enemyFOV.isAttackPlayer() && enemyFOV.isTracePlayer())  // 공격범위가 아니고 추적범위라면
        {
            return RESULT.SUCCESS;
        }
        else
            return RESULT.FAIL;
    }

    public RESULT Trace()
    {        
        activing_Func = "Trace";        
        trace.Trace();
        eEnemy_State = ENEMY_STATE.RUN;
        return RESULT.SUCCESS;
    }        

    // invoke에 사용하려고 만듦
    void ChangeOriginShader()
    {
        cs.SetIsHit(false);
        cs.SetOrigin();
    }    
}