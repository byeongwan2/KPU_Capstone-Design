using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SPECIAL_STATE { NONE, TURNONSPOT, DOUBLEJUMPLANDING, DANCE, AIM }

public class Player2 : MoveObject {

    private Transform playerTr;
    private Animator playerAni; public Animator GetPlayerAni()  { return playerAni; }
    private Move move;
    private Shot bulletShot;
    private Throw bombThrow;
    private Jump jump;
    [SerializeField]
    private STATE eState;           public string TempStateReturn() { return eState.ToString(); }
    private STATE ePreState;
    private SPECIAL_STATE eSpecialState;


    private CapsuleCollider playerCol;
    private bool isRoll;    
    private bool isSpecialState = false;

    private readonly int MAXPLAYERBOMBCOUNT = 10;
    private readonly int MAXPLAYERBULLETCOUNT = 40;
    [SerializeField]
    private int shotDamage = 10;            //무기의 데미지는 다를꺼기때문에 배열혹은 열거형으로 전환할가능성 ↑
    //private State m_state = null;               //상태에 따른클래스를 갖게끔
    private bool isMouse;
    private bool isMove;

    private bool isJumpDelay;    public bool IsJump() { return isJumpDelay; }
    private bool isAttackStop;
    private bool isRun;
	void Start () {
        instance = this;
        playerTr = GetComponent<Transform>();
        move = GetComponent<Move>();
        playerAni = GetComponent<Animator>();
        bulletShot = GetComponent<Shot>();
        bulletShot.Init("PlayerBasicBullet", MAXPLAYERBULLETCOUNT, 20.0f, shotDamage);

        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", 10, 15.0f);

        jump = GetComponent<Jump>();
        playerCol = GetComponent<CapsuleCollider>();
        eState = STATE.STAND;
        ePreState = STATE.STAND;
        eSpecialState = SPECIAL_STATE.NONE;
        isMouse = false;
        isMove = false;
        isRoll = false;
        isSpecialState = false;
        // m_state = new Stand();
        // m_state.PlayAnimation(playerAni);
        isJumpDelay = false;
        isAttackStop = false;
        isRun = false;
        bulletCount = 20;
    }

    // Update is called once per frame
    void Update () {
        Dancing();
        LookMousePoint();
        MouseManual();
        Running();
        Rolling();
        Jumping();
        Reloading();

        MovePlayer();
        KeyBoardManual();
        Logic();
        BlendAnimation();
        Render();
        SpecialAnimation();
    }

    //마우스 바라보기
    private void LookMousePoint()           
    {
        if (isMouse) return;
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장

        Vector3 pos = playerTr.position; //게임 오브젝트 좌표 저장
        Vector3 mpos2 = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);

        Vector3 aim1 = Camera.main.ScreenToWorldPoint(mpos2);

        float dx = aim1.x - pos.x;
        float dz = aim1.z - pos.z;

        float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;        
        playerTr.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
    }

    //이동값설정
    private void MovePlayer()           
    {
        if (isMove) return;
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }
    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(ePreState != STATE.JUMP)ePreState = eState;
            eState = STATE.JUMP;
            playerAni.SetTrigger("Jump");
            isJumpDelay = true;
            isMouse = true;
           // jump.Action(0.8f, 5.0f);

        }
    }
    private void Event_JumpingExit()
    {
        isMouse = false;
        eState = ePreState;
        if (isJumpDelay == true) eState = STATE.STAND;     //버그방지
        isJumpDelay = false;
    }


    private void KeyBoardManual()       //키보드입력시 상태변경
    {
        if (eState == STATE.ROLL) return;
        if (isJumpDelay == true) return;
        if (isRun == true) return;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            eState = STATE.WALK;
        }
        else
        {
            eState = STATE.STAND;
        }
       

    }
    //클래식 방식으로 이름을 짜봄 논리
    private void Logic()           
    {
        if (isMove) return;
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
            case STATE.JUMP:

                break;
          
            case STATE.RUN:
                playerAni.SetBool("IsRun", true);
                playerAni.SetBool("IsWalk", false);
                Velocity();
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

    private float velocity = 0.0f; //가속도   
    private void Velocity()        //가속도, 캐릭터 회전 적용 함수 
    {
        velocity += Time.deltaTime; //  * 10.0f;  //10.0f 를 곱하지않으면 밑에 else if 로 들어가지않아서 Run이해제안댐 일단원본
        playerAni.SetFloat(hashVelocity, velocity);

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 1.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -100.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 100.0f, 0.0f), Time.deltaTime * 10.0f);
    }
    //달리기
    private void Running()          //달리기는 쉬프트
    {
        if (Input.GetKey(KeyCode.LeftShift) && eState == STATE.WALK)
        {
            eState = STATE.RUN;            
            isRun = true;
            isMouse = true;
        }

        else  if (Input.GetKeyUp(KeyCode.LeftShift) && velocity >=0.1f)
        {
            velocity = 0.0f;
            playerAni.SetFloat(hashVelocity, velocity);
            eState = STATE.WALK;
            isRun = false;
            isMouse = false;
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
    private readonly int hashVelocity = Animator.StringToHash("Velocity");
    private readonly int hashX = Animator.StringToHash("X");
    private readonly int hashZ = Animator.StringToHash("Z");
    //블랜드 애니메이션
    private void BlendAnimation()
    {        
        playerAni.SetFloat(hashAngle, playerTr.rotation.eulerAngles.y);      
        playerAni.SetFloat(hashX, move.Horizontal);
        playerAni.SetFloat(hashZ, move.Vertical);
    }

    //진짜구르기
    private void Rolling()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            if (isRoll) return;
            isRoll = true;
            playerAni.SetTrigger("IsRoll");
            isMouse = true;
            isMove = true;
            ePreState = eState;
            eState = STATE.ROLL;
        }
    }

    //구르기
    private void TestRolling()
    {
        return;
        if(Input.GetKeyUp(KeyCode.W))
        {
            if (isSpecialState) return;
            isRoll = true;
            Invoke("RollingCancel", 0.3f);
        }
        else if(Input.GetKeyDown(KeyCode.W) && isRoll == true)
        {
            isMove = true;
            playerAni.SetTrigger("IsRoll");
            isSpecialState = true;
            isRoll = false;
            ePreState = eState;
            eState = STATE.ROLL;
            isMouse = true;
            Invoke("RollingReset", 1.0f);
        }
    }
    private void Event_RollingExit()
    {
        eState = STATE.WALK ;
        isMouse = false;
        isMove = false;
        isRoll = false;
    }
    private void RollingCancel() { isRoll = false; }
    private void RollingReset() { isSpecialState = false; }

    int bulletCount = 20;           //총알
    bool attackCoolTime = false;
    //기본공격
    private void MouseManual()
    {
        if (eState == STATE.JUMP) return;
        if (isAttackStop) return;
        if (Input.GetMouseButton(Define.MOUSE_LEFT_BUTTON) && attackCoolTime == false)
        {
            if (bulletCount == 0) {
                playerAni.SetTrigger("Reload");
                isAttackStop = true;
                bulletCount = 20;
                return;
            }
            bulletCount--;
            attackCoolTime = true;
            StartCoroutine(AttackBasicExit());
            playerAni.SetTrigger("Attack");
            bulletShot.Work();
        }
        else if(Input.GetMouseButtonDown(Define.MOUSE_RIGHT_BUTTON))
        {
            playerAni.SetTrigger("Throw");
            isMouse = true;
            isAttackStop = true;
        }
    }
    private IEnumerator AttackBasicExit()
    {
        yield return new WaitForSeconds(0.15f);
        attackCoolTime = false;
    }

    public static Player2 instance;     //조심해서 써야함
    private void Dancing()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { playerAni.SetInteger("Dance", 1); eSpecialState = SPECIAL_STATE.DANCE; }
        if (Input.GetKeyDown(KeyCode.F2)){ playerAni.SetInteger("Dance", 2); eSpecialState = SPECIAL_STATE.DANCE; }
        if (Input.GetKeyDown(KeyCode.F3)) { playerAni.SetInteger("Dance", 3); eSpecialState = SPECIAL_STATE.DANCE; }
    }

    private void Reloading()
    {
        if (isAttackStop) return;
        if(Input.GetKeyDown(KeyCode.R))
        {
            playerAni.SetTrigger("Reload");
        }
    }

    private void Event_ThrowBomb()
    {
        bombThrow.Work();
    }

    private void Event_MouseExit()
    {
        isMouse = false;
        isAttackStop = false;
    }
    private void Event_ReloadExit()
    {
        isAttackStop = false;
    }

    
}
