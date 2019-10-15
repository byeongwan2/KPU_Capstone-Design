using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Wound : MonoBehaviour {            //상처를 입는 사물이라면 이 클래스를 Add Component
    SkinnedMeshRenderer[] this_renderer;
    //MoveObject this_gameObject;
    bool woundEffect = false;
    int hp;
    Slider healthSlider;
    public AudioClip woundSound;
    AudioSource _audio;
    void Start()
    {
        this_renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        //this_gameObject = GetComponent<MoveObject>();
        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = 100;
        healthSlider.value = 100;
        _audio = GetComponent<AudioSource>();
    }
    public void Init(int  _hp = 100)
    {
        hp = _hp;
    }
    public void TriggerEnter(Collider _obj)
    {
        //var id = this_gameObject.Get_Id();
        //Debug.Log(id);
        //if (this_gameObject.Compare_This(id)) return;
        //Debug.Log(this_gameObject.Get_Id());
        ////  int damage = _obj.gameObject.GetComponent<Bullet>().Damage;
        // this_gameObject.MinusHp(10);         //임시
        Wound_Effect();
        _obj.gameObject.SetActive(false);
        GetDamage(10);
    }

    public void Wound_Effect()
    {
        woundEffect = true;
        StartCoroutine(Change_Effect_Color());
        Invoke("WoundEffectExit", 1.2f);
    }

    //플레이어나 적군이 데미지를 입었을때 이펙트
    IEnumerator Change_Effect_Color()
    {
        while (true)
        {
            foreach( var part in this_renderer)
                part.material.color = Color.red;
           
            yield return new WaitForSeconds(0.05f);

            foreach (var part in this_renderer)
                part.material.color = Color.white;
            yield return new WaitForSeconds(0.05f);

            if (woundEffect == false) yield break;
        }
    }

    //피격이펙트 종료
    void WoundEffectExit()
    {
        woundEffect = false;
    }
 

    //실제 데미지를 받고 자신의 체력바를 감소시킴
    public void GetDamage(int _damage)
    {
        hp -= _damage;
        healthSlider.value -= _damage;
        Wound_Effect();
        _audio.PlayOneShot(woundSound);
    }


}
