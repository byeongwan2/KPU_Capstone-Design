using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보스는 심플하게~
public class Boss : MoveObject
{
    BehaviorTree bt;

    BOSS_STATE eBoss_State = BOSS_STATE.IDLE;
    [SerializeField]
    string activing_Func = string.Empty;
    /// ////////////////////기본기능
    Attack attack;
    Trace trace;
    Wander wander;
    Alert alert;
    Idle idle;
    /// ////////////////////필수기능
    Animator animator;

    void Start()
    {
        //체력
        vitality = 100;
        wander = GetComponent<Wander>();
        attack = GetComponent<Attack>();
        trace = GetComponent<Trace>();
        alert = GetComponent<Alert>();
        idle = GetComponent<Idle>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }
    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Selector root = new Selector();

        Selector behaviour = new Selector();
        root.AddChild(behaviour);

        Leaf_Node alert_Node = new Leaf_Node(Alert);


        Leaf_Node idle_Node = new Leaf_Node(Idle);
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public RESULT Alert()
    {
        if (eBoss_State == BOSS_STATE.ALERT) alert.Work();
        if (activing_Func.Equals("Alert")) return RESULT.RUNNING;
        alert.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Alert";
        animator.SetTrigger("isWalk");
        eBoss_State = BOSS_STATE.ALERT;
        return RESULT.SUCCESS;
    }

    public RESULT Idle()
    {
        /*
        if (eBoss_State == BOSS_STATE.IDLE) idle.Work();
        if (activing_Func.Equals("Alert")) return RESULT.RUNNING;
        alert.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Alert";
        animator.SetTrigger("isWalk");
        eBoss_State = BOSS_STATE.ALERT;*/
        return RESULT.SUCCESS;
    }

}
