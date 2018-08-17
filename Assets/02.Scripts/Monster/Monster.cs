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

    void Start()
    {
        base.Setting();
        hp.SettingHp(100);

        eState = STATE.STAND;
        monsterAni = GetComponent<Animator>();
        moveAgent = GetComponent<MoveAgent>();

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
           
            case STATE.WALK:
                monsterAni.SetBool("IsMove", true);
                break;
            case STATE.STAND:
                monsterAni.SetBool("IsMove", false);
                break;
        }
    }

    IEnumerator CheckPlayerDistance()
    {
        //float distance = Check.Distance(system.pPlayer.transform, this.transform);
        //if(distance < 5.0f)
        //{

        //}
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(6.0f);
        if (moveAgent.pPatrolling == false) yield return null;
        monsterAni.SetTrigger("IsSpecialIdle");
        moveAgent.pPatrolling = false;
        moveAgent.StopNavi();
        eState = STATE.STAND;
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
