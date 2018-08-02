using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SPECIAL_STATE {  NONE,TURNONSPOT}

public class Player : MonoBehaviour {

    private float r = 0.0f;
    private float ry = 0.0f;

    private Transform playerTr;
    private Rigidbody playerRb;
    private Animator playerAni;
    private Jump jump;
    private Move move;
    private Throw bombThrow;
    public float rotSpeed = 250.0f; //회전 속도

    [SerializeField]
    private STATE eState = STATE.STAND;
    private STATE ePreState = STATE.STAND;
    private SPECIAL_STATE eSpecialState = SPECIAL_STATE.NONE;
    private WAY eWay = WAY.FORWARD;
    private bool isDoubleJump = false;

    [SerializeField]
    private float bombPower;

    private int MAXPLAYERBOMB = 10;

    //애니메이터 컨트롤러 해시값 추출    
    private readonly int hashV = Animator.StringToHash("v");
    private readonly int hashH = Animator.StringToHash("h");
    private readonly int hashJ = Animator.StringToHash("airborne");

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

        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", MAXPLAYERBOMB);
        bombPower = 15.0f;

    }

    void Update()
    {
        r = Input.GetAxis("Mouse X");

       // ry = Input.GetAxis("Mouse Y"); 완벽하지 않아서 주석처리

        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전
       // playerTr.Rotate(Vector3.forward * rotSpeed * Time.deltaTime * ry); // Z축을 기준으로 rotSpeed 만큼 회전

        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");

        KeyboardManual();
        //WayManual();
        MoveManual();
        Running();


        LogicAnimation();

        //해시에 이동 계수 전달
        playerAni.SetFloat(hashV, move.Vertical);
        playerAni.SetFloat(hashH, move.Horizontal);
        playerAni.SetFloat(hashJ, jump.airborneSpeed);
    }


    private void KeyboardManual()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (eState == STATE.JUMP && isDoubleJump == true)       //이단점프
            {
                playerRb.mass = 1.0f;           //무게를잠시수정
                playerRb.AddForce(new Vector3(0, 1.6f, 0) * 5.0f, ForceMode.Impulse);
                isDoubleJump = false;
                playerRb.mass = 1.2f;
            }
            else if(eState != STATE.JUMP)
            {                                
                ePreState = eState;     //점프전 상태보관
                eState = STATE.JUMP;
                if (ePreState == STATE.STAND) jump.Action(16.6f, 5.0f);
                else jump.Action(1.6f, 5.0f);     //점프력,점프스피드

                isDoubleJump = true; 
            }
        }
       
        if(Input.GetKeyDown(KeyCode.Return))
        {
         //뭐를쓸까낭?
        }


        if(Input.GetKeyDown(KeyCode.R))
        {
            bombThrow.Work(bombPower);          
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
    }

   private void ResetState()
    {
        playerAni.Play("PlayerIdle");
        eState = STATE.STAND;
    }

    

    private void MoveManual()       //상태만 바꾸는곳 메뉴얼이라는함수는 상태만 바꿈
    {
        if (eState == STATE.JUMP) return;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (eState == STATE.RUN) return;
            eState = STATE.WALK;

        }
        else
        {
            eState = STATE.STAND;
        }
  
    }
  

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground" && eState == STATE.JUMP)
        {
            eState = ePreState;
            isDoubleJump = false;
        }
    }

    private void LogicAnimation()
    {
        switch (eState)
        {
            case STATE.JUMP:
                playerAni.SetBool("IsJump",true);
                break;
            
            case STATE.RUN:
                move.SetMoveSpeed(10.0f);
                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsRun", true);
                playerAni.SetBool("IsWalk", false);
                break;
            case STATE.WALK:
                move.SetMoveSpeed(5.0f);
                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsWalk", true);
                playerAni.SetBool("IsRun", false);
                break;
            case STATE.STAND:

                playerAni.SetBool("IsJump", false);
                playerAni.SetBool("IsWalk", false);
                playerAni.SetBool("IsRun", false);
              
                break;
        }    
    }
    private void Dumbling()
    {
        if (Input.GetKey(KeyCode.W) ) //&& isRun == true)       //두번누르면 여기로들어옴
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

    IEnumerator RunningStart()
    {
        yield return new WaitForSeconds(0.2f);          //0.2초안에 두번눌러야 달리기h
        //isRun = false;
    }




    void OnTriggerEnter(Collider _obj)
    {
        if(_obj.tag == "Item"){
            _obj.gameObject.SetActive(false);       // 필드에서 아이템흡수
        }
        
    }


    
}
 