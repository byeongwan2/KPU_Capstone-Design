using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    // 적 캐릭터의 공격 사정 거리의 범위
    public float viewAttackRange = 12.0f;
    // 적 캐릭터의 추적 사정 거리의 범위
    public float viewTraceRange = 15.0f;
    [Range(0, 360)]
    // 적 캐릭터의 시야각
    public float viewAngle = 120.0f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;

    private void Start()
    {
        // 컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").transform;

        // 레이어 마스크 값 계산
        playerLayer = LayerMask.NameToLayer("MainObject");
        //obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << playerLayer;

    }
    private void Update()
    {
        isTracePlayer();
    }

    // 주어진 각도에 의해 원주 위의 점의 좌표값을 계산하는 함수
    public Vector3 CirclePoint(float angle)
    {
        // 로컬 좌표계를 기준으로 설정하기 위해 적 캐릭터의 Y회전값을 더함
        angle += transform.rotation.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad)     // 유니티 각도에서는 원주 위의 3차원 좌표를 sin(theta), 0, cos(theta) 로 표현한다
            , 0
            , Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    // 공격범위안에 플레이어가 있는지 감지
    public bool isAttackPlayer()
    {
        bool isAttack = false;

        // 추적 반경 범위 안에서 주인공 캐릭터를 추출
        Collider[] colls = Physics.OverlapSphere(enemyTr.position
                                                , viewAttackRange
                                                , 1 << playerLayer);    
        
        // 배열의 개수가 1일 때 주인공이 범위 안에 있다고 판단
        if(colls.Length == 1)
        {                
            // 적 캐릭터와 주인공 사이의 방향 벡터를 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;            
            // 적 캐릭터의 시야각에 들어왔는지 판단
            if(Vector3.Angle(enemyTr.forward, dir) < viewAngle / 2)
            {
                isAttack = true;                
            }
        }
        return isAttack;
    }

    // 추적범위안에 플레이어가 있는지 감지
    public bool isTracePlayer()
    {
        bool isTrace = false;

        // 추적 반경 범위 안에서 주인공 캐릭터를 추출
        Collider[] colls = Physics.OverlapSphere(enemyTr.position
                                                , viewTraceRange
                                                , 1 << playerLayer);
        
        // 배열의 개수가 1일 때 주인공이 범위 안에 있다고 판단
        if (colls.Length == 1)
        {
            // 적 캐릭터와 주인공 사이의 방향 벡터를 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;
            // 적 캐릭터의 시야각에 들어왔는지 판단
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle / 2)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    /* 장애물 검사
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;

        // 적 캐릭터와 주인공 사이의 방향 벡터를 계산
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        // 레이캐스트를 투사해서 장애물이 있는지 여부를 판단
        if(Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("MainObject"));
        }
        return isView;
    }
    */
}