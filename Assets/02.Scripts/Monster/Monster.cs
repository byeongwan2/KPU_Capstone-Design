using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MoveObject
{
    GameSystem system;

    [SerializeField]
    private float m_dis;
    void Start()
    {
        base.Setting();
        hp.SettingHp(100);
        StartCoroutine(CheckPlayerDistance());
        m_dis = 5.0f;
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

    IEnumerator CheckPlayerDistance()
    {
        float distance = Check.Distance(system.Player.transform, this.transform);
        if(distance < 5.0f)
        {

        }
        yield return new WaitForSeconds(1.0f);
    }
}
