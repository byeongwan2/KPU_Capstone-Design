using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.red;
    public float _radius = 0.1f;
    // Use this for initialization
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
        if(vec != Vector3.zero) Gizmos.DrawSphere(vec, _radius);
    }

    MoveAgent agent = null;
    void Start()
    {
        if (GetComponent<Enemy>() == null) return;      //이 기즈모가 적에붙어있는게 아니라면 하지말것 왜냐면 디버깅용이니까
        agent = GetComponent<MoveAgent>();
    }

    Vector3 vec = Vector3.zero;
    void Update()
    {
        if (agent == null) return;
    }
}
