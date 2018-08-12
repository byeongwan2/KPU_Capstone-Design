using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour{

    public Transform playerTr;              //Player Transform 컴포넌트    
    public float moveSpeed = 15.0f;         //카메라 이동 속도   
    public float rotateSpeed = 10.0f;       //카메라 회전 속도
    public float distance = 10.0f;           //카메라와 주인공과의 거리
    public float height = 1.0f;             //카메라 높이
    public float playerOffset = 1.0f;       //Player 좌표의 오프셋
    
    private Transform tr;   //카메라 Transfrom 컴포넌트

    void Start () {        
        tr = GetComponent<Transform>();                
	}
	
	
    //주인공이 이동한 후 카메라가 움직여야하기 떄문에 LateUpdate를 쓴다
	void LateUpdate () {

        //카메라 위치 계산
        var camPos = playerTr.position - (playerTr.forward * distance) + (playerTr.up * height);

        //이동 속도 계산
        tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * moveSpeed);

        //회전 속도 계산
        tr.rotation = Quaternion.Slerp(tr.rotation, playerTr.rotation, Time.deltaTime * rotateSpeed);

        //카메라가 Player의 머리를 기준
        tr.LookAt(playerTr.position + (playerTr.up * playerOffset));
        

	}
}
