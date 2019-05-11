using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Alien_Gun : Enemy
{
    BehaviorTree bt;
    public int vitality = 5;   // 체력
    public int damage = 5;
    // Start is called before the first frame update

    Attack attack;
    Trace trace;

    [SerializeField]
    string activing_Func = string.Empty;
    [SerializeField]
    bool isOther_State_Change = false;
    Rigidbody rb;
    Slider healthSlider;

    public GameObject hitEffect;
    public Transform hitPos;

    int bulletCount = 15;
    private void Awake()
    {
        base.Init();
       
    }
    void Start()
    {
        PrefabSystem.instance.allMonster.Add(gameObject);
        attack = GetComponent<Attack>();
        trace = GetComponent<Trace>();
        trace.Init_Target(system.pPlayer2);
        attack.Init_Target(system.pPlayer2);
        trace.Setting(agent, 1.0f);
        attack.Setting(agent, 5);

        rb = GetComponent<Rigidbody>();
        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = 5;
        healthSlider.value = 5;

        animator.SetTrigger("isRun");
        eEnemy_State = ENEMY_STATE.RUN;
        Build_BT();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(activing_Func);
        if (eEnemy_State == ENEMY_STATE.DIE)
        {
            isOther_State_Change = true;
            agent.isStopped = true;
            return;
        }
        if (isOther_State_Change == false)
        {
            bt.Run();
        }
        if (eEnemy_State == ENEMY_STATE.ATTACK)      //임시방편
        {
            Rotate();
        }
        // Debug.Log(activing_Func);
    }

    void Rotate()
    {
        attack.Work_Dir();
    }
   


    void Build_BT()
    {
        Selector root = new Selector();

        Selector behaviour = new Selector();

        Sequence attack_Sequence_Node = new Sequence();

        Sequence walk_Sequence_Node = new Sequence();

        Leaf_Node trace_Node = new Leaf_Node(Trace);
        Leaf_Node attack_Node = new Leaf_Node(Attack);
        Leaf_Node attack_Condition_Node = new Leaf_Node(Distance_Attack_Condition);
        Leaf_Node walk_Node = new Leaf_Node(Walk);
        Leaf_Node walk_Condition_Node = new Leaf_Node(Distance_Walk_Condition);

        walk_Sequence_Node.AddChild(walk_Condition_Node);
        walk_Sequence_Node.AddChild(walk_Node);


        Sequence reload_Sequence_Node = new Sequence();
        Leaf_Node satisfy_bullet_Node = new Leaf_Node(Satisfy_Bullet);
        Leaf_Node reload_Node = new Leaf_Node(Reload);

       

        Selector act_Selector = new Selector();

        act_Selector.AddChild(reload_Sequence_Node);
        act_Selector.AddChild(attack_Sequence_Node);
        reload_Sequence_Node.AddChild(satisfy_bullet_Node);
        reload_Sequence_Node.AddChild(reload_Node);
        attack_Sequence_Node.AddChild(attack_Condition_Node);
        attack_Sequence_Node.AddChild(attack_Node);

        behaviour.AddChild(act_Selector);
        behaviour.AddChild(walk_Sequence_Node);
        behaviour.AddChild(trace_Node);
        root.AddChild(behaviour);
        bt = new BehaviorTree(root);
    }

    public RESULT Satisfy_Bullet()
    {
        if (bulletCount > 0) return RESULT.FAIL;
        return RESULT.SUCCESS;
    }
    public RESULT Reload()
    {
        if (activing_Func.Equals("Reload")) return RESULT.RUNNING;
        activing_Func = "Reload";
        animator.SetTrigger("isReload");
        bulletCount = 15;
        eEnemy_State = ENEMY_STATE.RELOAD;
        isOther_State_Change = true;
        agent.isStopped = true;
        return RESULT.SUCCESS;
    }

    public RESULT Walk()
    {
        if (eEnemy_State == ENEMY_STATE.WALK) trace.Work();
        if (activing_Func.Equals("Walk")) return RESULT.RUNNING;
        trace.Init(0.25f);               
        activing_Func = "Walk";
        animator.SetTrigger("isWalk");
        eEnemy_State = ENEMY_STATE.WALK;
        agent.isStopped = false;
        return RESULT.SUCCESS;
    }

    public RESULT Trace()
    {
        if (eEnemy_State == ENEMY_STATE.RUN) trace.Work();
        if (activing_Func.Equals("Trace")) return RESULT.RUNNING;
        trace.Init();       //매개변수가 없으면 기본셋팅속도로 추적
        activing_Func = "Trace";
        animator.SetTrigger("isRun");
        eEnemy_State = ENEMY_STATE.RUN;
        agent.isStopped = false;
        return RESULT.SUCCESS;
    }

    public RESULT Attack()
    {
        if (activing_Func.Equals("Attack")) return RESULT.RUNNING;
        attack.Init();
        attack.Work();
        bulletCount--;
        activing_Func = "Attack";
        animator.SetTrigger("isAttack");
        eEnemy_State = ENEMY_STATE.ATTACK;

        isOther_State_Change = true;                //다른 상태로 바꿀수 없다
        return RESULT.SUCCESS;
    }

    public RESULT Distance_Attack_Condition()
    {
        if (trace.Condition(4.0f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }

    public RESULT Distance_Walk_Condition()
    {
        if (trace.Condition(5.5f)) return RESULT.SUCCESS;
        return RESULT.FAIL;
    }
    
    void Exit_Attack()          //장전,공격이 끝나면 이함수 호출
    {
        agent.isStopped = false;
        activing_Func = string.Empty;
        isOther_State_Change = false;
    }

    new void Die()
    {
        activing_Func = "Death";

        animator.SetTrigger("isDeath");

        eEnemy_State = ENEMY_STATE.DIE;
        isOther_State_Change = true;
        agent.isStopped = true;
        base.Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (vitality <= 0) return;
        if (other.CompareTag("Bullet"))
        {
            vitality--;
            healthSlider.value -= 1;
            other.gameObject.SetActive(false);
            GameObject effect = Instantiate(hitEffect, hitPos.position, Quaternion.identity);    // 피격 이펙트 동적 생성
            Destroy(effect, 2.0f);  // 1초후 삭제
            if (vitality <= 0)
            {
                Die();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

    }
    public void WoundExplosionDamage()     //시간이없으니까 컴포넌트대신 함수로
    {
        if (vitality <= 0) return;
        vitality -= 2;
        healthSlider.value -= 2;
        Debug.Log("맞음");
        GameObject effect = Instantiate(hitEffect, hitPos.position, Quaternion.identity);    // 피격 이펙트 동적 생성
        Destroy(effect, 2.0f);  // 1초후 삭제
        if (vitality <= 0)
        {
            Die();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
