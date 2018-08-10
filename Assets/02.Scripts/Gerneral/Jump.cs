using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {


    private bool isJumping = false;    
    private float gravity = 25.0f;     //중력
    private float gravityProportion = 0.03f;
   public float airborneSpeed;


    private Rigidbody this_rigidbody;
   

    void Start()
    {          

        this_rigidbody = GetComponent<Rigidbody>();
        airborneSpeed = -10.0f;
        isJumping = false;

    }

    void Update ()
    {
        if (isJumping)
        {
            airborneSpeed -= gravity * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = false;
            
        }
        
    }

                
    public void Action(float _jumpForce, float _jumpSpeed)
    {
        if (isJumping) return;
        
        this_rigidbody.AddForce(new Vector3(0, _jumpForce, 0) * _jumpSpeed ,ForceMode.VelocityChange);
        airborneSpeed = 15.0f;
        
        isJumping = true;
    }
}
