using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SPECIAL_STATE { NONE, TURNONSPOT, DOUBLEJUMPLANDING, DANCE, AIM }

public class Player2 : MoveObject {

    private Transform playerTr;
    private Animator playerAni;
    private Move move;
    [SerializeField]
    private STATE eState;           public string TempStateReturn() { return eState.ToString(); }
    private STATE ePreState;
    private SPECIAL_STATE eSpecialState;

    private bool isRollDelay;
    private bool isSpecialState = false;
	void Start () {
        playerTr = GetComponent<Transform>();
        move = GetComponent<Move>();
        playerAni = GetComponent<Animator>();
        eState = STATE.STAND;
        ePreState = STATE.STAND;
        eSpecialState = SPECIAL_STATE.NONE;

        isRollDelay = false;
        isSpecialState = false;
       

    }

    // Update is called once per frame
    void Update () {
        LookMousePoint();
        KeyBoardManual();
        MouseManual();
        MovePlayer();
        Running();
        TakeAim();
        Rolling();

        Logic();
        BlendAnimation();
        Render();
        SpecialAnimation();
    }

    //마우스 바라보기
    private void LookMousePoint()           
    {
        if (eState == STATE.ROLL) return;
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장

        Vector3 pos = playerTr.position; //게임 오브젝트 좌표 저장
        Vector3 mpos2 = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);

        Vector3 aim1 = Camera.main.ScreenToWorldPoint(mpos2);

        float dx = aim1.x - pos.x;
        float dz = aim1.z - pos.z;

        float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;
        playerTr.rotation = Quaternion.Euler(0.0f, rotateDegree, 0.0f);
    }

    //이동값설정
    private void MovePlayer()           
    {
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }
    private void KeyBoardManual()       //키보드입력시 상태변경
    {
        if (eState == STATE.ROLL) return;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            eState = STATE.WALK;
        }
        else
        {
            eState = STATE.STAND;
        }
    }
    //클래식 방식으로 이름을 짜봄     논리
    private void Logic()           
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

    //애니메이션
    private void Render()                
    {
        //if (isSpecialState) return;
        switch (eState)
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

    //애니메이터 컨트롤러 해시값 추출    
    private readonly int hashAngle = Animator.StringToHash("Angle");
    private readonly int hashX = Animator.StringToHash("X");
    private readonly int hashZ = Animator.StringToHash("Z");
    //블랜드 애니메이션
    private void BlendAnimation()
    {
        //해시에 이동 계수 전달
        playerAni.SetFloat(hashAngle, playerTr.rotation.y);
        playerAni.SetFloat(hashX, move.Horizontal);
        playerAni.SetFloat(hashZ, move.Vertical);
    }

    //구르기
    private void Rolling()
    {
        if(Input.GetKeyUp(KeyCode.W))
        {
            if (isSpecialState) return;
            isRollDelay = true;
            Invoke("RollingCancel", 0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.W) && isRollDelay == true)
        {
            playerAni.SetTrigger("IsRoll");
            isSpecialState = true;
            isRollDelay = false;
            ePreState = eState;
            eState = STATE.ROLL;
            Invoke("RollingReset", 2.0f);
        }
    }
    private void RollingExit(){ eState = STATE.WALK ; }
    private void RollingCancel() {  isRollDelay = false; }
    private void RollingReset() { isSpecialState = false; }


    //기본공격
    private void MouseManual()
    {
        if (Input.GetMouseButtonDown(Define.MOUSE_LEFT_BUTTON))
        {
            playerAni.Play("Attack");
        }
    }
    private void AttackBasicExit()
    {

    }
}
