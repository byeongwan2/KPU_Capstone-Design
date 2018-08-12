using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : AttackObject {
    [SerializeField]
    float power = 0.0f;
   
    public void SetPower(float _power){ power = _power; }
    private Vector3 velocity;
    public void SetVelocity(Vector3 _velocity)  { velocity = _velocity; }

    Rigidbody this_rigidbody;
    void Awake()
    {
        this_rigidbody = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐
    }
    void LifeOff()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
        gameObject.SetActive(false);
    }
	
        
	void FixedUpdate () {                           //폭탄 그자체가 날라가는 코드를 기입하면댐
        if(gameObject.activeSelf == false) return;          
        // this_rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
    }

    public void SetActiveLaunch()          //폭탄이 켜지면서 초기화
    {
        transform.position = launchPos;
        transform.rotation = launchRot;
        this_rigidbody.AddForce(transform.forward * 15.0f, ForceMode.VelocityChange);       //포물선수정필요
        Invoke("LifeOff", 5.0f);        //2초뒤 폭탄삭제
    }

    public override void StatSetting()
    {

    }
}
