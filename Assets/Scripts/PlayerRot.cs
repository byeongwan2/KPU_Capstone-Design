﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRot : MonoBehaviour {

    private float r = 0.0f;

    private Transform tr;

    public float rotSpeed = 150.0f; //회전 속도

	void Start () {

        tr = GetComponent<Transform>(); //Player Transform 컴포넌트 할당
	}
	
	
	void Update () {
        r = Input.GetAxis("Mouse X");

        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전
	}
}
