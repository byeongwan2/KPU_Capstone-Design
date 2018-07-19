using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    private float h = 0.0f;     //좌우 이동방향
    private float v = 0.0f;     //상하 이동방향
    //김병완 ㅋ.ㅋ
    //주인공 Transform 컴포넌트 변수
    private Transform tr;

    //이동 속도
    public float moveSpeed = 10.0f;
	
	void Start () {
        tr = GetComponent<Transform>(); //tr에 주인공 Transform 할당
	}
	
	
	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

	}
}
