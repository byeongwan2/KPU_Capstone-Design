using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MyMLAgent : Agent
{
    private Transform pivot;                 // 훈련 레벨의 중심 위치
    private Transform[] objectTransform;     // 리셋하기 위한 타겟 위치 변수    
    public Transform[] firePos;               // 발사 위치
    public GameObject explosionVFX;          // 총알 맞혔을 때 효과
    public AudioClip explosionSFX;
    RayPerception rayPerception;
        
    private int[] positionNum = { 0, 1, 2, 3};
    private int targetCount = 1;
    private Collider[] colliders;
    private Animator supporterAnimator;
    private AudioSource audioSource;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rayPerception = GetComponent<RayPerception>();
        supporterAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //ResetTarget();
    }
    
    // 한 에피소드가 끝나고 리셋할 때 자동 호출 됨.
    public override void AgentReset()
    {
        targetCount = 1;
        //ResetTarget();
    }

    // 관측 함수
    public override void CollectObservations()
    {        
        float rayDistance = 15f;
        float[] rayAngles = { 30f, 60f, 90f,  120f, 150f};
        string[] detectableObjects = { "EnemyBullet", "Untagged" };  //  20(angle count) * 2(tag count) * 2
        
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 1.5f, 0f));
    }

    // 행동 함수
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.0001f);
        MoveAgent(vectorAction);
    }

    // 타겟 위치 재설정
    void ResetTarget()
    {
        
        for (int i = 0; i < 2; i++)
        {
            objectTransform[i].gameObject.SetActive(true);            
        }
        

        objectTransform[0].gameObject.SetActive(true);

        int sign = Random.Range(0, 2);

        Vector3 randomPos;

        if (sign == 0)
        {
            randomPos = new Vector3(Random.Range(2f, 5f), 1.5f, Random.Range(2f, 5f));
        }
        else
        {
            randomPos = new Vector3(Random.Range(-5f, -2f), 1.5f, Random.Range(-5f, -2f));
        }

        objectTransform[0].position = randomPos + pivot.position;
        objectTransform[1].position = new Vector3(-randomPos.x, 1.5f, -randomPos.z) + pivot.position;
    }
    
    // 공격 함수
    void AttackAgent()
    {
        RaycastHit hit;

        for(int i = 0; i < 5; i++)
        {
            Debug.DrawRay(firePos[i].position, firePos[i].transform.forward * 15f, Color.red, 0.1f);
        }
        supporterAnimator.SetBool("Shoot", true);

        // 감지된 물체가 있다면
        for(int i = 0; i<5; i++)
        {
            if (Physics.Raycast(firePos[i].position, firePos[i].transform.forward, out hit, 15f))
            {
                Debug.Log(hit.collider.tag);
                // 그게 EnemyBullet 이라면
                if (hit.collider.tag == "EnemyBullet")
                {
                    Debug.Log("적총알");
                    AddReward(0.3f);
                    hit.transform.gameObject.SetActive(false);

                    GameObject VFX = Instantiate(explosionVFX, hit.transform.position, hit.transform.rotation);
                    audioSource.PlayOneShot(explosionSFX);
                    Destroy(VFX, 1.0f);

                    targetCount--;

                    if (targetCount == 0)
                    {
                        Done();
                    }
                }

                // 아니라면
                else
                {
                    AddReward(-1.0f);
                }
            }
            // 감지된 물체가 없다면
            else
            {
                AddReward(-1.0f);
            }
        }
       

        supporterAnimator.SetBool("Shoot", false);
    }    
    
    void RandomIndex()
    {
        for (int i = 0; i < 4; ++i)
        {
            int ranIdx = Random.Range(i, 4);

            int tmp = positionNum[ranIdx];

            positionNum[ranIdx] = positionNum[i];

            positionNum[i] = tmp;

        }
    }

    public void MoveAgent(float[] act)
    {        
            int action = Mathf.FloorToInt(act[0]);
            switch (action)
            {                
                case 1:
                    AttackAgent();
                    break;                                
            }        
    }
}
