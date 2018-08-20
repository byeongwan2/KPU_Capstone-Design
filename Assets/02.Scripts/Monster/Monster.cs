using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ENEMY_STATE {NONE,PATROL,TRACE }

public partial class Monster : Enemy
{

    Shot bulletShot;
    MoveAgent moveAgent;
    [SerializeField]
    private float m_dis;

    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE eState = STATE.STAND;
    [SerializeField]
    private ENEMY_STATE eEnemy_State = ENEMY_STATE.NONE;
    [SerializeField]
    private readonly float patrolSpeed = 1.5f;
    [SerializeField]
    private readonly float traceSpeed = 2.5f;


    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");

    void Start()
    {
        base.Setting();
        hp.SettingHp(100);

        base.Init();

        eEnemy_State = ENEMY_STATE.PATROL ;
        eState = STATE.STAND;
        
        moveAgent = GetComponent<MoveAgent>();
        moveAgent.Init();
        moveAgent.DataInput(patrolSpeed, traceSpeed);
        moveAgent.pPatrolling = true;

        bulletShot = GetComponent<Shot>();
        bulletShot.Init("Bullet", 8, 100.0f, 2);


        StartCoroutine(CheckPlayerDistance());
        m_dis = 5.0f;
        StartCoroutine(SpecialIdle());
    }

    void Update()
    {
        LogicState();
        LogicAnimation();
    }
    void OnTriggerEnter(Collider _obj)
    {
        if(_obj.tag == "Bullet")
        {
            _obj.gameObject.SetActive(false);
            Debug.Log("총알이 적과부딪힘");
            if(hp.getHp() <= 0)
            {
                eState = STATE.DIE;
            }
        }
    }
    

    void LogicState()
    {
        if(eEnemy_State == ENEMY_STATE.PATROL)
        {
            eState = STATE.WALK;
            moveAgent.pPatrolling = true;
        }
    }

    void LogicAnimation()
    {
        switch(eState)
        {
            case STATE.DIE:

                enemyAni.SetTrigger("Die");
                enemyAni.SetInteger(hashDieIdx, Random.Range(0, 6));
            break;

            case STATE.ATTACK:
                enemyAni.SetBool("IsAttack", true);
                enemyAni.SetBool("IsRun", false);
                break;
            case STATE.RUN:
                enemyAni.SetBool("IsRun", true);
                enemyAni.SetBool("IsMove", false);
                enemyAni.SetBool("IsAttack", false);
                break;
            case STATE.WALK:
                enemyAni.SetBool("IsMove", true);
                enemyAni.SetBool("IsRun", false);
                break;
            case STATE.STAND:
                enemyAni.SetBool("IsAttack", false);
                enemyAni.SetBool("IsMove", false);
                enemyAni.SetBool("IsRun", false);
                break;
        }
    }
    public GameObject obj;
    private IEnumerator CheckPlayerDistance()
    {
        float distance = Check.Distance(obj.transform, this.transform);
        if(distance < 10.0f && distance >= 5.0f)
        {
            moveAgent.pTraceTarget = obj.transform.position;
            eState = STATE.RUN;
            eEnemy_State = ENEMY_STATE.TRACE;
        }
        else if(distance < 5.0f)
        {
            eState = STATE.ATTACK;
            eEnemy_State = ENEMY_STATE.TRACE;
            moveAgent.Stop();
        }
        else
        {
            if(eState == STATE.ATTACK || eEnemy_State == ENEMY_STATE.TRACE)
            {
                eEnemy_State = ENEMY_STATE.PATROL;
            }
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckPlayerDistance());
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(5.0f);
        if (eEnemy_State == ENEMY_STATE.PATROL)
        {
            moveAgent.Stop();
            enemyAni.SetTrigger("IsSpecialIdle");
            eState = STATE.STAND;
            eEnemy_State = ENEMY_STATE.NONE;
        }
        yield return new WaitForSeconds(3.0f);
        if (eEnemy_State == ENEMY_STATE.NONE)  eEnemy_State = ENEMY_STATE.PATROL; 
        StartCoroutine(SpecialIdle());
    }
    
}
