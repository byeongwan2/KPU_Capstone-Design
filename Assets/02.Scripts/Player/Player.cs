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
    private GameObject canvas, aim;        

    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE eState = STATE.STAND;
    [SerializeField]                    //밖에서 쳐다보기위해 노출만시킴 
    private STATE ePreState = STATE.STAND;
    [SerializeField]
    private SPECIAL_STATE eSpecialState = SPECIAL_STATE.NONE;
    private WAY eWay = WAY.FORWARD;

    [SerializeField]
    private float bombPower = 15.0f;

    private readonly int MAXPLAYERBOMBCOUNT = 10;
    private readonly int MAXPLAYERBULLETCOUNT = 40;

    [SerializeField]
    private int shotDamage = 10;            //무기의 데미지는 다를꺼기때문에 배열혹은 열거형으로 전환할가능성 ↑
    //절대 바뀌지않는 초기화//컴포넌트관련내용만
    [SerializeField]
    private RuntimeAnimatorController normal_Animator;          //툴에서 초기화
    void Start()
    {
        base.Setting();
        

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

        hp.SettingHp(100);
    }
    private bool isKeyNone = false;    
    private void PlayerManual()
    {                      
        if(isKeyNone)           //어디서든지 움직임을 중단시킬수있는 변수
        {
            move.SetZero();
            return;
        }
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }
  
   
    //애니메이터 컨트롤러 해시값 추출    
    private readonly int hashV = Animator.StringToHash("v");
    private readonly int hashH = Animator.StringToHash("h");
    private readonly int hashJ = Animator.StringToHash("airborne");
    private readonly int hashGun = Animator.StringToHash("IsGun");

    void Update()
    {
        RotCamera();
       //PlayerManual();
        //KeyboardManual();       //입력        
        //SetMoveState();         //움직임
        //Running();              //달리기
        //MouseManual();          //마우스
        //ChangeGunMode();         //모드 변경


        LogicAnimation();//애니메이션 웬만하면 제일마지막

        //해시에 이동 계수 전달
      
       playerAni.SetFloat(hashV, move.Vertical);
        playerAni.SetFloat(hashH, move.Horizontal);
        playerAni.SetFloat(hashJ, jump.airborneSpeed);
    }
    public Camera _camera;

    public Vector3 aim1;
    private void RotCamera()
    {
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장
        Vector3 pos = playerTr.position; //게임 오브젝트 좌표 저장
        mpos.z = mpos.y;


        aim1 = Camera.main.ScreenToWorldPoint(mpos);
        Debug.Log(aim1);
        aim1.x -= pos.x;
        aim1.z -= pos.z;
       
        float dx = aim1.x - pos.x ;
        float dz = aim1.z - pos.z ;

        float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;        

        playerTr.rotation = Quaternion.Euler(0.0f, rotateDegree,0.0f);

    }

    private bool isDoubleJump = false;
    //키보드 입력
    private void KeyboardManual()
    {
        if (isKeyNone) return;
        if (Input.GetKeyDown(KeyCode.Space)) {   FuncJump(); }
      

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerAni.SetTrigger("IsThrow");  
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

    private bool isGun = false;
    //Gun Mode 변경
    private void ChangeGunMode()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
           if(isGun == false)
            {
                isGun = true;                
                playerAni.SetBool(hashGun, true);
               // playerAni.SetLayerWeight(1, 1);
               // playerAni.SetLayerWeight(0, 0);
            }
           else
            {
                isGun = false;                
                playerAni.SetBool(hashGun, false);
               // playerAni.SetLayerWeight(1, 0);
                //playerAni.SetLayerWeight(0, 1);
            }
             
        }
    }
   
}
 