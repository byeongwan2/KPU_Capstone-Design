using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//싱글톤클래스   매니저라고 써있는클래스는  게임내에 1개만존재
public class EffectManager : MonoBehaviour {
    public static EffectManager Instance { get { return instance; } }
    private static EffectManager instance = null;
    void Awake()
    {
        if (instance == null)               //매니저가 없다면 생성
        {
            instance = this;                //다른스크립트에서 매니저로접근한다는뜻은 = 인스턴스변수로 접근한다는뜻
        }
        else DestroyImmediate(this);        //매니저가이미있다면 파괴
    }

    //폭탄 터지는 효과 오브젝트를 미리 만들어놓고 꺼놓음
    void Start ()
    {
        GameObject expEffect = Resources.Load("Prefabs/BombEffect") as GameObject;
       
        PrefabSystem.instance.Create_Prefab(TYPE.BOMBEFFECT, expEffect, 10);      //오브젝트풀
    }

    //폭탄효과가 _delayTime후에 실행됨              (여기서말하는 효과는 폭발이펙트)
    public void Exercise_Effect(Vector3 _position,float _delayTime)
    {
        StartCoroutine(Look_Effect(_position,_delayTime));
    }

    
    //폭탄효과 실제실행하는코드
    IEnumerator Look_Effect(Vector3 _position , float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        var expEffect = PrefabSystem.instance.Active_Prefab(TYPE.BOMBEFFECT);
        expEffect.transform.rotation = Quaternion.identity;
        expEffect.transform.position = _position;
        //expEffect.SetActive(true);
        yield return new WaitForSeconds(Define.ADVANCE_BULLET_LIFE_TIME);
        expEffect.SetActive(false);
        //Destroy(expEffect);
    }
}
