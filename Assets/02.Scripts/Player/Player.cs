using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private float r = 0.0f;

    private Transform playerTr;

    private Jump jump;
    public float rotSpeed = 250.0f; //회전 속도
   
    void Start()
    {
        playerTr = GetComponent<Transform>(); //Player Transform 컴포넌트 할당
        jump = GetComponent<Jump>();
    }


    void Update()
    {
        r = Input.GetAxis("Mouse X");

        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전

        KeyboardManual();
    }


    private void KeyboardManual()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump.Action(1.6f,5.0f);     //점프력,점프스피드
        }
    }
   
}
 