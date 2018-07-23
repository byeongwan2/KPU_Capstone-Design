using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    private Vector3 launchPos;
    public void SetLaunchPos(Vector3 _launchPos)                //폭탄이 생성되는 위치
    {
        launchPos = _launchPos;
    }
    [SerializeField]
    float power = 0.0f;
   
    public void SetPower(float _power){ power = _power; }
    private Vector3 velocity;
    public void SetVelocity(Vector3 _velocity)  { velocity = _velocity; }

    Rigidbody this_rigidbody;
    void Start () {
        this_rigidbody = GetComponent<Rigidbody>();
       
    }
	
        
	void FixedUpdate () {                           //폭탄 그자체가 날라가는 코드를 기입하면댐
        this_rigidbody.AddForce(power * transform.forward);
    }

  
}
