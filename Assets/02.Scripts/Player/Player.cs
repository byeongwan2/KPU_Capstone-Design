using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour {
    private float r = 0.0f;

    private Transform playerTr;
    private Rigidbody playerRb;
    private Jump jump;
    public float rotSpeed = 250.0f; //회전 속도

    [SerializeField]
    private STATE eState = STATE.STAND;
    private bool isDoubleJump = false;

  
 
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerTr = GetComponent<Transform>(); //Player Transform 컴포넌트 할당
        jump = GetComponent<Jump>();

        eState = STATE.STAND;
        isDoubleJump = false;

     
    }


    void Update()
    {
        r = Input.GetAxis("Mouse X");

        playerTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r); // Y축을 기준으로 rotSpeed 만큼 회전

        KeyboardManual();
        MoveManual();
    }


    private void KeyboardManual()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (eState == STATE.JUMP && isDoubleJump == true)
            {
                playerRb.AddForce(new Vector3(0, 1.6f, 0) * 5.0f, ForceMode.Impulse);
                isDoubleJump = false;
            }
            else if(eState != STATE.JUMP)
            {
                eState = STATE.JUMP;
                jump.Action(1.6f, 5.0f);     //점프력,점프스피드
                isDoubleJump = true;
            }
        }
       
        if(Input.GetKeyDown(KeyCode.Return))
        {
         //뭐를쓸까낭?
        }
     

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground" && eState == STATE.JUMP)
        {
            eState = STATE.STAND;
            isDoubleJump = false;
        }
    }

    void State()
    {
        
    }

    private void MoveManual()
    {
        if (eState == STATE.JUMP) return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            eState = STATE.WALK;
        }
        else
        {
             eState = STATE.STAND;

        }
    }
}
 