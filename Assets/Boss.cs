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
    Return returnWalk;
    /// ////////////////////필수기능
    Behaviour haviour;
    Animator animator;
    Rigidbody rb;
    public Animator weaponAnimator;
    [SerializeField]
    bool isOther_State_Change = false;

    [SerializeField]
    public GameObject shotSecondPosition ;

    public void OffOther_State_Change() { isOther_State_Change = false; }
    void Start()
    {

        vitality = 100;

        //attack = GetComponent<Attack>();
        walk = GetComponent<Walk>();    //보스의 경우 walk는 사실상 trace에서 네비게이션을 뺸것
        walk.Init_Target(PrefabSystem.instance.player.transform.position);
        alert = GetComponent<Alert>();
        idle = GetComponent<Idle>();
        shot = GetComponent<Shot>();
        returnWalk = GetComponent<Return>();
        returnWalk.Init(transform.position);
        shot.Init("BossBullet", 5, 2, 3, TYPE.BOSSBULLET, Object_Id.MONSTER);     //상태가 바뀔때 한번만 호출되어야함
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weaponAnimator = transform.Find("ROOT/Pelvis/Top/Mount_top/Cockpit_Gun/Mount_Weapon_Main/Weapon_Shock_Rifle_lvl2/BossWeapon").gameObject.GetComponent<Animator>();
       
        Build_BT();
        haviour = idle;
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

        Sequence alert_Sequence_Node = new Sequence();
        Leaf_Node alert_Node = new Leaf_Node(Alert);
        Leaf_Node alert_Condition_Node = new Leaf_Node(Alert_Condition);

        Leaf_Node idle_Node = new Leaf_Node(Idle);

        Sequence walk_Sequence_Node = new Sequence();
        Leaf_Node walk_Node = new Leaf_Node(Walk,()=>walk.End());       //End 현재의미없음
        Leaf_Node walk_Condition_Node = new Leaf_Node(Walk_Condition);

        Sequence shot_Sequence_Node = new Sequence();
        Leaf_Node shot_Node = new Leaf_Node(Shot);
        Leaf_Node shot_Condition_Node = new Leaf_Node(Shot_Condition);

        Sequence return_Sequence_Node = new Sequence();
        Leaf_Node return_Node = new Leaf_Node(Return);
        Leaf_Node return_Condition_Node = new Leaf_Node(Return_Condition);


        shot_Sequence_Node.AddChild(shot_Condition_Node);
        shot_Sequence_Node.AddChild(shot_Node);
        behaviour.AddChild(shot_Sequence_Node);

        alert_Sequence_Node.AddChild(alert_Condition_Node);
        alert_Sequence_Node.AddChild(alert_Node);
        behaviour.AddChild(alert_Sequence_Node);


        walk_Sequence_Node.AddChild(walk_Condition_Node);
        walk_Sequence_Node.AddChild(walk_Node);
        behaviour.AddChild(walk_Sequence_Node);

        return_Sequence_Node.AddChild(return_Condition_Node);
        return_Sequence_Node.AddChild(return_Node);
        behaviour.AddChild(return_Sequence_Node);



        //behaviour.AddChild(alert_Node);
        behaviour.AddChild(idle_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }

    RESULT Return_Condition()
    {
        if (returnWalk.Condition(16.0f))
        {
            return RESULT.SUCCESS;

        }
        return RESULT.FAIL;
    }


    RESULT Return()
    {
        if (activing_Func.Equals("Return")) return RESULT.RUNNING;  //문자열은 기능이아니라 행동트리관련이기때문에 Return인게맞음
        haviour.End();
        haviour = returnWalk;
        activing_Func = "Return";
        animator.SetTrigger("Walk");
        eBoss_State = BOSS_STATE.RETURN;
        return RESULT.SUCCESS;
    }

    RESULT Alert_Condition()
    {
        float dis;
        if (haviour != alert)
            dis = 4.0f;
        else dis = 6.0f;
        if (alert.Condition(dis, PrefabSystem.instance.player.transform))
        
            return RESULT.SUCCESS;



        return RESULT.FAIL;
    }

    RESULT Walk_Condition()
    {
        if (16.0f > Check.Distance(returnWalk.startPosition, transform.position))
        {
            if (walk.Condition(PrefabSystem.instance.player.transform.position, 10.0f))
            {

                return RESULT.SUCCESS;

            }
            return RESULT.FAIL;
        }
        return RESULT.FAIL; 
    }

    RESULT Shot_Condition()
    {
        if (alert.Condition(7.0f, PrefabSystem.instance.player.transform))
        {
            if (alert.IsShotDelay)
            {

                return RESULT.SUCCESS;

            }

        }
        
        return RESULT.FAIL;
    }

    RESULT Shot()
    {
        if (activing_Func.Equals("Shot")) return RESULT.RUNNING;
        isOther_State_Change = true;
        haviour.End();
        haviour = shot;
        
        activing_Func = "Shot";
        weaponAnimator.SetTrigger("Ready");
        animator.SetTrigger("Idle");
        eBoss_State = BOSS_STATE.ATTACK;
        return RESULT.SUCCESS;
    }
    public void WorkShot()
    {
        shot.Work(TYPE.BOSSBULLET);
    }

    RESULT Walk()
    {
        if (eBoss_State == BOSS_STATE.WALK) walk.Work();
        if (activing_Func.Equals("Walk")) return RESULT.RUNNING;
        haviour.End();
        haviour = walk;
        walk.Init();      //상태가 바뀔때 한번만 호출되어야함

        activing_Func = "Walk";
        animator.SetTrigger("Walk");
        eBoss_State = BOSS_STATE.WALK;
        return RESULT.SUCCESS;
    }
    
    RESULT Alert()
    {
        if (eBoss_State == BOSS_STATE.ALERT) alert.Work();
        if (activing_Func.Equals("Alert")) return RESULT.RUNNING;
        haviour.End();
        haviour = alert;
        alert.Init();      //상태가 바뀔때 한번만 호출되어야함
        activing_Func = "Alert";
        animator.SetTrigger("Alert_L");
        eBoss_State = BOSS_STATE.ALERT;
        return RESULT.SUCCESS;
    }


    RESULT Idle()
    {
        
        if (eBoss_State == BOSS_STATE.IDLE) idle.Work();
        if (activing_Func.Equals("Idle")) return RESULT.RUNNING;
        haviour.End();
        haviour = idle;
        activing_Func = "Idle";
        animator.SetTrigger("Idle");
        eBoss_State = BOSS_STATE.IDLE;
        return RESULT.SUCCESS;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (vitality <= 0) return;
        if (other.gameObject.CompareTag("Bullet"))
        {
            vitality--;
            other.gameObject.SetActive(false);
           

            if (vitality <= 0)
            {
                haviour.End();
                animator.SetTrigger("Death");
                rb.isKinematic = true;

                var col = GetComponent<BoxCollider>();
                col.enabled = true;
                isOther_State_Change = true;

            }
        }

    }
   
}
