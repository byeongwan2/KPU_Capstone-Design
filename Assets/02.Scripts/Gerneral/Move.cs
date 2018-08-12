using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float Vertical { get; set; }     //상하 이동방향
    public float Horizontal { get; set; }   //좌우 이동방향
    //김병완 ㅋ.ㅋ
    //주인공 Transform 컴포넌트 변수
    private Transform tr;

    //이동 속도
	
	void Start () {
        Vertical = 0.0f;
        Horizontal = 0.0f;
        tr = GetComponent<Transform>(); //tr에 주인공 Transform 할당
	}
	
	
	void Update ()
    {
        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * Vertical) + (Vector3.right * Horizontal);
        //Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
        tr.Translate(moveDir.normalized *moveSpeed * Time.deltaTime, Space.Self);
        
    }

    private float moveSpeed = 0.0f;         
    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    public void SetZero()
    {
        Vertical = 0.0f;
        Horizontal = 0.0f;
        moveSpeed = 0.0f;
    }
}
