using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//플레이어가 던지는 폭탄 스크립트
public class Bomb : AttackObject {
    [SerializeField]
    float power = 0.0f;

    public void SetPower(float _power){ power = _power; }

    Rigidbody this_rigidbody;
    RangeEffect rangeEffect;
    Collider collider;
    void Awake()
    {
        this_rigidbody = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐
        rangeEffect = GetComponentInChildren<RangeEffect>();
        rangeEffect.Init();
        collider = GetComponent<BoxCollider>();
       //list = PrefabSystem.instance.Get_MonsterList();
    }
    //폭탄의 수명
    void LifeOff()
    {
        ExplosionEffect();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
        rangeEffect.RangeLookExit();
    }
	
        
	void FixedUpdate () {                           //폭탄 그자체가 날라가는 코드를 기입하면댐
        //if(gameObject.activeSelf == false) return;          
        // this_rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
    }

    //폭탄이 던져질떄 날라가는 코드
    public void SetActiveLaunch()
    { 
        transform.position = launchPos;
        transform.rotation = launchRot;
        this_rigidbody.AddForce(transform.up  + transform.forward* 7.0f, ForceMode.VelocityChange);       //포물선수정필요
        rangeEffect.RangeLook(3.5f);
        
        Invoke("LifeOff", 5.0f);        //2초뒤 폭탄삭제
    }

 

    //폭발이펙트를 킴
    void ExplosionEffect()
    {
        EffectManager.Instance.Exercise_Effect(transform.position, 0.0f);
        AttackExplosion();
        gameObject.SetActive(false);
    }

    List<GameObject> list;
    void AttackExplosion()          //범위내 적 데미지가함
    {
        foreach( var enemy in PrefabSystem.instance.allMonster)
        {
            if (enemy.activeSelf == false) continue;
            if ( 2.8f > Check.Distance(enemy.transform, this.transform))
            {
                Debug.Log(Check.Distance(enemy.transform, this.transform));
                enemy.SendMessage("WoundExplosionDamage", SendMessageOptions.DontRequireReceiver);
            }

        }
    }
}
