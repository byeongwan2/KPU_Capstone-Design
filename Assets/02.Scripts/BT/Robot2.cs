using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot2 : Enemy
{
    BehaviorTree bt;
    // Start is called before the first frame update
    Idle idle;
    Attack attack;
    Trace trace;
    string activing_Func = string.Empty;
    Behaviour haviour;

    Laser laser;

    [SerializeField]
    bool isOther_State_Change = false;
    void Start()
    {

        base.Init();

        idle = GetComponent<Idle>();
        attack = GetComponent<Attack>();
        attack.Setting(agent, 10);
        attack.Init_Target(PrefabSystem.instance.player);
        trace = GetComponent<Trace>();
        trace.Init_Target(PrefabSystem.instance.player);
        trace.Setting(agent, 1.0f);
        haviour = idle;
        eEnemy_State = ENEMY_STATE.IDLE;

        laser = GetComponentInChildren<Laser>();
        laser.gameObject.SetActive(false);

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

        Sequence attack_Sequence = new Sequence();
        Leaf_Node attack_Condition_Node = new Leaf_Node(Attack_Condition);
        Leaf_Node attack_Node = new Leaf_Node(Attack);

        attack_Sequence.AddChild(attack_Condition_Node);
        attack_Sequence.AddChild(attack_Node);
        behaviour.AddChild(attack_Sequence);

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

    RESULT Attack_Condition()
    {
        if (trace.Condition(4.0f)) return RESULT.SUCCESS;
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
        haviour.End();
        haviour = trace;
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
        haviour.End();
        activing_Func = "Change";
        animator.SetTrigger("FormChangeToRoller");
        eEnemy_State = ENEMY_STATE.CHAGNE;
        isOther_State_Change = true;
        return RESULT.SUCCESS;
    }
    RESULT Attack()
    {
        if (activing_Func.Equals("Attack")) return RESULT.RUNNING;
        haviour.End();
        haviour = attack;
        attack.Init();
        attack.Work();
        laser.gameObject.SetActive(true);
        activing_Func = "Attack";
        animator.SetTrigger("Attack");
        eEnemy_State = ENEMY_STATE.ATTACK;

        isOther_State_Change = true;                //다른 상태로 바꿀수 없다
        return RESULT.SUCCESS;
    }
    void Exit_Attack()          //장전,공격이 끝나면 이함수 호출
    {
        agent.isStopped = false;
        activing_Func = string.Empty;
        isOther_State_Change = false;
    }
}
