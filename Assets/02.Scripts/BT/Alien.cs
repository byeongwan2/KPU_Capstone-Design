using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Alien : Enemy
{
    BehaviorTree bt;
    public int vitality = 5;   // 체력
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly int hashDeath = Animator.StringToHash("isDeath");
    private readonly int hashRoll = Animator.StringToHash("isRoll");
    [SerializeField]
    bool isOther_State_Change = false;
    float traceCoverage = 6.0f;
    /// /////////////////////////인공지능 
    Wander wander;
    Attack attack;
    Trace trace;
    Roll roll;
    /// /////////////////////기능
    [SerializeField]
    bool isMust_Trace = false;
    [SerializeField]
    string active_Func = string.Empty;
    [SerializeField]
    int is_Rolling_Count = 0;

    Rigidbody rb;
    private void Awake()
    {
        base.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        wander = GetComponent<Wander>();
        attack = GetComponent<Attack>();
        trace = GetComponent<Trace>();
        roll = GetComponent<Roll>();
        trace.Init_Target(system.pPlayer2);
        attack.Init_Target(system.pPlayer2);
        
        wander.Init(agent,1.0f);      //배회할때 걷는 속도
        trace.Init(agent,2.0f);
        attack.Init(agent, 10);
        roll.Init(agent,4.0f);
        animator.SetTrigger("isWalk");
        Build_BT();

        Init_Data();

        rb = GetComponent<Rigidbody>();
    }
    
    void Init_Data()
    {
        traceCoverage = 6.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (eEnemy_State == ENEMY_STATE.DIE)
            return;
        if (!isOther_State_Change)
        { 
            bt.Run();
        }

        Debug.Log(active_Func);                 //디버깅
    }
   

    private void OnTriggerEnter(Collider other)
    {
        if (vitality <= 0) return;
        if(other.CompareTag("Bullet"))
        {
            vitality--;
            Debug.Log("맞음");
            other.gameObject.SetActive(false);
            if (vitality <= 0)
            {
                Die();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        
    }

    void Die() // Die 액션
    {
        active_Func = "Death";
        animator.SetTrigger(hashDeath);
        eEnemy_State = ENEMY_STATE.DIE;
        isOther_State_Change = true;
        agent.isStopped = true;
    }                   
    
    public RESULT Attack()   
    {
        if (active_Func.Equals("Attack")) return RESULT.RUNNING;
        attack.Work();
        active_Func = "Attack";
        animator.SetTrigger("isAttack");
        eEnemy_State = ENEMY_STATE.ATTACK;

        isOther_State_Change = true;                //다른 상태로 바꿀수 없다
        isMust_Trace = false;                       //반드시 추적하는 기능을 해제한다.
        return RESULT.SUCCESS;
    }  

    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Selector root = new Selector();

        Selector behaviour = new Selector();
        root.AddChild(behaviour);



        Selector avoid_Selecter = new Selector();
        Sequence trace_Sequence = new Sequence();
        Leaf_Node wander_Node = new Leaf_Node(Wander);

        Leaf_Node Valid_Range_Condition = new Leaf_Node(IsValid_Range_Condition);
        Leaf_Node_Float trace_Condition_Node = new Leaf_Node_Float(Distance_Condition, traceCoverage);
        Sequence trace_Sequence_Node = new Sequence(Trace);

        Sequence trace_lookAround_Sequence = new Sequence();
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node isBulletComeToMe_Node = new Leaf_Node(IsBulletComeToMe);
        Leaf_Node rolling_Node = new Leaf_Node(Rolling);
        Leaf_Node attack_Condition_Node = new Leaf_Node(Distance_Attack_Condition);
        Leaf_Node lookAround_Node = new Leaf_Node(LookAround);

   
        behaviour.AddChild(avoid_Selecter);
        behaviour.AddChild(trace_Sequence);
        behaviour.AddChild(wander_Node);

        trace_Sequence.AddChild(Valid_Range_Condition);
        trace_Sequence.AddChild(trace_Condition_Node);
        trace_Sequence.AddChild(trace_Sequence_Node);

        trace_Sequence_Node.AddChild(attack_Condition_Node);
        trace_Sequence_Node.AddChild(attack_Node);

        Sequence rolling_Sequence = new Sequence();
        avoid_Selecter.AddChild(rolling_Sequence);
        rolling_Sequence.AddChild(isBulletComeToMe_Node);
        rolling_Sequence.AddChild(rolling_Node);

        avoid_Selecter.AddChild(trace_lookAround_Sequence);

        Leaf_Node LookAround_Condition_Node = new Leaf_Node(LookAround_Condition);
        Leaf_Node Get_IsRolling_Play_Node = new Leaf_Node(Get_IsRolling_Play);
        trace_lookAround_Sequence.AddChild(Get_IsRolling_Play_Node);
        trace_lookAround_Sequence.AddChild(LookAround_Condition_Node);
        trace_lookAround_Sequence.AddChild(lookAround_Node);
            
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public RESULT Wander()
    {
        if (eEnemy_State == ENEMY_STATE.WALK ) wander.Work();
        if (active_Func.Equals("Wander")) return RESULT.RUNNING;
        active_Func = "Wander";
        animator.SetTrigger("isWalk");
        eEnemy_State = ENEMY_STATE.WALK;
        return RESULT.SUCCESS;
    }

    public RESULT Distance_Condition(float _dis)            //컨디션은 러닝이 필요없음  둘다 퍼스로 리턴
    {
        if (isMust_Trace) _dis = 30.0f;
        if (trace.Condition(_dis))  return RESULT.SUCCESS; 
        return RESULT.FAIL;
    }

    public RESULT Distance_Attack_Condition()
    {
        if (trace.Condition(2.0f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Trace()
    {
        if(eEnemy_State == ENEMY_STATE.RUN) trace.Work();
        if (active_Func.Equals("Trace")) return RESULT.RUNNING;
        active_Func = "Trace";
        animator.SetTrigger("isRun");
        eEnemy_State = ENEMY_STATE.RUN;
        agent.isStopped = false;
        return RESULT.SUCCESS;
    }

    public RESULT IsBulletComeToMe()
    {
        if (is_Rolling_Count > 1)
        {
            isMust_Trace = true;
            return RESULT.FAIL;
        }
        switch (roll.IsBulletComeToMe())
        {
            case RESULT.SUCCESS:
                return RESULT.SUCCESS;
            case RESULT.RUNNING:
                return RESULT.RUNNING;
            case RESULT.FAIL:
                return RESULT.FAIL;
        }
        return RESULT.FAIL;
    }
    public bool IsNotRollingCoolTime()
    {
        return roll.IsNotRollingCoolTime() ? true : false;
    }

    public RESULT Rolling()
    {
        if (active_Func.Equals("Rolling")) return RESULT.RUNNING;
        active_Func = "Rolling";
        roll.Rolling(); 
        animator.SetTrigger(hashRoll);
        eEnemy_State = ENEMY_STATE.ROLL;
        isOther_State_Change = true;
        is_Rolling_Count ++;
        return RESULT.SUCCESS;
    }

    public RESULT IsValid_Range_Condition()         //에일리언의 행동범위를 벗어나면 무조건추적해야되도 그만둔다
    {
        float dis = Check.Distance(wander.wayPoints[0].position, transform.position);
        if (dis < 20.0f) return RESULT.SUCCESS;
        else
        {
            isMust_Trace = false;
            is_Rolling_Count = 0;
            return RESULT.FAIL;

        }
    }

    public RESULT LookAround()
    {
        if (active_Func.Equals("LookAround")) return RESULT.RUNNING;
        active_Func = "LookAround";
        eEnemy_State = ENEMY_STATE.LOOKAROUND;
        animator.SetTrigger("isLookAround");
        isOther_State_Change = false;
        return RESULT.SUCCESS;
    }

    public RESULT Get_IsRolling_Play()
    {
        if (isMust_Trace) return RESULT.FAIL;
        if (is_Rolling_Count > 0) return RESULT.SUCCESS;
        else  return RESULT.FAIL;
    }

    public RESULT LookAround_Condition()
    {
         switch(Distance_Condition(traceCoverage))
        {
            case RESULT.SUCCESS:
                is_Rolling_Count = 0;
                return RESULT.FAIL;
            default: return RESULT.SUCCESS;
        }
    }



    void Exit_Rolling()
    {
        isOther_State_Change = false;           //구르기이후실행
    }

    public void Exit_Motion()       // Exit가 붙은함수는 전부 툴이실행해주는 콜백함수      //공격/두리번 이후 실행
    {
        agent.isStopped = false;
        active_Func = string.Empty;
        isOther_State_Change = false;

    }
    
    void Exit_LookAround()
    {
        is_Rolling_Count = 0;
    }
}
