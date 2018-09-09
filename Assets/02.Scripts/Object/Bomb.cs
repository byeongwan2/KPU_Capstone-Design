using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : AttackObject {
    [SerializeField]
    float power = 0.0f;
    GameObject expEffect;

    public void SetPower(float _power){ power = _power; }
    private Vector3 velocity;
    public void SetVelocity(Vector3 _velocity)  { velocity = _velocity; }

    Rigidbody this_rigidbody;
    RangeEffect rangeEffect;

    void Awake()
    {
        this_rigidbody = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐
        expEffect = Resources.Load("Prefabs/BombEffect") as GameObject;
        expEffect = Instantiate(expEffect);
        expEffect.SetActive(false);
        rangeEffect = GetComponentInChildren<RangeEffect>();
        rangeEffect.Init();

       list = PrefabSystem.instance.ActiveMonsterList();
    }

    void LifeOff()
    {
        StartCoroutine(ExplosionEffect());
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
        rangeEffect.RangeLookExit();
    }
	
        
	void FixedUpdate () {                           //폭탄 그자체가 날라가는 코드를 기입하면댐
        if(gameObject.activeSelf == false) return;          
        // this_rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
    }

    public void SetActiveLaunch()          //폭탄이 켜지면서 초기화
    {
        transform.position = launchPos;
        transform.rotation = launchRot;
        this_rigidbody.AddForce(transform.up * 12.0f /*+ transform.forward* 10.0f*/, ForceMode.VelocityChange);       //포물선수정필요
        rangeEffect.RangeLook(3.0f);

        Invoke("LifeOff", 5.0f);        //2초뒤 폭탄삭제
    }

    public override void StatSetting()
    {

    }

    IEnumerator ExplosionEffect()
    {
        expEffect.SetActive(true);
        ExplosionAttack();
        expEffect.transform.rotation = Quaternion.identity;
        expEffect.transform.position = transform.position;
        yield return new WaitForSeconds(0.8f);
        expEffect.SetActive(false);
        gameObject.SetActive(false);
    }
    List<GameObject> list;
    void ExplosionAttack()
    {
        foreach( var enemy in list)
        {
            if (enemy.activeSelf == false) continue;
            if( 2.4f > Check.Distance(enemy.transform, this.transform))
            {
                Debug.Log(Check.Distance(enemy.transform, this.transform));
                enemy.GetComponent<Wound>().ExplosionDamage(10);
            }
        }
    }
}
