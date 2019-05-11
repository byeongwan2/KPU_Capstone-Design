using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start ()
    {
        GameObject expEffect = Resources.Load("Prefabs/BombEffect") as GameObject;
       
        PrefabSystem.instance.Create_Prefab(TYPE.BOMBEFFECT, expEffect, 10);      //오브젝트풀
    }
	
	public void Exercise_Effect(Vector3 _position,float _delayTime)
    {
      
        StartCoroutine(Look_Effect(_position,_delayTime));
    }

    

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
