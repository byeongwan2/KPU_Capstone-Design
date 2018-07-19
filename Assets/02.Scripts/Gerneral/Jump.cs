using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {
    [SerializeField]
    private float jumpForce = 1.6f;
    [SerializeField]
    private float jumpSpeed = 5f;

    private bool isJumping = false;

    private Rigidbody this_rigidbody;
    // Use this for initialization
    void Start()
    {
        this_rigidbody = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        if (isJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this_rigidbody.AddForce(new Vector3(0, jumpForce, 0) * jumpSpeed, ForceMode.Impulse); //위방향으로 올라가게함
                isJumping = false;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
            isJumping = true;
    }
}
