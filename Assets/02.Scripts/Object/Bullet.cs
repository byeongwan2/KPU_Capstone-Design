using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AttackObject {

    Rigidbody rb;
    public Object_Id fire_ObjectId;
    TYPE mType;
    void Awake()
    {       
        
        rb = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐    
    }
    public Object_Id Get_ID()
    {
        return fire_ObjectId;
    }
 

    public void Resister_ID(Object_Id _id)
    {
        fire_ObjectId = _id;
    }

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
    public void LifeOff()
    {
        gameObject.SetActive(false);
    }
    Vector3 destination; 
    
    public void SetActiveLaunch(TYPE _type)          //총알이 켜지면서 초기화
    {
        mType = _type;
        transform.position = launchPos;
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
    void FixedUpdate()
    {
        if (gameObject.activeSelf == false) return;
   
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        
    }

    public override void StatSetting()
    {
        
    }

    public void DamageSetting(int _damage)
    {
        damage = _damage;
    }
    public void SpeedSetting(float _speed )
    {
        speed = _speed;
    }
}
