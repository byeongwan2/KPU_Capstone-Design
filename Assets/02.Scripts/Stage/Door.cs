using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDir { LEFT,RIGHT,FORWARD,BACK}
public class Door : MonoBehaviour
{
    bool isActivate;
    float dest;
    Vector3 moveDir ;
    // Start is called before the first frame update
    void Start()
    {
        isActivate = false;
        dest = transform.position.z + 2.5f ;    
        //if(dir )
    }

    public void Setting(eDir _dir)
    {
        switch(_dir)
        {
            case eDir.FORWARD:
                moveDir = Vector3.forward;
                break;
            case eDir.BACK:
                moveDir = Vector3.back;
                break;
            case eDir.RIGHT:
                moveDir = Vector3.right;
                break;
            case eDir.LEFT:
                moveDir = Vector3.left;
                break;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActivate == false) return;

        transform.Translate(moveDir.normalized * 0.8f * Time.deltaTime, Space.World);
        if (transform.position.z > dest)
            isActivate = false;
    }

    public void Active(float _data)
    {
        if (isActivate) isActivate = false;
        else isActivate = true;
    }
}
