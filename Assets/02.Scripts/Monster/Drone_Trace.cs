using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Trace : MonoBehaviour
{
    private Transform playerTr;
    private Transform enemyTr;


    // 주인공을 향해 회전 속도 계수
    private readonly float damping = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        enemyTr = GetComponent<Transform>();
    }

    public bool Trace()
    {
        // 회전각 계산
        Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
        
        // 보간 함수를 이용해 회전
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);

        Vector3 dir = playerTr.position - enemyTr.position;
        enemyTr.transform.position += dir.normalized * Time.deltaTime;
        return true;
    }
}
