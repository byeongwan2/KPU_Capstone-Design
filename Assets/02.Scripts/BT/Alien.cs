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
    bool isOther_State_Change = false;
    float traceCoverage = 6.0f;
    /// /////////////////////////인공지능 
    Wander wander;
    Attack attack;
    Trace trace;
    Roll roll;
    /// /////////////////////기능
    bool isMust_Trace = false;

    string active_Func = string.Empty;
    private void Awake()
    {
        base.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = 100;
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
    }
    
    void Init_Data()
    {
        traceCoverage = 6.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOther_State_Change)
             bt.Run();

        Debug.Log(active_Func);                 //디버깅
    }
    
    void Idle()             //쓰지않음
    {
        eEnemy_State = ENEMY_STATE.IDLE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            vitality--;
        }
    }

    public bool isVitalityZero()    // 체력이 0이하인가?
    {
        if(vitality<=0)
        {            
            return true;
        }
        else
        {            
            return false;
        }
    }

    public bool Die() // Die 액션
    {
        animator.SetTrigger(hashDeath);        
        return true;
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
        Sequence root = new Sequence();
        //Sequence death = new Sequence();
        Selector behaviour = new Selector();
        
        Sequence attack_Sequence = new Sequence();
        Sequence trace_Sequence = new Sequence();

        Selector trace_lookAround_Selector = new Selector();
        Sequence lookAround_Sequence = new Sequence();
        Sequence rolling_Sequence = new Sequence();
        //Leaf_Node isVitalityZero_Node = new Leaf_Node(isVitalityZero);
        //Leaf_Node die_Node = new Leaf_Node(Die);
        /*
        Leaf_Node isBulletComeToMe_Node = new Leaf_Node(IsBulletComeToMe);
        Leaf_Node isDelayTime_Roll_Node = new Leaf_Node(IsNotRollingCoolTime);
        Leaf_Node rolling_Node = new Leaf_Node(Rolling);
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node lookAround_Condition_Node = new Leaf_Node(LookAround_Condition);
        
        Leaf_Node_Float attack_Condition_Node = new Leaf_Node_Float(Distance_Condition, 2.0f);
        Leaf_Node_Float trace_Must_Condition_Node = new Leaf_Node_Float(Distance_Condition_Must, 30.0f);
        */
        //노드 연결
        // root.AddChild(death);
        Leaf_Node wander_Node = new Leaf_Node(Wander);
        Leaf_Node trace_Node = new Leaf_Node(Trace);
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node_Float trace_Condition_Node = new Leaf_Node_Float(Distance_Condition, 6.0f);
        Leaf_Node_Float attack_Condition_Node = new Leaf_Node_Float(Distance_Condition, 2.0f);
        root.AddChild(behaviour);

        behaviour.AddChild(attack_Sequence);
        behaviour.AddChild(trace_Sequence);
        behaviour.AddChild(wander_Node);
      
        
        trace_Sequence.AddChild(trace_Condition_Node);
        trace_Sequence.AddChild(trace_Node);

        attack_Sequence.AddChild(attack_Condition_Node);
        attack_Sequence.AddChild(attack_Node);
        //death.AddChild(isVitalityZero_Node);
        // death.AddChild(die_Node);
        /*
        behaviour.AddChild(trace_lookAround_Selector);
        

        behaviour.AddChild(attack_Sequence);
        behaviour.AddChild(trace_Sequence);
        behaviour.AddChild(wander_Node);

        trace_lookAround_Selector.AddChild(lookAround_Sequence);
        trace_lookAround_Selector.AddChild(rolling_Sequence);

        lookAround_Sequence.AddChild(lookAround_Condition_Node);
        lookAround_Sequence.AddChild(trace_Must_Condition_Node);
        lookAround_Sequence.AddChild(trace_Node);

        rolling_Sequence.AddChild(isDelayTime_Roll_Node);
        rolling_Sequence.AddChild(isBulletComeToMe_Node);
        rolling_Sequence.AddChild(rolling_Node);
        
        attack_Sequence.AddChild(attack_Condition_Node);
        attack_Sequence.AddChild(attack_Node);

        trace_Sequence.AddChild(trace_Condition_Node);
        trace_Sequence.AddChild(trace_Node);                        
        */
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
        if (trace.Condition(_dis)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Trace()
    {
        if(eEnemy_State == ENEMY_STATE.RUN) trace.Work();
        if (active_Func.Equals("Trace")) return RESULT.RUNNING;
        active_Func = "Trace";
        animator.SetTrigger("isRun");
        eEnemy_State = ENEMY_STATE.RUN;
        return RESULT.SUCCESS;
    }

    public bool IsBulletComeToMe()
    {
        return roll.IsBulletComeToMe() ?true : false;
    }
    public bool IsNotRollingCoolTime()
    {
        return roll.IsNotRollingCoolTime() ? true : false;
    }

    public bool Rolling()
    {
        roll.Rolling(); 
        if (eEnemy_State != ENEMY_STATE.ROLL)
            animator.SetTrigger(hashRoll);
        eEnemy_State = ENEMY_STATE.ROLL;
        isOther_State_Change = true;
        return true;
    }

    bool LookAround_Condition()
    {
        if (isMust_Trace) return true;
        if (eEnemy_State != ENEMY_STATE.LOOKAROUND)    return false;        //두리번거리고 있지않다면 이조건은 실행할필요없음
        //if (Distance_Condition(6.0f) || IsBulletComeToMe())
        //{
        ////    eEnemy_State = ENEMY_STATE.IDLE;
        //    Exit_Motion();
        //    return true;
        //}
       
        return true;            //트루로 나갈때는 다른상태를 실행
    }

    public bool Distance_Condition_Must(float _dis)
    {
        isMust_Trace = true;
        return trace.Condition(_dis) ? true : false;
    }


    void Exit_LookAround()
    {
        eEnemy_State = ENEMY_STATE.LOOKAROUND;
        isOther_State_Change = false;
       // if (Distance_Condition(6.0f)) return;

       // animator.SetTrigger("isLookAround");
    }

    void Exit_AnimationState_Reset()
    {
        eEnemy_State = ENEMY_STATE.IDLE;
    }

    public void Exit_Motion()       // Exit가 붙은함수는 전부 툴이실행해주는 콜백함수
    {
        isOther_State_Change = false;
        agent.isStopped = false;
        active_Func = string.Empty;
    }
}
