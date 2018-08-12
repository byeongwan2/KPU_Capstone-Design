using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MoveObject
{
    void Start()
    {
        base.Setting();
        hp.SettingHp(100);
    }

    void OnTriggerEnter(Collider _obj)
    {
        if(_obj.tag == "Bullet")
        {
            _obj.gameObject.SetActive(false);
            Debug.Log("총알이 적과부딪힘");
            Debug.Log(hp.getHp());
        }
    }
}
