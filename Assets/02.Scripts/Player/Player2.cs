using System.Collections;
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
    private Rigidbody playerRb;
    private Transform playerTr;
    private Animator playerAni; public Animator GetPlayerAni() { return playerAni; }
    private Shot bulletShot;
    private Throw bombThrow;
    private Jump jump;
    private Dash dash;
    private Move move;
    private Wound wound;
    [SerializeField]
    private STATE eState;           public string TempStateReturn() { return eState.ToString(); }
    private STATE ePreState;

    private Player2_Controller controller;

    private CapsuleCollider playerCol;
   

    private readonly int MAXPLAYERBULLETCOUNT = 40;
    [SerializeField]
    private int shotDamage = 10;

    [SerializeField]
    private bool isKey = false;
    [SerializeField]
    private bool isMouse;
    private bool isJumpHit;         //점프할때 피격이 가능한지 false이면 가능
    public bool IsJumpHit() { return isJumpHit; }
    [SerializeField]
    private bool isJump;   //점프중인지 단순확인 
    private bool isAttackStop;
    [SerializeField]
    private bool isRun;
    private bool isDash;
    [SerializeField]
    private bool isMove;
    [SerializeField]
    private bool isRoll;
    private bool isAttackMode = false;
    private bool isReload=false;
    //상의 하의 유용할수 있는 변수
    private bool isTop = false;
    private bool isDown = false;
    public static Player2 instance;     //조심해서 써야함

    GameSystem system;
    private Particle particle;
    public bool GetIsAttackMode()
    {
        return isAttackMode;
    }
    void Start ()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        instance = this;
        playerRb = GetComponent<Rigidbody>();
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

        controller = GetComponent<Player2_Controller>();
        particle = GetComponent<Particle>();
        particle.Init("RollWaveEffect");

        wound = GetComponent<Wound>();
        //현재상태 // 이전상태 
        eState = STATE.STAND;
        ePreState = STATE.STAND;
       
        //is로 시작하는 bool변수는 false일때 해당변수를 안하고있다는뜻 isRoll 가 false라면 안굴고 있다는뜻
        isMouse = false;            //true이면 마우스를 사용못한다는뜻
        isRoll = false;
        isJump = false;
        isJumpHit = false;
        isAttackStop = false;
        isRun = false;
        isDash = false;
        isMove = false;
        bulletCount = 35;       //현재 35발쏘고 장전

        
        isReload = false;
        constraints = playerRb.constraints;
    }
    RigidbodyConstraints constraints;
    // Update is called once per frame
    void Update () {

        Input_MouseRight();
        Attack();
        //구르기
        Rolling();
        //달리기
        Run_Rotation();

        //점프 현재보류상태
        Jumping();
        //장전
        Reloading();
        //대시
        Move_Dash();
        Move_Update();
        Input_Move_Run();
        Input_Move_Walk();
        Logic();

        Change_Gun();
        Dancing();
        Update_Animation_Parameter();
        Render();
    }

    //마우스 바라보기

    void FixedUpdate()
    {
        Gravity();
    }
    //공격모드인지 확인
    private void Input_MouseRight()
    {
        if (isRoll || isJump) return;
        if (controller.Is_Input_AttackMode())
        {

            isAttackMode = true;

            Vector3 aim1 = system.MousePoint();
            float rotateDegree = Mathf.Atan2(aim1.x - transform.position.x, aim1.z - transform.position.z) * Mathf.Rad2Deg;
            playerTr.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
        }
        else isAttackMode = false;
    }


    //이동값설정
    private void Move_Update()           
    {
        if (isDash) return;
        if (isKey) return;
        move.Horizontal = Input.GetAxis("Horizontal");
        move.Vertical = Input.GetAxis("Vertical");
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isJump || isKey) return;
            eState = STATE.JUMP;
            playerAni.SetTrigger("Jump");
            isJump = true;
            isMouse = true;
            isKey = true;

        }
    }
    private void Input_Move_Walk()
    {
        if (!isAttackMode) return;   //오른쪽마우스버튼(공격모드)일때가 아니면 걷지마
        if (isKey) return;
        if (controller.Is_Input_WASD())
        {
            isMove = true;
            eState = STATE.WALK;
            isRun = false;
            isMouse = false;
        }
    }
    private void Input_Move_Run()       //키보드입력시 상태변경  기본 Run
    {
        if (isAttackMode) return;
        if (isDash) return;     //대시중일떄 뛰지마
        if (isRoll || isJump ) return;    //구를떄 뛰지마
        if (isKey) return;
        if (controller.Is_Input_WASD())
        {
            isMove = true;
            isRun = true;
            isMouse = true;
            eState = STATE.RUN;
        }
        else
        {
            isRun = false;
            isMove = false;
            isMouse = false;
            eState = STATE.STAND;
            move.Set_Zero();
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
            case STATE.STAND:
                Debug.Log(eState);
                
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
        //playerAni.SetFloat(hashAngle, playerTr.rotation.eulerAngles.y);       //이게뭐임?
        playerAni.SetFloat(hashX, move.Horizontal);
        playerAni.SetFloat(hashZ, move.Vertical);
        if (eState == STATE.STAND && !isAttackMode) { 
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;
            playerRb.constraints = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            playerRb.constraints = constraints;
        }

    }
   
    //달리기
    [SerializeField]
    private float velocity = 0.0f; //가속도   
    private void Run_Rotation()        //가속도, 캐릭터 회전 적용 함수 
    {
        if (isDash) return;
        if (!isRun) return;
        if (isKey) return;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 1.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.A)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, -100.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.D)) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 100.0f, 0.0f), Time.deltaTime * 10.0f);
    }
    public enum EFFECT_TYPE { ROLL }

    private void Update_Animation_Parameter()
    {
        float dest = controller.Get_f_Run_Sprint();
        if (velocity < dest && isMove)
        {
            velocity += Time.deltaTime; //  * 10.0f; 

            velocity = Check.Clamp(velocity, dest);
            
        }
        else
        {
            if(velocity != dest)
                velocity -= Time.deltaTime * 2.0f;
            if (velocity < 0.0f) velocity = 0.0f;
            
        }
        playerAni.SetFloat(hashVelocity, velocity);
    }
   


    //진짜구르기
    private void Rolling()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isRoll ||isKey) return;
            playerAni.SetTrigger("IsRoll");
            isMouse = true;
            isRoll = true;
            ePreState = eState;
            eState = STATE.ROLL;
            isKey = true;
            particle.Activate(transform.position);
        }
    }

    int bulletCount = 20;           //총알
    bool attackCoolTime = false;
    //기본공격
    private void Attack()
    {
        if (isDash) return;
        if (isReload) return;
        if (isAttackStop) return;
        if (!isAttackMode) return;
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
        else if(Input.GetKeyDown(KeyCode.B))
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
    
    public void Wound_Effect()
    {
        wound.Wound_Effect();
    }
    void OnTriggerEnter(Collider _obj)
    {
        if (_obj.CompareTag("Bullet"))
        {
            wound.TriggerEnter(_obj);
        }
    }

    void Move_Dash()
    {
        if (isKey) return;
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

    int gunMode = 0;
    void Attack_Gun()
    {
        if (gunMode == 0)
            bulletShot.Work(TYPE.BULLET);
        else if (gunMode == 1)
            bulletShot.Work(TYPE.ADVANCEBULLET);
    }

    void Change_Gun()
    {
        if (isKey) return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            gunMode = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            gunMode = 1;

    }

}
