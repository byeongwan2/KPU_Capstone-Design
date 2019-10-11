using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Supporter : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent pathFinder;
    float angle = 0f;
    void Start()
    {
        pathFinder = GetComponent<NavMeshAgent>();        
        StartCoroutine(Rotation());
        StartCoroutine(Follow());
    }


    private void Update()
    {
        
    }


    private IEnumerator Follow()
    {
        while(true)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);   // 거리 측정

            if(distance >= 5.0f)    // 만약 거리가 5유닛 이상이라면
            {
                Vector3 randomPos = Random.insideUnitSphere * 2.5f + player.transform.position; // 플레이어 기준으로 2.5 거리의 랜덤 좌표 생성
                randomPos.y = 0.0f; // 높이 조정
                pathFinder.isStopped = false;
                pathFinder.SetDestination(randomPos);               
            }

            else
            {
                pathFinder.isStopped = true;                
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    private void Follow2()
    {
        angle += Time.deltaTime;
        float radian = 1.5f;
        transform.position = new Vector3(player.transform.position.x + radian * Mathf.Cos(angle * 3f),
            0f,
            player.transform.position.z + radian * Mathf.Sin(angle * 3f));
    }

    private IEnumerator Rotation()
    {   
        while(true)
        {
            Collider[] others = Physics.OverlapSphere(transform.position, 10f);
        
            for (int i = 0; i < others.Length; i++)
            {  
                if(others[i].tag == "EnemyBullet")
                {
                   Vector3 target = new Vector3(others[i].transform.position.x, others[i].transform.position.y, others[i].transform.position.z);
                   transform.LookAt(target);           
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
