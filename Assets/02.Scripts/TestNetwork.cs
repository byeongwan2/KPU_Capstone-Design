using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestNetwork : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    [ClientCallback]
    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        Vector3 v = transform.position;
        v.z = 0;
        v.y += 17;
        v.x += 5;
        Camera.main.transform.position = v;
    }

    [ClientCallback]
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        CmdMoveBox(x, z);
    }
    
    [Command]
    public void CmdMoveBox(float x , float z)
    {
        Vector3 v = new Vector3(x, 0, z) * 10.0f;
        GetComponent<Rigidbody>().AddForce(v);
    }
}
