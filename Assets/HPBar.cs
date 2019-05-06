using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    Quaternion vec;
    private void Start()
    {
        vec = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = vec;
        
    }
}
