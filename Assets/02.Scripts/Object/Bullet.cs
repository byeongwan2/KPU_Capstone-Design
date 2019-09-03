using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어가 쏘는 총알은 모두 이 컴포넌트를 사용 그러니까 1번2번총알 모두 여기 스크립트를 사용
public class Bullet : AttackObject {

    Rigidbody rb;
    public Object_Id fire_ObjectId;
    TYPE mType;

    void Awake()
    {       
        rb = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐    
    }

    //총알이 누구의 총알인지 반환
    public Object_Id Get_ID()
    {
        return fire_ObjectId;
    }
 
    //총알이 누구의 총알인지 등록         //자기가쏜총알을 자기가 맞고 체력이 소모되는 버그수정
    public void Resister_ID(Object_Id _id)
    {
        fire_ObjectId = _id;
    }

    //총알은 사라질때 자신의 위치등을 초기화
    void OnDisable()
    {
        
        if(mType == TYPE.ADVANCEBULLET)
        {
            EffectManager.Instance.Exercise_Effect(transform.position, 0.0f);
        }
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
        CancelInvoke();
    }
    
    // 총알의 수명종료
    public void LifeOff()
    {
        gameObject.SetActive(false);
    }
    Vector3 destination; 
    
    //총알이 발사될떄 날라가는 방법   서로 다른 두 총알은 날아가는 방식이다름
    public void SetActiveLaunch(TYPE _type)          //총알이 켜지면서 초기화
    {
        mType = _type;
        transform.position = launchPos;
       
        if (_type == TYPE.BOSSBULLET)
        {
            var v = GameObject.Find("Boss").GetComponent<Boss>().shotSecondPosition.transform.position;
            transform.LookAt(v);
            Invoke("Attack_BossBullet", 3.0f);        //3초뒤 총알삭제
            return;
        }
        if (_type == TYPE.ROBOTBEAM)
        {
            SetActiveLaunch_Enemy();
            Invoke("DestroyBeam", 1.5f);
            return;
        }

        if (_type == TYPE.ENEMYBULLET)
        {
            SetActiveLaunch_Enemy();
            Invoke("LifeOff", 2.0f);        //2초뒤 총알삭제
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);

        float rayDistance;

        Vector3 point = Vector3.zero ;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            point = ray.GetPoint(rayDistance);
            transform.LookAt(point);
        }
        

        if (_type == TYPE.BULLET)
        {
            Invoke("LifeOff", 2.0f);        //2초뒤 총알삭제
        }
        if(_type == TYPE.ADVANCEBULLET)
        {
            // point.x = point.x * 1.0f;
            //point.z = point.z * 1.0f;
             rb.AddForce(new Vector3(0,1,0)* 4.0f, ForceMode.Impulse);
            //Vector3 direction = body.transform.position - transform.position;
            //rb.AddForceAtPosition(Vector3.forward,,ForceMode.Impulse);
            speed = 4.0f;
            Invoke("Explode_Bullet", 2.0f);
        }
        
    }
    void DestroyBeam()
    {
        Destroy(gameObject);
    }
    //적군이 쏘는 총알이 발사될때
    void SetActiveLaunch_Enemy( )
    {
        Vector3 target = PrefabSystem.instance.player.transform.position;
        target.y = target.y + 1.5f;
        transform.LookAt(target);
        float y = transform.rotation.y;
        //Quaternion vec = Quaternion.Euler(Vector4.zero) ;
        //vec.y = y;
        //transform.rotation = vec;
    }

    void Explode_Bullet()
    {
        //EffectManager.Instance.Exercise_Effect(transform.position, 0.0f);
        gameObject.SetActive(false);
    }


    [SerializeField]
    float speed = 10.0f;
    [SerializeField]
    int damage = 0;
    public int Damage
    {
        get{ return damage; }
    }
    bool isFall_Bullet = false; //보스총알은 위에서 아래로 떨어진다
    //1번총알이 작동하는 방법
    void FixedUpdate()
    {
        if ( mType == TYPE.BULLET  || mType == TYPE.ENEMYBULLET || mType == TYPE.ROBOTBEAM)
        {
            transform.localPosition += transform.forward * speed * Time.deltaTime;
        }
        //보스총알
        if(mType == TYPE.BOSSBULLET && isFall_Bullet == false)
        {
            transform.localPosition += transform.forward * 1.5f * Time.deltaTime;
        }
        else if(mType == TYPE.BOSSBULLET && isFall_Bullet == true)
        {
            transform.localPosition += transform.forward * 5.0f * Time.deltaTime;
        }
        
    }
    [SerializeField]
    RangeEffect rangeEffect = null;
    public void Attack_BossBullet()
    {
        transform.LookAt(PrefabSystem.instance.player.transform.position);
        isFall_Bullet = true;
        //Invoke("LifeOff");
        rangeEffect = Instantiate(Resources.Load<GameObject>("Prefabs/Range")).GetComponent<RangeEffect>();
        rangeEffect.Init();
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("StaticObject");
        if (Physics.Raycast(transform.position, transform.forward,out hit,15.0f, layerMask))
        {
            rangeEffect.transform.position = hit.point;
            Vector3 v =  rangeEffect.transform.position;
            v.y += 0.5f;
            rangeEffect.transform.position = v;
            rangeEffect.RangeLook(0.0f);
        }
      
    }

    //이 총알의 데미지를 설정     이 총알을 쓰는 주체가 호출
    public void DamageSetting(int _damage)
    {
        damage = _damage;
    }

    //이 총알의 스피드를 설정     이 총알을 쓰는 주체가 호출
    public void SpeedSetting(float _speed )
    {
        speed = _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mType != TYPE.BOSSBULLET) return;   //일단 보스의 총알만 사용하는 함수   //그전엔 어떻게처리했는지 기억이잘안남

        if (other.gameObject.layer == 11)
        {
            Destroy(rangeEffect.gameObject);
            EffectManager.Instance.Exercise_Effect(transform.position, 0.0f);
            isFall_Bullet = false;
            if (2.6f > Check.Distance(transform.position, PrefabSystem.instance.player.transform.position))
            {
                PrefabSystem.instance.player.GetComponent<Wound>().GetDamage(10);
            }
            LifeOff();

        }

    }
}
