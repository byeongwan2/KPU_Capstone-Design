using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isActivate;
    float dest;
    // Start is called before the first frame update
    void Start()
    {
        isActivate = false;
        dest = transform.position.z + 2.5f ;       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActivate == false) return;
        Vector3 moveDir = (Vector3.forward );
        transform.Translate(moveDir.normalized * 0.7f * Time.deltaTime, Space.World);
        if (transform.position.z > dest)
            isActivate = false;
    }

    public void Active(float _data)
    {
        if (isActivate) isActivate = false;
        else isActivate = true;
    }
}
