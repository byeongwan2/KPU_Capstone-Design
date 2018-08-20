using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ENEMY_STATE {NONE,PATROL,TRACE }

public class Monster : MoveObject
{
    GameSystem system;

    Animator monsterAni;
    MoveAgent moveAgent;
    [SerializeField]
    private float m_dis;

    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE eState = STATE.STAND;
    private ENEMY_STATE eEnemy_State = ENEMY_STATE.NONE;
    [SerializeField]
    private readonly float patrolSpeed = 1.5f;
    [SerializeField]
    private readonly float traceSpeed = 2.5f;

    void Start()
    {
        base.Setting();
        hp.SettingHp(100);

        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

        eEnemy_State = ENEMY_STATE.PATROL ;
        eState = STATE.STAND;
        monsterAni = GetComponent<Animator>();
        moveAgent = GetComponent<MoveAgent>();
        moveAgent.Init();
        moveAgent.DataInput(patrolSpeed, traceSpeed);
        moveAgent.pPatrolling = true;
        StartCoroutine(CheckPlayerDistance());
        m_dis = 5.0f;
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
            Debug.Log(hp.getHp());
        }
    }
    

    void LogicState()
    {
        if(eEnemy_State == ENEMY_STATE.PATROL)
        {
            if(eState != STATE.WALK)StartCoroutine(SpecialIdle());
            eState = STATE.WALK;
           
        }
    }

    void LogicAnimation()
    {
        switch(eState)
        {
            case STATE.ATTACK:

                break;
            case STATE.RUN:
                monsterAni.SetBool("IsRun", true);
                break;
            case STATE.WALK:
                monsterAni.SetBool("IsMove", true);
                break;
            case STATE.STAND:
                monsterAni.SetBool("IsMove", false);
                monsterAni.SetBool("IsRun", false);
                break;
        }
    }

    private IEnumerator CheckPlayerDistance()
    {
        float distance = Check.Distance(system.pPlayer2.transform, this.transform);
        if(distance < 2.0f)
        {
            moveAgent.pTraceTarget = system.pPlayer2.transform.position;
            eState = STATE.RUN;
            eEnemy_State = ENEMY_STATE.TRACE;
            moveAgent.StopNavi();
        }
        else if(distance < 1.0f)
        {
            eState = STATE.ATTACK;
        }
        else
        {
            
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckPlayerDistance());
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(5.0f);
        if (eEnemy_State != ENEMY_STATE.PATROL) yield return null;
        eState = STATE.STAND;
        eEnemy_State = ENEMY_STATE.NONE;
        monsterAni.SetTrigger("IsSpecialIdle");
        moveAgent.StopNavi();
        yield return new WaitForSeconds(5.0f);
        ResetState();
    }

    void ResetState()
    {
        eState = STATE.WALK;
        moveAgent.StartNavi();
        StartCoroutine(SpecialIdle());
    }
}
