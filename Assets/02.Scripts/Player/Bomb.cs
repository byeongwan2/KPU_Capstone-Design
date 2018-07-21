using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [SerializeField]
    float power = 0.0f;
   
    public void SetPower(float _power){ power = _power; }
    private Vector3 velocity;
    public void SetVelocity(Vector3 _velocity)  { velocity = _velocity; }

    Rigidbody this_rigidbody;
    void Start () {
        this_rigidbody = GetComponent<Rigidbody>();
       
    }
	

	void FixedUpdate () {
        this_rigidbody.AddForce(power * transform.forward);
    }

  
}
