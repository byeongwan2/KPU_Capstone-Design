using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDir { LEFT,RIGHT,FORWARD,BACK,UP,DOWN}
public class Door : MonoBehaviour
{
    bool isActivate;
    public float dest;
    Vector3 moveDir ;
    // Start is called before the first frame update
    void Start()
    {
        isActivate = false;
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
            case eDir.UP:
                moveDir = Vector3.up;
                break;
            case eDir.DOWN:
                moveDir = Vector3.down;
                break;
        }
    }
    bool IsActivate()
    {
        if (!isActivate) return false;
        if (moveDir == Vector3.forward)
            if (transform.position.z > dest)
            {
                Active(0.0f);
                return false;
            }
            else return true;
        else if (moveDir == Vector3.back)
            if (transform.position.z < dest)
            {
                Active(0.0f);
                return false;
            }
            else return true;
        else if (moveDir == Vector3.up)
            if (transform.position.y > dest)
            {
                Active(0.0f);
                return false;
            }
            else return true;
        else if (moveDir == Vector3.down)
            if (transform.position.y < dest)
            {
                Active(0.0f);
                return false;
            }
            else return true;
        return false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsActivate()) return;
     
        transform.Translate(moveDir.normalized * 0.8f * Time.deltaTime, Space.World);
    }

    public void Active(float _data)
    {
        if (isActivate) isActivate = false;
        else isActivate = true;
    }

    public void Close()
    {
        if (moveDir == Vector3.forward)
        {
            moveDir = Vector3.back;
            dest -= 2.5f;
        }
        else if (moveDir == Vector3.back)
        {
            moveDir = Vector3.forward;
            dest += 2.5f;
        }
        else if(moveDir == Vector3.down)
        {
            moveDir = Vector3.up;
            dest += 4.0f;
        }
        else if (moveDir == Vector3.up)
        {
            moveDir = Vector3.down;
            dest -= 4.0f;
        }
        if (isActivate) isActivate = false;
        else isActivate = true;
    }   
}
