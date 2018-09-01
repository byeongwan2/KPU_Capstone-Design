using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	[ClientCallback]
	void LateUpdate () {
        return;
        if (!isLocalPlayer) return;
        Vector3 v = transform.position;
        v.z = 0;
        v.y += 17;
        v.x += 5;
        Camera.main.transform.position = v;
	}
}
