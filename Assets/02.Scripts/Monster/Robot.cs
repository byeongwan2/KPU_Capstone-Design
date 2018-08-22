using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy {
    enum UP_BODY_STATE { THROW,SHOT}
    UP_BODY_STATE eMotionState;
    private MoveAgent agent;
    private Transform tr;
    private Move move;

    private readonly string str = "RobotWayPoint";
	void Start()
    {
        //hp

        base.Init();
        base.Setting();

        SettingPlayerDistance(10.0f, 5.0f);

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
        MotionRender();
        Render();
     
    }

    void Render()
    {
        switch (eState)
        {
            case STATE.WALK:
              //  enemyAni.SetBool("IsAttack", true);
                enemyAni.SetBool("IsMove", true);
                break;

            case STATE.STAND:
                enemyAni.SetBool("IsMove", false);
                break;
        }
    }

    void MotionRender()
    {
        switch(eMotionState)
        {
            case UP_BODY_STATE.SHOT:
                enemyAni.SetBool("IsAttack",true);
                break;
            case UP_BODY_STATE.THROW:

                break;
        }
    }

    private void Logic()
    {
        if (agent.pPatrolling)   eState = STATE.WALK; 


    }

    
   
}
