using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Alien : Enemy
{
    ENEMY_STATE eState = ENEMY_STATE.IDLE;
    BehaviorTree bt;
    public int vitality = 5;   // 체력
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly int hashDeath = Animator.StringToHash("isDeath");
    private readonly int hashRoll = Animator.StringToHash("isRoll");

    /// <summary>
    /// /////////////////////////인공지능 
    /// </summary>
    Wander wander;
    Attack attack;
    Trace trace;
    Roll roll;
    /// <summary>
    /// /////////////////////기능
    /// </summary>
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
        Build_BT();

        wander.Init(2.0f);      //배회할때 걷는 속도
        animator.SetTrigger("isWalk");
    }

    // Update is called once per frame
    void Update()
    {
        bt.Run();
        
        Render();
    }
    
    void Idle()             //쓰지않음
    {
        eState = ENEMY_STATE.IDLE;
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
    
    public bool Attack()   
    {
        attack.Work();
        if (eState != ENEMY_STATE.ATTACK)
            animator.SetTrigger("isAttack");        
        eState = ENEMY_STATE.ATTACK;
        return true;
    }
    

    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Sequence root = new Sequence();
        Sequence death = new Sequence();
        Selector behaviour = new Selector();
        Sequence rolling_Sequence = new Sequence();
        Sequence attack_Sequence = new Sequence();
        Sequence trace_Sequence = new Sequence();        
        Leaf_Node isVitalityZero_Node = new Leaf_Node(isVitalityZero);
        Leaf_Node die_Node = new Leaf_Node(Die);
        Leaf_Node isBulletComeToMe_Node = new Leaf_Node(IsBulletComeToMe);
        Leaf_Node rolling_Node = new Leaf_Node(Rolling);
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node wander_Node = new Leaf_Node(Wander);
        Leaf_Node trace_Node = new Leaf_Node(Trace);
        Leaf_Node_Float trace_Condition_Node = new Leaf_Node_Float(Distance_Condition,6.0f);
        Leaf_Node_Float attack_Condition_Node = new Leaf_Node_Float(Distance_Condition, 2.0f);

        //노드 연결
        root.AddChild(death);
        root.AddChild(behaviour);

        death.AddChild(isVitalityZero_Node);
        death.AddChild(die_Node);

        behaviour.AddChild(rolling_Sequence);
        behaviour.AddChild(attack_Sequence);
        behaviour.AddChild(trace_Sequence);
        behaviour.AddChild(wander_Node);

        rolling_Sequence.AddChild(isBulletComeToMe_Node);
        rolling_Sequence.AddChild(rolling_Node);
        
        attack_Sequence.AddChild(attack_Condition_Node);
        attack_Sequence.AddChild(attack_Node);

        trace_Sequence.AddChild(trace_Condition_Node);
        trace_Sequence.AddChild(trace_Node);                        
        
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public bool Wander()
    {
        RESULT result = wander.Work();
        if (result == RESULT.SUCCESS)
            animator.SetTrigger("isWalk");
        eState = ENEMY_STATE.WALK;
        return true;
    }

    public bool Distance_Condition(float _dis) 
    {
        if (trace.Condition(_dis))  return true; 
        return false;
    }

    public bool Trace()
    {
        RESULT result = trace.Work();
        if (eState != ENEMY_STATE.RUN)
            animator.SetTrigger("isRun");
        eState = ENEMY_STATE.RUN;
        return true;
    }

    void Render()
    {
        switch(eState)
        {
            case ENEMY_STATE.IDLE:               
                break;
        }
    }

    public bool IsBulletComeToMe()
    {
        if (roll.IsBulletComeToMe()) return true;
        return false;
    }

    public bool Rolling()
    {
        roll.Rolling();
        if (eState != ENEMY_STATE.ROLL)
            animator.SetTrigger(hashRoll);
        eState = ENEMY_STATE.ROLL;        
        return true;
    }
}
