using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2_Controller : MonoBehaviour {

    public bool Is_Input_WASD()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
    }

    public bool Is_Input_AttackMode()
    {
        return Input.GetMouseButton(Define.MOUSE_RIGHT_BUTTON);
    }


   
    
}
