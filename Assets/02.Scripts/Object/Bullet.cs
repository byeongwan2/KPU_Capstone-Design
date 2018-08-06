using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Vector3 launchPos;
    public void SetLaunchPos(Vector3 _launchPos)
    {
        launchPos = _launchPos;
    }

    float speed = 1000.0f;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}
	
	
}
