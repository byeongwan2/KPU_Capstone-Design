using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {


    private bool isJumping = false;
   

    private Rigidbody this_rigidbody;
    // Use this for initialization
    void Start()
    {
        this_rigidbody = GetComponent<Rigidbody>();
   
        isJumping = false;
    }

    void Update ()
    {
       
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = true;

        }
    }

    public void Action(float _jumpForce, float _jumpSpeed)
    {
        if (isJumping)
        {
            this_rigidbody.AddForce(new Vector3(0, _jumpForce, 0) * _jumpSpeed, ForceMode.Impulse);
            isJumping = false;
        }
       
    }
}
