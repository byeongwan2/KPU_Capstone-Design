using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy {
    private MoveAgent agent;
    private Transform tr;
    private STATE eState = STATE.STAND;
    private Move move;

    private readonly string str = "RobotWayPoint";
	void Start()
    {
        //hp

        base.Init();
        base.Setting();

        eState = STATE.STAND;
        tr = GetComponent<Transform>();
        move = GetComponent<Move>();
        agent = GetComponent<MoveAgent>();
        agent.Init(str, 2.0f, 3.0f);
        move.SetMoveSpeed(4.0f);
    }

    void Update()
    {
        Logic();
        Render();
    }

    void Render()
    {
        switch(eState)
        {
            case STATE.WALK:
                enemyAni.SetBool("IsMove", true);
                break;

            case STATE.STAND:
                enemyAni.SetBool("IsMove", false);
                break;
        }
    }

    private void Logic()
    {
        if (agent.pPatrolling)   eState = STATE.WALK; 
    }

   
}
