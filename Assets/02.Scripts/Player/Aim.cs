using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Aim : MonoBehaviour {

    private Transform tr;
	
	void Start () {
        tr = GetComponent<Transform>();	
	}
		
	void Update () {
      //  tr.position = Input.mousePosition;
	}
    public void Debugg(Vector3 vec)
    {
        tr.position = vec;
    }
}
