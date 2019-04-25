using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.red;
    public float _radius = 0.1f;
    // Use this for initialization
    Vector3 vec = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
        if(vec != Vector3.zero) Gizmos.DrawSphere(vec, _radius);
    }
  
}
