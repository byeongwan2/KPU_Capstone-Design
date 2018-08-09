using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting  : MonoBehaviour{

    private SmoothFollow mainCamera;

    void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<SmoothFollow>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
           mainCamera.rotationDamping -= 0.5f;
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
           mainCamera.rotationDamping += 0.5f;
        }    
    }
}
