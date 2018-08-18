using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SPECIAL_STATE { NONE, TURNONSPOT, DOUBLEJUMPLANDING, DANCE, AIM }

public class Player2 : MoveObject {

    private Transform playerTr;
    private Animator playerAni;
    private Move move;
    [SerializeField]
    private STATE eState ;
    private SPECIAL_STATE eSpecialState;
	void Start () {
        playerTr = GetComponent<Transform>();
        move = GetComponent<Move>();
        playerAni = GetComponent<Animator>();
        eState = STATE.STAND;
        eSpecialState = SPECIAL_STATE.NONE;



    }
	
	// Update is called once per frame
	void Update () {
        LookMousePoint();
        KeyBoardManual();
        MovePlayer();
        Running();
        TakeAim();

        Logic();
        Render();
        SpecialAnimation();
    }

    private void LookMousePoint()           //김병완이 구현한 마우스바라보기
    {
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장

        Vector3 pos = playerTr.position; //게임 오브젝트 좌표 저장
        Vector3 mpos2 = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);

        Vector3 aim1 = Camera.main.ScreenToWorldPoint(mpos2);

        float dx = aim1.x - pos.x;
        float dz = aim1.z - pos.z;

        float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;

        playerTr.rotation = Quaternion.Euler(0.0f, rotateDegree, 0.0f);

    }

    private void MovePlayer()           //이동값설정
    {
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }

    private void KeyBoardManual()       //키보드입력시 상태변경
    {
        if(Input.GetKey(KeyCode.W))
        {
            eState = STATE.WALK;

        }
        else
        {
            eState = STATE.STAND;
        }
    }

    private void Logic()            //클래식 방식으로 이름을 짜봄     논리
    {
        switch (eState)
        {
            case STATE.RUN:
                move.SetMoveSpeed(4.0f);
                break;
            case STATE.WALK:

                move.SetMoveSpeed(2.0f);
                break;
        }
    }

    private void Render()                //애니메이션
    {
        switch(eState)
        {
            case STATE.RUN:
                playerAni.SetBool("IsRun", true);
                playerAni.SetBool("IsWalk", false);
                break;
            case STATE.WALK:
                playerAni.SetBool("IsWalk", true);
                playerAni.SetBool("IsRun", false);
                break;

            case STATE.STAND:
                playerAni.SetBool("IsRun", false);
                playerAni.SetBool("IsWalk", false);
                break;
        }
    }


    //달리기
    private void Running()          //달리기는 쉬프트
    {
        if (Input.GetKey(KeyCode.LeftShift) && eState == STATE.WALK)
        {
            eState = STATE.RUN;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && eState == STATE.RUN)
        {
            eState = STATE.WALK;
        }
    }


    //조준
    private void TakeAim()
    {
        if (Input.GetMouseButtonDown(Define.MOUSE_RIGHT_BUTTON))
        {
            eSpecialState = SPECIAL_STATE.AIM;
        }
        else if(Input.GetMouseButtonUp(Define.MOUSE_RIGHT_BUTTON))
        {
            eSpecialState = SPECIAL_STATE.NONE;
        }
    }

    //특별한 애니메이션
    private void SpecialAnimation()
    {
        if(eSpecialState == SPECIAL_STATE.AIM)
        {
            playerAni.SetBool("IsAim", true);
        }
        else
        {
            playerAni.SetBool("IsAim", false);
        }
    }
}
