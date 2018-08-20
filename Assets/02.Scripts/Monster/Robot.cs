using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy {

    private Transform tr;
    private Around around;
    private STATE eState = STATE.STAND;
    private Move move;
	void Start()
    {
        //hp

        base.Init();
        base.Setting();

        eState = STATE.STAND;
        tr = GetComponent<Transform>();
        move = GetComponent<Move>();
        around = GetComponent<Around>();

        move.SetMoveSpeed(4.0f);
    }

    void Update()
    {
        Direction();
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
        if (around.IsArounding())   eState = STATE.WALK; 
    }

    private void Direction()        //차후 일반화 시킬필요가있음 기초든 컴포넌트든
    {
       tr.rotation = Quaternion.Euler(0.0f, around.GetDirection(tr.position), 0.0f);
    }
   
}
