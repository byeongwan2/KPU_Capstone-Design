using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보스는 심플하게~
public class Boss : MoveObject
{
    BehaviorTree bt;
    [SerializeField]
    BOSS_STATE eBoss_State = BOSS_STATE.IDLE;
    [SerializeField]
    string activing_Func = string.Empty;
    /// ////////////////////기본기능
    Shot shot;
    Alert alert;
    Idle idle;
    Walk walk;
    /// ////////////////////필수기능
    Animator animator;

    [SerializeField]
    bool isOther_State_Change = false;

    void Start()
    {
        //체력
        vitality = 100;
        //attack = GetComponent<Attack>();
        walk = GetComponent<Walk>();    //보스의 경우 walk는 사실상 trace에서 네비게이션을 뺸것
        alert = GetComponent<Alert>();
        idle = GetComponent<Idle>();
        shot = GetComponent<Shot>();
        shot.Init("BossBullet", 5, 2, 3, TYPE.BOSSBULLET, Object_Id.MONSTER);     //상태가 바뀔때 한번만 호출되어야함
        animator = GetComponent<Animator>();

        Build_BT();
    }

    void Update()
    {
        if (isOther_State_Change == false)
        
            bt.Run();

        
    }
    void Build_BT() // 행동트리 생성
    {
        // 노드 생성
        Selector root = new Selector();

        Selector behaviour = new Selector();
        
        Leaf_Node alert_Node = new Leaf_Node(Alert);


        Leaf_Node idle_Node = new Leaf_Node(Idle);

        Sequence walk_Sequence_Node = new Sequence();
        Leaf_Node walk_Node = new Leaf_Node(Walk,()=>walk.End());
        Leaf_Node walk_Condition_Node = new Leaf_Node(Distance_Walk_Condition);

        Sequence shot_Sequence_Node = new Sequence();
        Leaf_Node shot_Node = new Leaf_Node(Shot);
        Leaf_Node shot_Condition_Node = new Leaf_Node(Shot_Condition);


       // shot_Sequence_Node.AddChild(shot_Condition_Node);
       // shot_Sequence_Node.AddChild(shot_Node);
      //  behaviour.AddChild(shot_Sequence_Node);

        walk_Sequence_Node.AddChild(walk_Condition_Node);
        walk_Sequence_Node.AddChild(walk_Node);
        behaviour.AddChild(walk_Sequence_Node);

        
        //behaviour.AddChild(alert_Node);
        behaviour.AddChild(idle_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    public RESULT Distance_Walk_Condition()
    {
        if (walk.Condition(6.0f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Shot_Condition()
    {
        if (walk.Condition(4.0f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Shot()
    {
        if (activing_Func.Equals("Shot")) return RESULT.RUNNING;

        shot.Work(TYPE.BOSSBULLET);
        activing_Func = "Shot";
        //animator.SetTrigger("Walk");
        eBoss_State = BOSS_STATE.ATTACK;
        return RESULT.SUCCESS;
    }

    public RESULT Walk()
    {
        if (eBoss_State == BOSS_STATE.WALK) walk.Work();
        if (activing_Func.Equals("Walk")) return RESULT.RUNNING;
        walk.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Walk";
        animator.SetTrigger("Walk");
        eBoss_State = BOSS_STATE.WALK;
        Debug.Log("asd");
        return RESULT.SUCCESS;
    }
    
    public RESULT Alert()
    {
        if (eBoss_State == BOSS_STATE.ALERT) alert.Work();
        if (activing_Func.Equals("Alert")) return RESULT.RUNNING;
        alert.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Alert";
        animator.SetTrigger("Alert_L");
        eBoss_State = BOSS_STATE.ALERT;
        return RESULT.SUCCESS;
    }

    public RESULT Idle()
    {
        
        if (eBoss_State == BOSS_STATE.IDLE) idle.Work();
        if (activing_Func.Equals("Idle")) return RESULT.RUNNING;
        alert.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Idle";
        animator.SetTrigger("Idle");
        eBoss_State = BOSS_STATE.IDLE;
        return RESULT.SUCCESS;
    }

}
