using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private WAY eWay = WAY.FORWARD;
    private bool isDoubleJump = false;
    private bool isRun = false;

    [SerializeField]
    private float bombPower;

    private int MAXPLAYERBOMB = 10;

    //애니메이터 컨트롤러 해시값 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashV = Animator.StringToHash("v");

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
        isRun = false;

        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", MAXPLAYERBOMB);
        bombPower = 15.0f;

    }

    void Update()
    {
        r = Input.GetAxis("Mouse X");
       // ry = Input.GetAxis("Mouse Y");

       // playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime *r); // Y축을 기준으로 rotSpeed 만큼 회전
       // playerTr.Rotate(Vector3.forward * rotSpeed * Time.deltaTime * ry); // Z축을 기준으로 rotSpeed 만큼 회전

        //move.Horizontal = Input.GetAxis("Horizontal");
        //move.Vertical = Input.GetAxis("Vertical");

        KeyboardManual();
        WayManual();
        MoveManual();

        LogicAttribute();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
           // playerAni.SetBool(hashMove, true);
        }
        else
        {
            playerAni.SetBool(hashMove, false);
        }

        //playerAni.SetFloat(hashV, -1.0f);
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
                ePreState = eState;
                eState = STATE.JUMP;
                jump.Action(1.6f, 5.0f);     //점프력,점프스피드
                isDoubleJump = true;
            }
        }
       
        if(Input.GetKeyDown(KeyCode.Return))
        {
         //뭐를쓸까낭?
        }

        Running();

        if(Input.GetKeyDown(KeyCode.E))
        {
            bombThrow.Work(bombPower);
          
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

    void State()
    {
        
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
    private void WayManual()
    {
        if(Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.A))
        {
            //eWay = WAY.F45L;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            eWay = WAY.LEFT;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            eWay = WAY.RIGHT;
        }
        else if(move.Horizontal == 1.0f)
        {
          //  eWay = WAY.FORWARD;           
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            eWay = WAY.BACKWARD;
        }
        
    }

    private void LogicAttribute()
    {
        switch(eState)
        {
            case STATE.RUN:
               // move.SetMoveSpeed(20.0f);
               // playerAni.SetBool("IsMove", true);
                break;
            case STATE.WALK:
               // move.SetMoveSpeed(10.0f);
              //  playerAni.SetBool("IsMove", true);
                break;
            case STATE.STAND:
                playerAni.SetBool("IsMove", false);
                break;
        }

        switch(eWay)
        {
            case WAY.FORWARD:
              //  playerAni.SetInteger("IsWay", 0);
                break;
            case WAY.BACKWARD:                
                playerAni.SetInteger("IsWay", 1);
                break;
            case WAY.LEFT:
                playerAni.SetInteger("IsWay", 2);
                break;
            case WAY.RIGHT:
                playerAni.SetInteger("IsWay", 3);   
                break;
            case WAY.F45L:
                playerAni.SetInteger("IsWay", 4);
                break;
            case WAY.F45R:
                playerAni.SetInteger("IsWay", 5);
                break;
        }
    }

    private void Running()
    {
        if (Input.GetKey(KeyCode.W) && isRun == true)       //두번누르면 여기로들어옴
        {
            isRun = false;
            eState = STATE.RUN;
        }
        else if(Input.GetKeyUp(KeyCode.W) && eState != STATE.RUN)       
        {
            isRun = true;
            StartCoroutine(RunningStart());
        }
    }

    IEnumerator RunningStart()
    {
        yield return new WaitForSeconds(0.2f);          //0.2초안에 두번눌러야 달리기
        isRun = false;
    }

    void OnTriggerEnter(Collider _obj)
    {
        if(_obj.tag == "Item"){
            _obj.gameObject.SetActive(false);       // 필드에서 아이템흡수
        }
        
    }
}
 