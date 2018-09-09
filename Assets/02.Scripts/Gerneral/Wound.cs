using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wound : MonoBehaviour {            //상처를 입는 사물이라면 이 클래스를 Add Component
    private MoveObject this_gameObject;
    void Start()
    {
        this_gameObject = GetComponent<MoveObject>();
    }
	void OnTriggerEnter(Collider _obj)
    {
        if (_obj.CompareTag("Bullet"))
        {
            //  int damage = _obj.gameObject.GetComponent<Bullet>().Damage;
            this_gameObject.MinusHp(10);         //임시
            this_gameObject.WoundEffect();
        }
    }

    public void ExplosionDamage(int _damage)
    {
        this_gameObject.MinusHp(_damage);
    }
}
