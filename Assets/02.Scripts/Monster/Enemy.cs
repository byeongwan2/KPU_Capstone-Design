using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public enum ENEMY_STATE { IDLE, WALK, RUN ,ATTACK ,RETRACE,ROLL,LOOKAROUND,DIE,RELOAD,ROTATE}
public enum BOSS_STATE {  IDLE,ALERT,ATTACK,DIE}
//적군들은 모두 상속받아야 하는 클래스     애니메이션,충돌체, 네비게이션등을 포함함
public abstract class Enemy : MoveObject
{
    Vector3 newPosition;
    protected GameSystem system;
    protected Animator animator;
    protected NavMeshAgent agent;
    protected Collider col;
    [SerializeField]
    protected ENEMY_STATE eEnemy_State = ENEMY_STATE.IDLE;

  
    public ENEMY_STATE Get_State()
    {
        return eEnemy_State;
    }
    protected void Init()        //하위에서 호출해야함       //그리고 웬만하면 모든 멤버변수를 여기서 초기화해야함
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        col = GetComponent<Collider>();
    }
  
    public void Set_NewPosition(Vector3 _vec)
    {
        transform.position = _vec;
    }

    public delegate ENEMY_STATE GetTraceState( ENEMY_STATE _Enemy_State );

    protected void Die()
    {
        col.enabled = false;
        agent.radius = 0;
        agent.height = 0;
    }

   
}
