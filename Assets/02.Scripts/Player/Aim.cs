using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Aim : MonoBehaviour {

    private Transform tr;
    private Vector3 mPos,mPos2;
	void Start () {
        tr = GetComponent<Transform>();	
	}
		
	void Update () {
        Vector3 mPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        tr.position = mPos;
	}

}
