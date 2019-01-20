using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour
{    
    List<GameObject> activeBullet = new List<GameObject>();
    GameObject bullet;
    public bool IsBulletComeToMe()  // 총알이 나에게 오는가?
    {
        activeBullet = PrefabSystem.instance.Get_BulletPool();
        
        float distance;

        foreach( var obj in activeBullet)
        {
            distance = Mathf.Pow(transform.position.x - obj.transform.position.x, 2) + Mathf.Pow(transform.position.z - obj.transform.position.z, 2);
            if (distance <3.0f)
            {
                bullet = obj;
                Debug.Log("Bullet Comming!");
                return true;
            }            
        }
        return false;
    }

    public bool IsNotRollingCoolTime()  // 구르기가 쿨타임이 아닌가?
    {        
        return true;    
    }

    public bool Rolling()   // 구르기 액션
    {
        float rotateDegree = Mathf.Atan2(bullet.transform.position.x - transform.position.x, bullet.transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree + 30.0f, 0.0f), Time.deltaTime * 10.0f);
        return true;
    }

    
}
