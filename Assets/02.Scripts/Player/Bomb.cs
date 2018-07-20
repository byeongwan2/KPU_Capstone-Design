using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    float bombPower = 0.0f;
    public void SetPower(float _power){  bombPower = _power; }
    Vector3 velocity = new Vector3(0.5f, 0.4f);

    Rigidbody this_rigidbody;
    void Start () {
        this_rigidbody = GetComponent<Rigidbody>();

    }
	

	void Update () {
        this_rigidbody.AddForce(velocity);
    }

    public void Work()
    {
      
    }
}
