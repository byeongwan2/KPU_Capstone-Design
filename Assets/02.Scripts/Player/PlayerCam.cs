using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour{
 
    private Transform aimTr;
    public float moveSpeed = 15.0f;         //카메라 이동 속도       
    public float distance = 15.0f;           //카메라와 주인공과의 거리
    public float height = 25.0f;             //카메라 높이
    public float playerOffset = 1.0f;       //Player 좌표의 오프셋

    void Start ()
    {
        aimTr = GameObject.Find("Camera_ViewPoint").GetComponent<Transform>();
    }


	
    //주인공이 이동한 후 카메라가 움직여야하기 떄문에 LateUpdate를 쓴다
	void LateUpdate ()
    {
        transform.position = aimTr.position - 1 * (Vector3.forward * distance) + (Vector3.up * height);
        
        transform.LookAt(aimTr.position);
        /*
        Vector3 vec = aimTr.position - 1 * (Vector3.forward * distance) + (Vector3.up * height);
        float x = Mathf.Lerp(transform.position.x, vec.x, Time.deltaTime);
        float z = Mathf.Lerp(transform.position.z, vec.z, Time.deltaTime);

        Vector3 vec2 = aimTr.position - transform.position;
        vec2.Normalize();

        Quaternion q = Quaternion.LookRotation(vec2);
        q.y = 0.0f;
        q.z = 0.0f;
        //q.x = 63.435f;
        transform.rotation = q;

        vec.x = x;
        vec.z = z;
        transform.position = vec;
        */
       



        /*
        //카메라 위치 계산
         var camPos = playerTr.position - (playerTr.forward * distance) + (playerTr.up * height);

        //이동 속도 계산
        // tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * moveSpeed);        

        //카메라가 Player의 머리를 기준
         tr.LookAt(playerTr.position + (playerTr.up * playerOffset));
         */
    }
}
