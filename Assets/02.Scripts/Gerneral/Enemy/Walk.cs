using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : Behaviour
{
    //걷기
    bool isWalk = false;
    //목적지
    Vector3 target;
    Vector3 dir;        // 내가현재 바라보는 방향
    float speed = 1.0f;
    // Start is called before the first frame update
    float timeSpan = 0.0f;

    void Start()
    {
        isWalk = false;
        target = Vector3.zero;
        timeSpan = 0.0f;
    }
    public void Init()          
    {
        
    }


    //보스는 꼭 플레이어를 향해 움직이진 않으므로 vector가 들어옴 //걷는중 1초마다 한번씩 호출
    public void Init_Target(Vector3 _target)
    {
        target = _target;
        dir = target - transform.position;
        //transform.forward = dir.normalized;
        //target = target - transform.position;

        //target.Normalize();
        //Quaternion q = Quaternion.LookRotation(target);
    }

    //걷고있을땐 계속호출
    public void Work()
    {
        isWalk = true;
    }

    public override void End()
    {
        isWalk = false;
        timeSpan = 0.0f;
    }

    private void FixedUpdate()
    {
        if (isWalk == false ) return;

        float rotateDegree = Mathf.Atan2(target.x - transform.position.x, target.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime );
        transform.position += transform.forward * speed * Time.deltaTime;
        Debug.Log("걷기");
        

    }

    //타겟과의 거리를 재고 추적할지 말지 결정        //update호출이지만 성능향상을 위해 실제론 1초에1번호출
    public bool Condition(Vector3 _vec,float _dis = 6.0f)
    {
        timeSpan += Time.deltaTime;  // 경과 시간을 계속 등록
        if (timeSpan > 1.0f)  // 경과 시간이 특정 시간이 보다 커졋을 경우
        {
            Init_Target(_vec);
            if (_dis > Check.Distance(target, this.transform.position))
            {
                Debug.Log("가까이");
                isWalk = true;
                timeSpan = 0;
                return true;
            }
            else
            {
                Debug.Log("멀리");
                isWalk = false;
                timeSpan = 0;
                return false;
            }
        }
        if (isWalk == false)
        {
            return false;
        }
        else
        {

            return true;

        }
    }
    
}
