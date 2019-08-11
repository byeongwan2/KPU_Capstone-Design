using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    //걷기시작할지말지
    bool isStart = false;
    //걷기
    bool isWalk = false;
    //목적지
    Vector3 target;
    float speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        isWalk = false;
        target = Vector3.zero;
        isStart = false;
    }
    public void Init()          
    {
        
    }
    //보스는 꼭 플레이어를 향해 움직이진 않으므로 vector가 들어옴 //걷는중 1초마다 한번씩 호출
    public void Init_Target(Vector3 _target)
    {
        target = _target;
    }

    //걷고있을땐 계속호출
    public void Work()
    {
        isWalk = true;
    }

    public void End()
    {
        isWalk = false;
        isStart = false;
    }

    private void FixedUpdate()
    {
        if(isWalk)
            transform.localPosition += transform.forward * speed * Time.deltaTime;
    }

    //타겟과의 거리를 재고 추적할지 말지 결정        //update호출이지만 성능향상을 위해 실제론 1초에1번호출
    public bool Condition(float _dis = 6.0f)
    {
        if (isStart == true) return false;
        StartCoroutine("DistanceCompare",_dis);
        isStart = true;
        return false;
    }

    IEnumerator DistanceCompare(float _dis)
    {
        while (true)
        {
            if (_dis > Check.Distance(target, this.transform.position))
            {
                StopCoroutine("DistanceCompare");
                isWalk = true;
                yield return null;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
    
}
