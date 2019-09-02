using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot2 : Enemy
{
    BehaviorTree bt;
    // Start is called before the first frame update
    Idle idle;
    Shot shot;
    Trace trace;
    string activing_Func = string.Empty;
    Behaviour haviour;


    [SerializeField]
    bool isOther_State_Change = false;
    void Start()
    {

        base.Init();

        idle = GetComponent<Idle>();
        shot = GetComponent<Shot>();
        trace = GetComponent<Trace>();
        trace.Init_Target(PrefabSystem.instance.player);
        trace.Setting(agent, 1.0f);
        haviour = idle;
        eEnemy_State = ENEMY_STATE.IDLE;
        Build_BT();
    }

    // Update is called once per frame
    void Update()
    {
        if(isOther_State_Change == false)
            bt.Run();
    }

    void Build_BT()
    {
        Selector root = new Selector();

        Selector behaviour = new Selector();

        Leaf_Node idle_Node = new Leaf_Node(Idle);

        Sequence trace_Sequence = new Sequence();
        Leaf_Node trace_Condition_Node = new Leaf_Node(Trace_Codition);
        Leaf_Node trace_Node = new Leaf_Node(Trace);

        trace_Sequence.AddChild(trace_Condition_Node);          //추적가능한 거리에있는지
        trace_Sequence.AddChild(trace_Node);           //추적시퀸스 실행
        behaviour.AddChild(trace_Sequence);


        behaviour.AddChild(idle_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }
    RESULT Trace_Codition()
    {
        if (trace.Condition(10.0f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }


    RESULT Idle()
    {

        if (eEnemy_State == ENEMY_STATE.IDLE) idle.Work();
        if (activing_Func.Equals("Idle")) return RESULT.RUNNING;
        haviour.End();
        haviour = idle;
        activing_Func = "Idle";
        animator.SetTrigger("Idle");
        eEnemy_State = ENEMY_STATE.IDLE;
        return RESULT.SUCCESS;
    }

    RESULT Trace()
    {
        if (eEnemy_State == ENEMY_STATE.RUN) trace.Work();
        if (activing_Func.Equals("Trace")) return RESULT.RUNNING;
        trace.Init();
        activing_Func = "Trace";
        animator.SetTrigger("Trace");
        eEnemy_State = ENEMY_STATE.RUN;
        agent.isStopped = false;
        return RESULT.SUCCESS;
    }

    RESULT Change()
    {
         if (activing_Func.Equals("Change")) return RESULT.RUNNING;
        return RESULT.SUCCESS;
    }
}
