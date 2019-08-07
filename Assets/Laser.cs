using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private GameObject startPosition;
    private LineRenderer lr;
    private GameSystem system;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }

    // 레이저 공격모드일때만 켜진다
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);

        float rayDistance;

       

        Vector3 point = Vector3.zero;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            point = ray.GetPoint(rayDistance);
            transform.LookAt(point);
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit,200.0f, (-1) - (1 << 15)))
        {
            if (hit.collider)
            {
                
                    lr.SetPosition(1, hit.point);

            }
        }


       
    }
}
