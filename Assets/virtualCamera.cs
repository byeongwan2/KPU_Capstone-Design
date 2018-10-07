using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class virtualCamera : MonoBehaviour {
    Transform tr;
    GameSystem system;
	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

    }
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 vec = system.pPlayer2.transform.position;
        vec.y += 20;
        tr.position = vec;
	}
}
