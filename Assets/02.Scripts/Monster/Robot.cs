using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy {
    enum UP_BODY_STATE { THROW,SHOT}
    UP_BODY_STATE eMotionState;
    BehaviorTree bt;

    Trace trace;
    float traceCoverage = 6.0f;
    bool isOther_State_Change = false;
    [SerializeField]
    string activing_Func = string.Empty;
    void Start()
    {
        base.Init();
        trace = GetComponent<Trace>();
        trace.Init_Target(system.pPlayer2);
        trace.Setting(agent, 2.0f);
        traceCoverage = 6.0f;
        eEnemy_State = ENEMY_STATE.WALK;
        animator.SetTrigger("IsWALK");
        Build_BT();
    }

    void Update()
    {
        if (isOther_State_Change == false)
        {
            bt.Run();
        }

    }

    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Selector root = new Selector();

        Selector behaviour = new Selector();
        
        
        Leaf_Node trace_Node = new Leaf_Node(Trace);
        behaviour.AddChild(trace_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);
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

    void MotionRender()
    {
        switch(eMotionState)
        {
            case UP_BODY_STATE.SHOT:
                animator.SetBool("IsAttack",true);
                break;
            case UP_BODY_STATE.THROW:

                break;
        }
    }

    

    
   
}
