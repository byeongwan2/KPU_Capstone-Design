using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : MonoBehaviour {

    private Transform tr;
    private float ry = 0.0f;
    public float rotSpeed = 200.0f; //회전 속도

    void Start () {
        tr = GetComponent<Transform>();
	}
	
	
	void Update () {		
        ry = Input.GetAxis("Mouse Y");
        tr.Rotate(Vector3.left * rotSpeed * Time.deltaTime * ry); 
    }
}
