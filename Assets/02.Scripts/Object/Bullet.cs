using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AttackObject {

    Rigidbody rb;
    Object_Id fire_ObjectId;
    
    void Awake()
    {       
        rb = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐    
        fire_ObjectId = Object_Id.NONE;
    }

    public void Check_BulletId(Object_Id _id)
    {
        fire_ObjectId = _id;
    }

    void OnDisable()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
    }
    private void LifeOff()
    {
        gameObject.SetActive(false);
    }
    Vector3 destination; 
    
    public void SetActiveLaunch(TYPE _type)          //총알이 켜지면서 초기화
    {
        transform.position = launchPos;
       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            transform.LookAt(point);
        }
        if (_type == TYPE.BULLET)
        {
            Invoke("LifeOff", 2.0f);        //2초뒤 총알삭제
        }
        if(_type == TYPE.ADVANCEBULLET)
        {
            rb.AddForce(new Vector3(1, 0, 0) * 5.0f, ForceMode.Impulse);
            Invoke("Explode_Bullet", 2.0f);
        }
    }
    
    void Explode_Bullet()
    {
        EffectManager.Instance.Exercise_Effect(transform.position, 0.0f);
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
