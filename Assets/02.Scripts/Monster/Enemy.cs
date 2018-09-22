using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY_STATE { NONE, PATROL, TRACE ,ATTACK ,RETRACE}

public abstract class Enemy : MoveObject {

    protected GameSystem system;
    protected Animator enemyAni;

    [SerializeField]
    protected ENEMY_STATE eEnemy_State = ENEMY_STATE.NONE;
    protected void Init()        //하위에서 호출해야함       //그리고 웬만하면 모든 멤버변수를 여기서 초기화해야함
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        enemyAni = GetComponent<Animator>();
    }

    public delegate ENEMY_STATE GetTraceState( ENEMY_STATE _Enemy_State );

 
 
}
