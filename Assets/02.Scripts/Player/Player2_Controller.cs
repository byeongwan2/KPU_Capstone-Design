using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 입력을 위한 스크립트
public class Player2_Controller : MonoBehaviour {

    public bool Is_Input_WASD()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
    }

    public bool Is_Input_AttackMode()
    {
        return Input.GetMouseButton(Define.MOUSE_RIGHT_BUTTON);
    }

    public float Get_f_Run_Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            return 3.0f;
        else return 1.0f;
    }

    public bool Is_Input_EventMode()
    {
        return Input.GetKeyDown(KeyCode.B);
    }
}
