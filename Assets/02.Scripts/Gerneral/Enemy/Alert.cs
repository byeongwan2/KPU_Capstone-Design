using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//경계하다
public class Alert : Behaviour
{
    bool isActive = false;
    float timeSpan = 0.0f;
    public bool IsShotDelay  { get; set; }

    float timeRotate = 0.0f;      //쳐다보는시간을 주기위함 0.5초
    void Start()
    {
        IsShotDelay = false;
        timeSpan = 0.0f;
        timeRotate = 0.0f;
    }

    //상태가 바뀔때마다 호출되는함수
    public void Init()
    {

        timeRotate = 0.0f;

    }
    void Rotate_Alert()
    {
     
        float t = Time.realtimeSinceStartup * 5.0f;

        Vector3 vec = Vector3.zero;
        vec.y = transform.position.y;
        vec.x = transform.position.x + 1.0f * Mathf.Cos(t);
        vec.z = transform.position.z + 1.0f * Mathf.Sin(t);

        transform.position = vec;
        Debug.Log("옆으로걷기");

    }
    void FixedUpdate()
    {
        if (isActive == false) return;
        timeRotate += Time.deltaTime;
        if (timeRotate < 0.5f)
        {
            Vector3 target = PrefabSystem.instance.player.transform.position;
            float rotateDegree = Mathf.Atan2(target.x - transform.position.x, target.z - transform.position.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 2.0f);
        }
        else
        {
            /*
            if (timeRotate < 2.0f)
            {
                InvokeRepeating("Rotate_Alert", 0.0f, 0.16f);
                timeRotate = 2.0f;
            }*/
            float t = Time.realtimeSinceStartup * 12.0f;

            Vector3 vec = Vector3.zero;
            vec.y = transform.position.y;
            vec.x = transform.position.x + 1.0f * Mathf.Cos(t);
            vec.z = transform.position.z + 1.0f * Mathf.Sin(t);

            //vec = vec - transform.position;
            vec.Normalize();
            transform.position += vec * 1.0f * Time.deltaTime;
            //transform.position = vec;
            Debug.Log("옆으로걷기");
        }
            
    }

    public void Work()
    {
        isActive = true;
        timeSpan += Time.deltaTime;  // 경과 시간을 계속 등록
        if (timeSpan > 2.5f)  // 경과 시간이 특정 시간이 보다 커졋을 경우
        {
            timeSpan = 0.0f;
            IsShotDelay = true;

        }
    }
  
    //타겟과의 거리를 재고 추적할지 말지 결정
    public bool Condition(float _dis, Transform tr)
    {
        if (_dis > Check.Distance(tr.transform.position, this.transform.position))
        {
            return true;

        }
        return false;
    }

    public override void End()
    {
        CancelInvoke();
        IsShotDelay = false;
        isActive = false;
        timeRotate = 0.0f;
        //timeSpan = 0.0f;   //이 주석을 풀면 공격전엔 항상 2.5초 경계를하게된다 어떤기획이 좋을까
    }
}
