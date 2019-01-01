﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//함수는 동사_단어 혹은 동사 
//코루틴함수는 동사단어  
//이벤트함수는 Event_수식 혹은 Event_동사

//단어 수식 명사 등등 

//변수는 소문자시작 언더바없고 기본변수는 두단어이상 
//클래스이름은 대문자시작 언더바없음



public partial class Player2 : MoveObject
{
    private Transform playerTr;
    private Animator playerAni; public Animator GetPlayerAni() { return playerAni; }
    private Shot bulletShot;
    private Throw bombThrow;
    private Jump jump;
    private Dash dash;
    private Move move;
    [SerializeField]
    private STATE eState;           public string TempStateReturn() { return eState.ToString(); }
    private STATE ePreState;

    private CapsuleCollider playerCol;
    private bool isRoll;    

    private readonly int MAXPLAYERBULLETCOUNT = 40;
    [SerializeField]
    private int shotDamage = 10;            

    private bool isMouse;
    private bool isJumpHit;         //점프할때 피격이 가능한지 false이면 가능
    public bool IsJumpHit() { return isJumpHit; }
    private bool isJumpDelay;   //점프중인지 단순확인 
    private bool isAttackStop;
    private bool isRun;
    private bool isDash;

    private bool isReload=false;
    //상의 하의 유용할수 있는 변수
    private bool isTop = false;
    private bool isDown = false;
    public static Player2 instance;     //조심해서 써야함
    void Start ()
    {
        instance = this;
        playerTr = GetComponent<Transform>();
        playerAni = GetComponent<Animator>();
        

        bulletShot = GetComponent<Shot>();
        bulletShot.Init("PlayerBasicBullet", MAXPLAYERBULLETCOUNT, 20.0f, shotDamage,TYPE.BULLET);
        bulletShot.Init("PlayerAdvanceBullet", MAXPLAYERBULLETCOUNT, 0, shotDamage,TYPE.ADVANCEBULLET);
        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", 10, 15.0f);
        jump = GetComponent<Jump>();
        dash = GetComponent<Dash>();
        move = GetComponent<Move>();
        playerCol = GetComponent<CapsuleCollider>();        // 없어질 예정?

        this_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        //현재상태 // 이전상태 
        eState = STATE.STAND;
        ePreState = STATE.STAND;

        //is로 시작하는 bool변수는 false일때 해당변수를 안하고있다는뜻 isRoll 가 false라면 안굴고 있다는뜻
        isMouse = false;
        isRoll = false;
        isJumpDelay = false;
        isJumpHit = false;
        isAttackStop = false;
        isRun = false;
        isDash = false;
        bulletCount = 35;       //현재 35발쏘고 장전

        attackMode = 0;
        isReload = false;
    }

    // Update is called once per frame
    void Update () {
        
        Look_MousePoint();
        Attack();
        //구르기
        Rolling();
        //달리기
        Run_Rotation();
        Running();

        //점프 현재보류상태
        Jumping();
        //장전
        Reloading();
        //대시
        Move_Dash();
        Move_Update();
        Input_Move();
        Logic();

        Change_Gun();
        Dancing();
        Render();
    }

    //마우스 바라보기
    private void Look_MousePoint()           
    {
        if (isMouse) return;
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장       
        Vector3 mpos2 = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);
        Vector3 aim1 = Camera.main.ScreenToWorldPoint(mpos2);
        float rotateDegree = Mathf.Atan2(aim1.x - transform.position.x, aim1.z - transform.position.z) * Mathf.Rad2Deg;        
        playerTr.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
    }

    //이동값설정
    private void Move_Update()           
    {
        if (isDash) return;
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }
    private void Jumping()
    {
        return;
        if (isDash) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(ePreState != STATE.JUMP)ePreState = eState;
            eState = STATE.JUMP;
            playerAni.SetTrigger("Jump");
            isJumpDelay = true;
            isJumpHit = true;
            isMouse = true;
           // jump.Action(0.8f, 5.0f);

        }
    }

    private void Input_Move()       //키보드입력시 상태변경
    {
        if (isDash) return;
        if (eState == STATE.ROLL ) return;
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
    //do 혹은 update 역할
    private void Logic()           
    {
        if (isDash) return;
        switch (eState)
        {
            case STATE.RUN:
                move.Set_MoveSpeed(4.0f);
                break;
            case STATE.WALK:
                move.Set_MoveSpeed(2.0f);
                break;
        }
    }
    //애니메이터 컨트롤러 해시값 추출    
    private readonly int hashAngle = Animator.StringToHash("Angle");
    private readonly int hashVelocity = Animator.StringToHash("Velocity");
    private readonly int hashX = Animator.StringToHash("X");
    private readonly int hashZ = Animator.StringToHash("Z");
    //애니메이션
    private void Render()                
    {
        switch (eState)
        {
            case STATE.DASH:
                playerAni.SetBool("Dash", true);
               
                break;
            case STATE.RUN:
                playerAni.SetBool("IsRun", true);
                playerAni.SetBool("IsWalk", false);
                break;
            case STATE.WALK:
                playerAni.SetBool("IsWalk", true);
                playerAni.SetBool("IsRun", false);
                playerAni.SetBool("Dash", false);
                break;

            case STATE.STAND:
                playerAni.SetBool("IsRun", false);
                playerAni.SetBool("IsWalk", false);
                playerAni.SetBool("Dash", false);
                break;
        }
        playerAni.SetFloat(hashAngle, playerTr.rotation.eulerAngles.y);
        playerAni.SetFloat(hashX, move.Horizontal);
        playerAni.SetFloat(hashZ, move.Vertical);
    }
    //달리기
    private float velocity = 0.0f; //가속도   
    private void Run_Rotation()        //가속도, 캐릭터 회전 적용 함수 
    {
        if (isDash) return;
        if (eState != STATE.RUN) return;
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
        if (isDash) return;
        if (Input.GetKey(KeyCode.LeftShift) && eState == STATE.WALK)
        {
            eState = STATE.RUN;            
            isRun = true;
            isMouse = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && velocity >0.0f)
        {
            playerAni.SetFloat(hashVelocity, velocity);
            eState = STATE.WALK;
            isRun = false;
            isMouse = false;
        }
        if (isRun == false)
            if (velocity > 0.0f)
            {
                velocity -= Time.deltaTime;
                if (velocity < 0.0f) velocity = 0.0f;
            }
          
    }
  

    //진짜구르기
    private void Rolling()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            if (isRoll) return;
            playerAni.SetTrigger("IsRoll");
            isMouse = true;
            isRoll = true;
            ePreState = eState;
            eState = STATE.ROLL;
        }
    }

    int bulletCount = 20;           //총알
    bool attackCoolTime = false;
    //기본공격
    private void Attack()
    {
        if (isDash) return;
        if (eState == STATE.JUMP) return;
        if (isReload) return;
        if (isAttackStop) return;
        if (Input.GetMouseButton(Define.MOUSE_LEFT_BUTTON) && attackCoolTime == false)
        {
            if (bulletCount == 0)
            {
                playerAni.SetTrigger("Reload");
                isReload = true;
                bulletCount = 20;
                return;
            }
            bulletCount--;
            attackCoolTime = true;
            playerAni.SetTrigger("Attack");
            Attack_Gun();
        }
        else if(Input.GetMouseButtonDown(Define.MOUSE_RIGHT_BUTTON))
        {
            playerAni.SetTrigger("Throw");
            isMouse = true;
            isAttackStop = true;
        }
    }
   

    private void Dancing()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { playerAni.SetInteger("Dance", 1);  }
        if (Input.GetKeyDown(KeyCode.F2)){ playerAni.SetInteger("Dance", 2);  }
        if (Input.GetKeyDown(KeyCode.F3)) { playerAni.SetInteger("Dance", 3);  }
    }

    private void Reloading()
    {
        if (isDash) return;
        if (isReload) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReload = true;
            playerAni.SetTrigger("Reload");
        }
    }

    //깜빡임
    SkinnedMeshRenderer[] this_renderer;

    public override void Wound_Effect()
    {
        woundEffect = true;
        StartCoroutine(ColEffect());
        Invoke("WoundEffectExit", 1.5f);
    }

    IEnumerator ColEffect()
    {
        while (true)
        {
            this_renderer[0].material.color = Color.red;
            this_renderer[1].material.color = Color.red;
            this_renderer[2].material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            this_renderer[0].material.color = Color.white;
            this_renderer[1].material.color = Color.white;
            this_renderer[2].material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            if (woundEffect == false) yield break; 
        }

    }

    bool woundEffect = false;
    void WoundEffectExit()
    {
        woundEffect = false;
    }

    void Move_Dash()
    {
        if (Input.GetKey(KeyCode.C))
        {
            isDash = true;
            eState = STATE.DASH;
            Vector3 aim1 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            dash.Dash_Destination(aim1,40.0f);
        }
        else if(Input.GetKeyUp(KeyCode.C))
        {
            eState = STATE.STAND;
            isDash = false;
        }
    }
    int attackMode = 0;
    void Attack_Gun()
    {
        if (attackMode == 0)
            bulletShot.Work(TYPE.BULLET);
        else if (attackMode == 1)
            bulletShot.Work(TYPE.ADVANCEBULLET);
    }

    void Change_Gun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            attackMode = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            attackMode = 1;
    }
}
