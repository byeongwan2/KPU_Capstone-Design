using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Aim : MonoBehaviour {

	void Start () {
	}
		
	void Update () {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
	}

}
