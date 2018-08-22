using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SPECIAL_STATE { NONE, TURNONSPOT, DOUBLEJUMPLANDING, DANCE, AIM }

public class Player2 : MoveObject {

    private Transform playerTr;
    private Animator playerAni; public Animator GetPlayerAni()  { return playerAni; }
    private Move move;
    private Shot bulletShot;
    [SerializeField]
    private STATE eState;           public string TempStateReturn() { return eState.ToString(); }
    private STATE ePreState;
    private SPECIAL_STATE eSpecialState;

    private bool isRollDelay;    
    private bool isSpecialState = false;

    private bool isMouse;
    private readonly int MAXPLAYERBOMBCOUNT = 10;
    private readonly int MAXPLAYERBULLETCOUNT = 40;
    [SerializeField]
    private int shotDamage = 10;            //무기의 데미지는 다를꺼기때문에 배열혹은 열거형으로 전환할가능성 ↑
    private State m_state = null;               //상태에 따른클래스를 갖게끔
	void Start () {
        instance = this;
        playerTr = GetComponent<Transform>();
        move = GetComponent<Move>();
        playerAni = GetComponent<Animator>();
        bulletShot = GetComponent<Shot>();
        bulletShot.Init("PlayerBasicBullet", MAXPLAYERBULLETCOUNT, 20.0f, shotDamage);

        eState = STATE.STAND;
        ePreState = STATE.STAND;
        eSpecialState = SPECIAL_STATE.NONE;

        isRollDelay = false;
        isSpecialState = false;

        isMouse = false;            //true일때 마우스가 멈춤 false때 작동함 디폴트 false

       // m_state = new Stand();
       // m_state.PlayAnimation(playerAni);
    }

    // Update is called once per frame
    void Update () {
        Dance();
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
        if ( isMouse) return;
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
    //클래식 방식으로 이름을 짜봄 논리
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

    private float velocity = 0.0f; //가속도        
    //달리기
    private void Running()          //달리기는 쉬프트
    {    
        if (Input.GetKey(KeyCode.LeftShift) && eState == STATE.WALK)
        {
            eState = STATE.RUN;
            velocity = velocity + Time.deltaTime;
            playerAni.SetFloat(hashVelocity, velocity);

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -60.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 60.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -145.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 145.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.W)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 1.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -100.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.S)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 10.0f);
            else if (Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 100.0f, 0.0f), Time.deltaTime * 10.0f);
            isMouse = true;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift) && velocity >=0.1f)
        {   
            velocity = 0.0f;
            playerAni.SetFloat(hashVelocity, velocity);
            eState = STATE.WALK;
            isMouse = false;
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
    private readonly int hashVelocity = Animator.StringToHash("Velocity");
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
            bulletShot.Work();
        }
    }
    private void AttackBasicExit()
    {

    }

    public static Player2 instance;     //조심해서 써야함
    private void Dance()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { playerAni.SetInteger("Dance", 1); eSpecialState = SPECIAL_STATE.DANCE; }
        if (Input.GetKeyDown(KeyCode.F2)){ playerAni.SetInteger("Dance", 2); eSpecialState = SPECIAL_STATE.DANCE; }
        if (Input.GetKeyDown(KeyCode.F3)) { playerAni.SetInteger("Dance", 3); eSpecialState = SPECIAL_STATE.DANCE; }
    }
    
}
