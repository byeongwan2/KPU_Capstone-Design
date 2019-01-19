using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Alien : Enemy
{
    ENEMY_STATE eState = ENEMY_STATE.IDLE;
    BehaviorTree bt;
    public int vitality = 5;   // 체력
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly int hashDeath = Animator.StringToHash("isDeath");
    /// <summary>
    /// /////////////////////////인공지능 
    /// </summary>
    Wander wander;
    Attack attack;
    Trace trace;
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

        wander = GetComponent<Wander>();
        attack = GetComponent<Attack>();
        trace = GetComponent<Trace>();
        trace.Init_Target(system.pPlayer2);
        Build_BT();

        wander.Init(2.0f);      //배회할때 걷는 속도
    }

    // Update is called once per frame
    void Update()
    {
        bt.Run();
      
        Render();
    }
    
    void Idle()
    {
        eState = ENEMY_STATE.IDLE;
    }

    void FixedUpdate()
    {

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

        animator.SetBool(hashDeath, true);        
        return true;
    }                   
    
    public bool Attack()   
    {
        attack.Work();
        animator.SetBool(hashAttack, true);
        return true;
    }
    

    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Sequence root = new Sequence();
        Sequence Death = new Sequence();
        Sequence behaviour = new Sequence();
       // Leaf_Node isVitalityZero_Node = new Leaf_Node(isVitalityZero);
        //Leaf_Node Die_Node = new Leaf_Node(Die);
        // Leaf_Node Attack_Node = new Leaf_Node(Attack);
        Leaf_Node Wander_Node = new Leaf_Node(Wander);
        Leaf_Node Trace_Node = new Leaf_Node(Trace);
        Leaf_Node Trace_Condition_Node = new Leaf_Node(Trace_Condition);
        //노드 연결
        root.AddChild(Death);
        root.AddChild(behaviour);
        //Death.AddChild(isVitalityZero_Node);
        //Death.AddChild(Die_Node);
        // behaviour.AddChild(Attack_Node);

        behaviour.AddChild(Wander_Node);
        behaviour.AddChild(Trace_Condition_Node);
        behaviour.AddChild(Trace_Node);
        
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public bool Wander()
    {
        wander.Work();
        animator.SetBool("IsWalk", true);
        eState = ENEMY_STATE.WALK;
        return true;
    }

    public bool Trace_Condition()
    {
        if (trace.Condition())  return true; 
        return false;
    }

    public bool Trace()
    {
        trace.Work();
        animator.SetBool("IsRun", true);
        eState = ENEMY_STATE.RUN;
        return true;
    }

    

    void Render()
    {
        switch(eState)
        {
            case ENEMY_STATE.IDLE:
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", false);
                break;
        }
    }
}
