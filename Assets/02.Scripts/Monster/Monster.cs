using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MoveObject
{
    GameSystem system;

    Animator monsterAni;
    MoveAgent moveAgent;
    [SerializeField]
    private float m_dis;

    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE eState = STATE.STAND;

    [SerializeField]
    private readonly float patrolSpeed = 1.5f;
    [SerializeField]
    private readonly float traceSpeed = 2.5f;

    void Start()
    {
        base.Setting();
        hp.SettingHp(100);

        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

        eState = STATE.STAND;
        monsterAni = GetComponent<Animator>();
        moveAgent = GetComponent<MoveAgent>();
        moveAgent.Init();
        moveAgent.DataInput(patrolSpeed, traceSpeed);
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
        if(moveAgent.pPatrolling == true)
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
        float distance = Check.Distance(system.pPlayer.transform, this.transform);
        if(distance < 10.0f)
        {
            moveAgent.pPatrolling = false;
            moveAgent.pTraceTarget = system.pPlayer.transform.position;
            eState = STATE.RUN;
        }
        else if(distance < 5.0f)
        {
            moveAgent.pPatrolling = false;
            eState = STATE.ATTACK;
        }
        else
        {
            moveAgent.pPatrolling = true;
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckPlayerDistance());
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(6.0f);
        if (moveAgent.pPatrolling == false) yield return null;
        eState = STATE.STAND;
        monsterAni.SetTrigger("IsSpecialIdle");
        moveAgent.pPatrolling = false;
        moveAgent.StopNavi();
        yield return new WaitForSeconds(5.0f);
        ResetState();
    }

    void ResetState()
    {
        eState = STATE.WALK;
        moveAgent.pPatrolling = true;
        moveAgent.StartNavi();
        StartCoroutine(SpecialIdle());
    }
}
