using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Attack : MonoBehaviour
{
    private Transform playerTr;
    private Transform enemyTr;


    // 공격 시간 계산 변수
    private float nextAttack = 0.0f;
    // 총알 발사 간격
    private readonly float fireRate = 0.1f;
    // 주인공을 향해 회전 속도 계수
    private readonly float damping = 10.0f;

    // 총알 발사 판단 여부
    public bool isFire = false;

    // 드론 총알 프리팹
    public GameObject bullet;
    // 총알 발사 위치
    public Transform firePos;
        
    void Start()
    {
        playerTr = GameObject.FindWithTag("PLAYER").transform;
        enemyTr = GetComponent<Transform>();
    }
      
    
    public void DroneRotate()
    {
        // 회전각 계산
        Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
        // 보간 함수를 이용해 회전
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
    }
    public bool Attack()
    {
        // 현재 시간이 다음 공격 시간보다 큰가?
        if (Time.time >= nextAttack)
        {
            // 발사
            GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
            // 3초 후 미사일 삭제
            Destroy(_bullet, 3.0f);
            // 다음 공격 시간
            nextAttack = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            return true;
        }
        else
            return false;
    }
    
}
