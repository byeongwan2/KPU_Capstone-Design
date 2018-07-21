using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [SerializeField]
    float bombPower = 0.0f;
    public void SetPower(float _power){  bombPower = _power; }
   // Vector3 velocity = new Vector3(0.2f, 0.2f,0.0f);

    Rigidbody this_rigidbody;
    void Start () {
        this_rigidbody = GetComponent<Rigidbody>();
        this_rigidbody.AddForce(bombPower * transform.forward);
    }
	

	void FixedUpdate () {
     
    }

    public void Work()
    {
      
    }
}
