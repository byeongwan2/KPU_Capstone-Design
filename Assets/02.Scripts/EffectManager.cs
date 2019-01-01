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
    GameObject m_expEffect;
    void Start ()
    {
        m_expEffect = Resources.Load("Prefabs/BombEffect") as GameObject;

    }
	
	public void Exercise_Effect(Vector3 _position,float _delayTime)
    {
        m_expEffect.transform.rotation = Quaternion.identity;
        m_expEffect.transform.position = _position;
        StartCoroutine(Look_Effect(_delayTime));
    }

    IEnumerator Look_Effect(float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        GameObject expEffect = Instantiate(m_expEffect);
        expEffect.SetActive(true);
        yield return new WaitForSeconds(Define.ADVANCE_BULLET_LIFE_TIME);
        expEffect.SetActive(false);
        Destroy(expEffect);
    }
}
