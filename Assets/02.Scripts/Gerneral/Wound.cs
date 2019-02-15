using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wound : MonoBehaviour {            //상처를 입는 사물이라면 이 클래스를 Add Component
    SkinnedMeshRenderer[] this_renderer;
    MoveObject this_gameObject;
    bool woundEffect = false;
    void Start()
    {
        this_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        this_gameObject = GetComponent<MoveObject>();
    }
	public void TriggerEnter(Collider _obj)
    {
        var id = this_gameObject.Get_Id();
        if (this_gameObject.Compare_This(id)) return;
        ////  int damage = _obj.gameObject.GetComponent<Bullet>().Damage;
       // this_gameObject.MinusHp(10);         //임시
        Wound_Effect();
        _obj.gameObject.SetActive(false);
    }

    public void Wound_Effect()
    {
        woundEffect = true;
        StartCoroutine(Change_Effect_Color());
        Invoke("WoundEffectExit", 1.5f);
    }

    IEnumerator Change_Effect_Color()
    {
        while (true)
        {
            foreach( var part in this_renderer)
                part.material.color = Color.red;
           
            yield return new WaitForSeconds(0.1f);

            foreach (var part in this_renderer)
                part.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            if (woundEffect == false) yield break;
        }
    }

    void WoundEffectExit()
    {
        woundEffect = false;
    }
    public void ExplosionDamage(int _damage)
    {
      //  this_gameObject.MinusHp(_damage);
    }
}
