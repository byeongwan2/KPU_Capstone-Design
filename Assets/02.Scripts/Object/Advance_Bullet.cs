using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advance_Bullet : Bullet
{
    float power;
    void Awake()
    {
        base.Init();
    }
    void Start ()
    {
        speed = 20.0f;
        power = 10.0f;
    }

    void FixedUpdate()
    {
        //현재 총알은 수정하는게 의미없음
    }
 

    public override void SetActiveLaunch()
    {
        base.SetActiveLaunch();
        rb.AddForce(transform.forward *power, ForceMode.Impulse);

        //총알이 콩콩콩 날라가는것을 구현하면됨 에임은 부모꺼를 이용하여 
    }


}
