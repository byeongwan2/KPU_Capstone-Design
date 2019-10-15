using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Behaviour {

    [SerializeField]
    private float moveSpeed = 0.0f;
    public float Vertical { get; set; }     //상하 이동방향
    [SerializeField]
    public float Horizontal { get; set; }   //좌우 이동방향
   

    //이동 속도	
	void Start () {
        Vertical = 0.0f;
        Horizontal = 0.0f;
        moveSpeed = 0.0f;
    }
	
    //이동
	void FixedUpdate ()
    {
        Vector3 moveDir = (Vector3.forward * Vertical) + (Vector3.right * Horizontal) ;
        transform.Translate(moveDir.normalized *moveSpeed * Time.deltaTime, Space.World);
    }

    //속도 설정        
    public void Set_MoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    //움직임 중단
    public void Set_Zero()
    {
        Vertical = 0.0f;
        Horizontal = 0.0f;
        moveSpeed = 0.0f;
    }
    
    public override void End()
    {

    }


}
