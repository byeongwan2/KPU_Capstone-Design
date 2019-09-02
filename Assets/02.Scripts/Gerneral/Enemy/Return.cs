using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : Behaviour
{
    public Vector3 startPosition = Vector3.zero;
    float timeSpan = 0.0f;
    bool isReturn = false;
    public void Init(Vector3 _vec)
    {
        startPosition = _vec;
        isReturn = false;
    }
    public override void End()
    {
        isReturn = false;
        timeSpan = 0.0f;
    }
    private void FixedUpdate()
    {
        if (isReturn == false) return;

        float rotateDegree = Mathf.Atan2(startPosition.x - transform.position.x, startPosition.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime);
        transform.position += transform.forward * 1.0f * Time.deltaTime;
        Debug.Log("제자리로");

    }

    public bool Condition(float _dis)
    {
        timeSpan += Time.deltaTime;
        if (timeSpan > 1.0f)  // 경과 시간이 특정 시간이 보다 커졋을 경우
        {
            timeSpan = 0.0f;
            if(isReturn)
            {
                if (transform.position.x < startPosition.x + 1.0f && transform.position.x >= startPosition.x - 1.0f
                && transform.position.z < startPosition.z + 1.0f && transform.position.z >= startPosition.z - 1.0f)
                {
                    timeSpan = 0.0f;
                    isReturn = false;
                }
                else isReturn = true;
                return isReturn;
            }
            else if (_dis < Check.Distance(startPosition, this.transform.position))
            {
                isReturn = true;
            }
            else
            {
                isReturn = false;
            }
            return isReturn;
        }
        return isReturn;
    }
}
