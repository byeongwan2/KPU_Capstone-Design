using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    private GameSystem system;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, system.pPlayer2.transform.forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }
        }
        else lr.SetPosition(1, system.pPlayer2.transform.forward * 2);
    }
}
