using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Alien : Enemy
{
    STATE eState;
    BehaviorTree bt;
    public int vitality = 5;   // 체력
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly int hashDeath = Animator.StringToHash("isDeath");
    Wander wander;
    Attack attack;
    Trace trace;
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
    }

    // Update is called once per frame
    void Update()
    {
        bt.Run();
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

    public bool Trace()
    {
        trace.Work();
        //달리기코드
        return true;
    }
    

    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Sequence root = new Sequence();
        Sequence Death = new Sequence();
        Sequence search = new Sequence();
        Selector behaviour = new Selector();
       // Leaf_Node isVitalityZero_Node = new Leaf_Node(isVitalityZero);
        //Leaf_Node Die_Node = new Leaf_Node(Die);
        // Leaf_Node Attack_Node = new Leaf_Node(Attack);
        Leaf_Node Wander_Node = new Leaf_Node(Wander);
        Leaf_Node Trace_Node = new Leaf_Node(Trace);
        //노드 연결
        root.AddChild(Death);
        root.AddChild(search);
        root.AddChild(behaviour);
        //Death.AddChild(isVitalityZero_Node);
        //Death.AddChild(Die_Node);
        // behaviour.AddChild(Attack_Node);
        search.AddChild(Trace_Node);
        behaviour.AddChild(Wander_Node);
        behaviour.AddChild(Trace_Node);
        
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public bool Wander()
    {
        wander.Work();
        animator.SetBool("IsWalk", true);
        return true;
    }
    
}
