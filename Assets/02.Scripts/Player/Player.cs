using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private float r = 0.0f;

    private Transform playerTr;

    public float rotSpeed = 250.0f; //회전 속도
                                    // Use this for initialization
    void Start()
    {

        playerTr = GetComponent<Transform>(); //Player Transform 컴포넌트 할당
    }


    void Update()
    {
        r = Input.GetAxis("Mouse X");

        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전
    }

}
