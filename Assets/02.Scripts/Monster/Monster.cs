using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Monster : Enemy
{
    Shot bulletShot;

    [SerializeField]
    private readonly float patrolSpeed = 1.5f;
    [SerializeField]
    private readonly float traceSpeed = 2.5f;


    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");

    bool isDie;
    void Start()
    {
        hp = 100;

        base.Init();

        eEnemy_State = ENEMY_STATE.PATROL ;
        eState = STATE.STAND;
        
        bulletShot = GetComponent<Shot>();
        //bulletShot.Init("Bullet", 8, 100.0f, 2,TYPE.BULLET);

        StartCoroutine(SpecialIdle());          //걷다가 쉬었다가기

        StartCoroutine(CheckPlayerDistance());
        isDie = false;

    }

    void Update()
    {
        LogicState();
        Render();
        UpdateRotateTarget();
    }

    void LogicState()
    {
        if (isDie) return;
        if(eEnemy_State == ENEMY_STATE.PATROL)
        {
            eState = STATE.WALK;
        }
    }

    void Render()
    {

        switch(eState)
        {
            case STATE.DIE:
                if (isDie) return;
                animator.SetTrigger("Die");
                animator.SetInteger(hashDieIdx, Random.Range(0, 6));
                isDie = true;
            break;

            case STATE.ATTACK:
                animator.SetBool("IsAttack", true);
                animator.SetBool("IsRun", false);
                break;
            case STATE.RUN:
                animator.SetBool("IsRun", true);
                animator.SetBool("IsMove", false);
                animator.SetBool("IsAttack", false);
                break;
            case STATE.WALK:
                animator.SetBool("IsMove", true);
                animator.SetBool("IsRun", false);
                break;
            case STATE.STAND:
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsMove", false);
                animator.SetBool("IsRun", false);
                break;
        }
    }

    void TracePlayer()
    {
        eState = STATE.RUN;
        eEnemy_State = ENEMY_STATE.TRACE;
    }

    IEnumerator CheckPlayerDistance()
    {
        float distance = Check.Distance(system.pPlayer2.transform, this.transform);
        if(distance < 10.0f && distance >= 5.0f )
        {
            TracePlayer();
        }
        else if(distance < 5.0f)
        {
            eState = STATE.ATTACK;
            eEnemy_State = ENEMY_STATE.ATTACK;
           
        }
        else
        {
            if(eEnemy_State == ENEMY_STATE.ATTACK)
            {
                eEnemy_State = ENEMY_STATE.PATROL;
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (isDie) yield break;
        StartCoroutine(CheckPlayerDistance());
    }

    IEnumerator SpecialIdle()
    {
        yield return new WaitForSeconds(5.0f);
        if (isDie) yield break;
        if (eEnemy_State == ENEMY_STATE.PATROL)
        {
            animator.SetTrigger("IsSpecialIdle");
            eState = STATE.STAND;
            eEnemy_State = ENEMY_STATE.NONE;
        }
        yield return new WaitForSeconds(3.0f);
        if (isDie)yield break;
        if (eEnemy_State == ENEMY_STATE.NONE)  eEnemy_State = ENEMY_STATE.PATROL; 
        StartCoroutine(SpecialIdle());
    }

    void UpdateRotateTarget()
    {
        if (eEnemy_State == ENEMY_STATE.TRACE || eEnemy_State == ENEMY_STATE.ATTACK)
        {
            float dx = system.pPlayer2.transform.position.x - transform.position.x;
            float dz = system.pPlayer2.transform.position.z - transform.position.z;

            float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 5.0f);
        }
    }
}
