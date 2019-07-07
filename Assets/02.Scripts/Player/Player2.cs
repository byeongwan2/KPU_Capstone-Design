using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//함수는 동사_단어 혹은 동사 
//코루틴함수는 동사단어  
//이벤트함수는 Event_수식 혹은 Event_동사

//단어 수식 명사 등등 

//변수는 소문자시작 언더바없고 기본변수는 두단어이상 
//클래스이름은 대문자시작 언더바없음



public partial class Player2 : MoveObject
{
    private Rigidbody rb;
    private Transform tr;
    private Animator ani;
    public Animator GetPlayerAni() { return ani; }
    private Shot shot;
    private Throw bombThrow;
    private Jump jump;
    private Dash dash;
    private Move move;
    private Wound wound;
    private CameraShake cameraShake;
    [SerializeField]
    private STATE eState;
    private STATE ePreState;

    private Player2_Controller controller;

    private readonly int MAXPLAYERBULLETCOUNT = 15;
    [SerializeField]
    private int shotDamage = 10;

    [SerializeField]
    private bool isKey = false;
    [SerializeField]
    private bool isMouse = false;
    private bool isJumpHit;         //점프할때 피격이 가능한지 false이면 가능
    public bool IsJumpHit() { return isJumpHit; }
    [SerializeField]
    private bool isJump;   //점프중인지 단순확인 
    private bool isAttackStop;
    [SerializeField]
    private bool isRun = false;
    private bool isDash = false;
    [SerializeField]
    private bool isMove = false;
    [SerializeField]
    private bool isRoll = false;
    private bool isAttackMode = false;
    private bool isReload = false;
    //상의 하의 유용할수 있는 변수
    private bool isTop = false;
    private bool isDown = false;
    public static Player2 instance;     //조심해서 써야함

    GameSystem system;
    private Particle particle;
    private int bombCount =10;
    int bulletCount = 35;           //총알
    private int bulletAdvancedCount;
    public bool GetIsAttackMode()
    {
        return isAttackMode;
    }
    void Start()
    {
        cameraShake = GameObject.Find("Camera_ViewPoint").GetComponent<CameraShake>();
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        instance = this;
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        ani = GetComponent<Animator>();


        shot = GetComponent<Shot>();
        shot.Init("PlayerBasicBullet", MAXPLAYERBULLETCOUNT, 20.0f, shotDamage, TYPE.BULLET,Object_Id.PLAYER);
        shot.Init("PlayerAdvanceBullet", MAXPLAYERBULLETCOUNT,0, shotDamage, TYPE.ADVANCEBULLET, Object_Id.PLAYER);
        bombThrow = GetComponent<Throw>();
        bombThrow.Init("PlayerBomb", bombCount, 2.0f);
        jump = GetComponent<Jump>();
        dash = GetComponent<Dash>();
        move = GetComponent<Move>();

        controller = GetComponent<Player2_Controller>();
        particle = GetComponent<Particle>();
        particle.Init("RollWaveEffect");

        wound = GetComponent<Wound>();
        wound.Init(100);
        //현재상태 // 이전상태 
        eState = STATE.STAND;
        ePreState = STATE.STAND;

        isJump = false;
        isJumpHit = false;
        isAttackStop = false;
        bulletCount = 35;       //현재 35발쏘고 장전
        bombCount = 10;
        bulletAdvancedCount = 25;

        isReload = false;
        constraints = rb.constraints;
        Start_Sound();
        laser = GetComponentInChildren<Laser>();
        laser.gameObject.SetActive(false);
    }
    RigidbodyConstraints constraints;
    // Update is called once per frame
    void Update() {

        Input_MouseRight();
        Proceed_Event();
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

    Laser laser;
    //공격모드인지 확인
    private void Input_MouseRight()
    {
        if (isRoll || isJump) return;
        if (controller.Is_Input_AttackMode())
        {

            isAttackMode = true;
            laser.gameObject.SetActive(true);
            Vector3 aim1 = system.MousePoint();
            float rotateDegree = Mathf.Atan2(aim1.x - tr.position.x, aim1.z - tr.position.z) * Mathf.Rad2Deg;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
        }
        else
        {
            isAttackMode = false;
            laser.gameObject.SetActive(false);
        }
    }
    //플레이어가 이벤트를 발생시킴
    void Proceed_Event()
    {
        if (controller.Is_Input_EventMode())
        {
            StageManager.instance.Input_EventKey();
        }
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
            ani.SetTrigger("Jump");
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
        if (isRoll || isJump) return;    //구를떄 뛰지마
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
                ani.SetBool("Dash", true);
                break;
            case STATE.RUN:
                ani.SetBool("IsRun", true);
                ani.SetBool("IsWalk", false);
                break;
            case STATE.WALK:
                ani.SetBool("IsWalk", true);
                ani.SetBool("IsRun", false);
                ani.SetBool("Dash", false);
                break;

            case STATE.STAND:
                ani.SetBool("IsRun", false);
                ani.SetBool("IsWalk", false);
                ani.SetBool("Dash", false);

                break;
        }
        ani.SetFloat(hashAngle, tr.rotation.eulerAngles.y);       //이게뭐임?
        ani.SetFloat(hashX, move.Horizontal);
        ani.SetFloat(hashZ, move.Vertical);
        if (eState == STATE.STAND && !isAttackMode) {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            rb.constraints = constraints;
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

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, -60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, 60.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, -145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, 145.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.W)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, 1.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.S)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.A)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, -100.0f, 0.0f), Time.deltaTime * 10.0f);
        else if (Input.GetKey(KeyCode.D)) tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0.0f, 100.0f, 0.0f), Time.deltaTime * 10.0f);
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
            if (velocity != dest)
                velocity -= Time.deltaTime * 2.0f;
            if (velocity < 0.0f) velocity = 0.0f;

        }
        ani.SetFloat(hashVelocity, velocity);
    }



    //진짜구르기
    private void Rolling()
    {
        if (isMove == false) return;
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isRoll || isKey) return;
            ani.SetTrigger("IsRoll");
            isMouse = true;
            isRoll = true;
            ePreState = eState;
            eState = STATE.ROLL;
            isKey = true;
            particle.Activate(tr.position);
        }
    }

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
                ani.SetTrigger("Reload");
                isReload = true;
                bulletCount = 35;
                bulletAdvancedCount = 25;
                return;
            }
            bulletCount--;
            attackCoolTime = true;
            ani.SetTrigger("Attack");
            Attack_Gun();
            
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            bombCount--;
            ani.SetTrigger("Throw");
            isMouse = true;
            isAttackStop = true;
            Update_UI_Bomb();
        }
    }


    private void Dancing()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { ani.SetInteger("Dance", 1); }
        if (Input.GetKeyDown(KeyCode.F2)) { ani.SetInteger("Dance", 2); }
        if (Input.GetKeyDown(KeyCode.F3)) { ani.SetInteger("Dance", 3); }
    }

    private void Reloading()
    {
        if (isDash) return;
        if (isReload) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReload = true;
            ani.SetTrigger("Reload");
            bulletCount = 35;
            bulletAdvancedCount = 25;
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
            if (_obj.GetComponent<Bullet>().Get_ID() == Object_Id.PLAYER) return;
            wound.TriggerEnter(_obj);
        }
    }

    void Move_Dash()
    {
        return;
        if (isKey) return;
        if (Input.GetKey(KeyCode.C))
        {
            isDash = true;
            eState = STATE.DASH;
            Vector3 aim1 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            dash.Dash_Destination(aim1, 40.0f);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            eState = STATE.STAND;
            isDash = false;
        }
    }

    int gunMode = 0;
    void Attack_Gun()
    {
        if (gunMode == 0)
            StartCoroutine(Time_Submachine_Gun());

        else if (gunMode == 1)
        {
            bulletAdvancedCount--;
            shot.Work(TYPE.ADVANCEBULLET);
            Update_UI_Bullet();
            Sfx();
        }
    }

    IEnumerator Time_Submachine_Gun()
    {
        int count = 0;
        Sfx();
        while (true)
        {
            shot.Work(TYPE.BULLET);
            yield return new WaitForSeconds(0.05f);
            count++;
            Update_UI_Bullet();
            cameraShake.SetDruation(0.01f); // 카메라 흔들림 0.01은 흔들리는 시간
            if (count == 4) yield break;
        }
    }

    void Change_Gun()
    {
        if (isKey) return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunMode = 0;
            Update_UI_Bullet();
            currWeapon = WeaponType.RIFLE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunMode = 1;
            Update_UI_Bullet();
            currWeapon = WeaponType.SHOTGUN;
        }
        

    }
    public Image magazineImg_bullet;
    public Text magazineText_bullet;

    void Update_UI_Bullet()
    {
        if (gunMode == 0)
        {
            magazineImg_bullet.fillAmount = (float)bulletCount / (float)35;
            magazineText_bullet.text = string.Format(bulletCount.ToString());
        }
        else if (gunMode == 1)
        {
            magazineImg_bullet.fillAmount = (float)bulletAdvancedCount / (float)25;
            magazineText_bullet.text = string.Format(bulletAdvancedCount.ToString());
        }
    }
    public Text magazineText_bomb;
    void Update_UI_Bomb()
    {
        magazineText_bomb.text = string.Format(bombCount.ToString());
    }

    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN = 1
    }
    public WeaponType currWeapon = WeaponType.RIFLE;
    private AudioSource _audio;

    public PlayerSfx playerSfx;
    void Start_Sound()
    {
        _audio = GetComponent<AudioSource>();
    }
    void Sfx()
    {
        var _sfx = playerSfx.fire[(int)currWeapon];
        _audio.PlayOneShot(_sfx, 1.0f);
    }
}
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}