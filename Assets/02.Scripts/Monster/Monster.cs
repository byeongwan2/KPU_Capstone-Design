using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ENEMY_STATE {NONE,PATROL,TRACE }

public partial class Monster : MoveObject
{
    GameSystem system;

    Shot bulletShot;
    Animator monsterAni;
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
            Debug.Log(hp.getHp());
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
            case STATE.ATTACK:
                monsterAni.SetBool("IsAttack", true);
                monsterAni.SetBool("IsRun", false);
                break;
            case STATE.RUN:
                monsterAni.SetBool("IsRun", true);
                monsterAni.SetBool("IsMove", false);
                break;
            case STATE.WALK:
                monsterAni.SetBool("IsMove", true);
                monsterAni.SetBool("IsRun", false);
                break;
            case STATE.STAND:
                monsterAni.SetBool("IsAttack", false);
                monsterAni.SetBool("IsMove", false);
                monsterAni.SetBool("IsRun", false);
                break;
        }
    }
    public GameObject obj;
    private IEnumerator CheckPlayerDistance()
    {
        float distance = Check.Distance(obj.transform, this.transform);
        Debug.Log(distance);
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
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckPlayerDistance());
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(5.0f);
        if (eEnemy_State == ENEMY_STATE.PATROL)
        {
            moveAgent.Stop();
            monsterAni.SetTrigger("IsSpecialIdle");
            eState = STATE.STAND;
            eEnemy_State = ENEMY_STATE.NONE;
        }
        yield return new WaitForSeconds(3.0f);
        if (eEnemy_State == ENEMY_STATE.NONE)  eEnemy_State = ENEMY_STATE.PATROL; 
        StartCoroutine(SpecialIdle());
    }
    
}
