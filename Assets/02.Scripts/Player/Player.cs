using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SPECIAL_STATE {  NONE,TURNONSPOT , DOUBLEJUMPLANDING, DANCE}

public partial class Player : MoveObject
{

    private Transform playerTr;
    private Rigidbody playerRb;
    private Animator playerAni;
    private Jump jump;
    private Move move;
    private Throw bombThrow;
    private Shot bulletShot;
    private ShortAttack shortAttack;
    public float rotSpeed = 250.0f; //회전 속도

    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE eState = STATE.STAND;
    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE ePreState = STATE.STAND;
    [SerializeField]
    private SPECIAL_STATE eSpecialState = SPECIAL_STATE.NONE;
    private WAY eWay = WAY.FORWARD;

    [SerializeField]
    private float bombPower = 15.0f;

    private int MAXPLAYERBOMBCOUNT = 10;
    private int MAXPLAYERBULLETCOUNT = 40;

    private int shotDamage = 10;
    //절대 바뀌지않는 초기화//컴포넌트관련내용만

    void Start()
    {
        eState = STATE.STAND;
        ePreState = STATE.STAND;
        eWay = WAY.FORWARD;
        playerRb = GetComponent<Rigidbody>();
        playerTr = GetComponent<Transform>(); //Player Transform 컴포넌트 할당
        playerAni = GetComponent<Animator>();
        jump = GetComponent<Jump>();
        isDoubleJump = false;

        move = GetComponent<Move>();
        bulletShot = GetComponent<Shot>();
        bulletShot.Init("Bullet", MAXPLAYERBULLETCOUNT, 100.0f, shotDamage);

        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", MAXPLAYERBOMBCOUNT, bombPower);

        shortAttack = GetComponent<ShortAttack>();

        hp = 100;
    }
    private bool isKeyNone = false;
    private float r = 0.0f;
    private float ry = 0.0f;
    private void PlayerManual()
    {
        r = Input.GetAxis("Mouse X");

        // ry = Input.GetAxis("Mouse Y"); //완벽하지 않아서 주석처리

        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전

       //  playerTr.Rotate(Vector3.forward * rotSpeed * Time.deltaTime * ry); // Z축을 기준으로 rotSpeed 만큼 회전
       
        if(isKeyNone)           //어디서든지 움직임을 중단시킬수있는 변수
        {
            move.Init();
            return;
        }
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");

    }
  
   
    //애니메이터 컨트롤러 해시값 추출    
    private readonly int hashV = Animator.StringToHash("v");
    private readonly int hashH = Animator.StringToHash("h");
    private readonly int hashJ = Animator.StringToHash("airborne");
    void Update()
    {
        PlayerManual();
        KeyboardManual();//입력        
        SetMoveState();//움직임
        Running();//달리기
        MouseManual();//마우스

        LogicAnimation();//애니메이션 웬만하면 제일마지막

        //해시에 이동 계수 전달
      
       playerAni.SetFloat(hashV, move.Vertical);
        playerAni.SetFloat(hashH, move.Horizontal);
        playerAni.SetFloat(hashJ, jump.airborneSpeed);
    }

    private bool isDoubleJump = false;
    //키보드 입력
    private void KeyboardManual()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {   FuncJump(); }
      

        if (Input.GetKeyDown(KeyCode.R))
        {
            bombThrow.Work();          
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            playerAni.SetTrigger("LongAttack");
            isKeyNone = true;
            isJumpNone = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAni.Play("EllenTurnOnSpotLeft45");
            eSpecialState = SPECIAL_STATE.TURNONSPOT;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            playerAni.Play("EllenTurnOnSpotRight45");
            eSpecialState = SPECIAL_STATE.TURNONSPOT;
        }

        SetFun();
    }
    

    //상태리셋
   private void ResetState()
    {
        playerAni.Play("PlayerIdle");
        eState = STATE.STAND;
        eSpecialState = SPECIAL_STATE.NONE;
        isKeyNone = false;
        isJumpNone = false;
    }

    //이전상태로 리셋
    //이전상태로 리셋
    private void ResetPreState()
    {
        eState = ePreState;
        isKeyNone = false;
        isJumpNone = false;
    }


    //점프및 더블점프
    private bool isJumpNone = false;            //점프가 가능한 상태임을 나타내는 변수
    private bool isDoubleJumping = false;
    private void FuncJump()
    {
        if (isJumpNone) return;
        if (eState == STATE.JUMP && isDoubleJump == true)       //이단점프
        {
            if (ePreState == STATE.STAND)
             playerRb.AddForce(new Vector3(0, 2.6f, 0) * 5.0f, ForceMode.VelocityChange);
            else  playerRb.AddForce(new Vector3(0, 1.6f, 0) * 5.0f, ForceMode.VelocityChange);
            isDoubleJump = false;
            isDoubleJumping = true;
        }
        else if (eState != STATE.JUMP)
        {
            ePreState = eState;     //점프전 상태보관
            eState = STATE.JUMP;
            if (ePreState == STATE.STAND)
            {
                jump.Action(1.6f, 5.0f);
            }

            else
            {
                jump.Action(1.6f, 5.0f);//점프력,점프스피드
            } 

            isDoubleJump = true;
            StartCoroutine(JumpingStart());
        }
    }
    //이단점프 코루틴
    IEnumerator JumpingStart()
    {
        yield return new WaitForSeconds(0.5f);          //0.2초안에 두번눌러야 달리기h
        isDoubleJump = false;


    }


    //기본적인 움직임 상태값
    private void SetMoveState()       //움직이는 상태변경
    {
        if (eState == STATE.JUMP) return;
        if (eState == STATE.ATTACK) return;
   
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
           // if (eState == STATE.RUN) return;
            eState = STATE.WALK;
        }
        else
        {
            eState = STATE.STAND;
            
        }
  
    }
   
    //애니메이션 해제용 이벤트
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground" && eState == STATE.JUMP)
        {
            if (isDoubleJumping)
            {          //더블점프끄나면 착지
                playerAni.Play("EllenIdleLandFast");
                eSpecialState = SPECIAL_STATE.DOUBLEJUMPLANDING;
                isKeyNone = true;
                isJumpNone = true;
            }    
            isDoubleJumping = false;
            eState = ePreState;
            ManagerTest();
        }
    }

    //애니메이션 상태
    private void LogicAnimation()
    {
        switch (eState)
        {
            case STATE.HIT:
                playerAni.SetBool("isHit", true);
                break;
            case STATE.JUMP:
                playerAni.SetBool("IsJump",true);
                break;
            
            case STATE.RUN:
                SetMove(10.0f);
                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsRun", true);
                playerAni.SetBool("IsWalk", false);
                break;
            case STATE.WALK:
                SetMove(5.0f);
                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsWalk", true);
                playerAni.SetBool("IsRun", false);
                break;
            case STATE.ATTACK:
  
                
                break;
            case STATE.STAND:
                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsWalk", false);
                playerAni.SetBool("IsRun", false);


               
                break;
        }    
    }

    private void SetMove(float _speed)
    {
        move.SetMoveSpeed(_speed);
    }
   
    //달리기
    private void Running()          //달리기는 쉬프트
    {
       if(Input.GetKey(KeyCode.LeftShift) &&eState == STATE.WALK  )
        {
            eState = STATE.RUN;
        }
       else if(Input.GetKeyUp(KeyCode.LeftShift) && eState == STATE.RUN)
        {
            eState = STATE.WALK;
        }
    }

    //구르기
    private void Dumbling()
    {
        if (Input.GetKey(KeyCode.W)) //&& isRun == true)       //두번누르면 여기로들어옴
        {
            //isRun = false;
            // eState = STATE.RUN;
        }
        else if (Input.GetKeyUp(KeyCode.W) && eState != STATE.RUN)
        {
            // isRun = true;
            //StartCoroutine(RunningStart());
        }
    }
    IEnumerator RunningStart()
    {
        yield return new WaitForSeconds(0.2f);          //0.2초안에 두번눌러야 달리기h
        //isRun = false;
    }

    private int comboCount = 0;
    //마우스로 인한 상태변경
    private void MouseManual()                  //마우스로 콤보 공격 가능
    {
        if (Input.GetMouseButtonDown(0))
        {
            comboCount++;
            playerAni.SetInteger("ShortAttackCombo", comboCount);
            isKeyNone = true;
            isJumpNone = true;
            Check.AllFreeze(playerRb);
        }
    }
    private void ShartAttackExit()
    {
        comboCount = 0;
        playerAni.SetInteger("ShortAttackCombo", 0);
        isKeyNone = false;
        isJumpNone = false;
        Check.ResetFreeze(playerRb);        //이 한줄이면 원래대로 만들어줌 ( 플레이어 한해서)
    }

    void ManagerTest()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.NPC_CHAT_START, this);
    }

    private void SetFun()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (eSpecialState == SPECIAL_STATE.DANCE)
            {
                ResetState();
                return;
            }
            playerAni.SetTrigger("Dance");
            eSpecialState = SPECIAL_STATE.DANCE;
        }
    }
    

}
 