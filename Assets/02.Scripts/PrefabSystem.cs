using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TYPE { BOMB, BULLET ,MONSTER }
public class PrefabSystem : MonoBehaviour {                //프리팹시스템에서 모든 오브젝트풀을 관리하니까 나중에 분리할필요가있음
    public static PrefabSystem instance = null;             //제네릭클래스로 바꿔야함
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this.gameObject); Debug.Log("매니저중복추적"); }
        //DontDestroyOnLoad(this.gameObject);
    }
    enum WHO { PLAYER }
    private List<GameObject> bombPool = new List<GameObject>();         //폭탄을 미리생성
    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> monsterPool = new List<GameObject>();
    public void CreatePrefab(TYPE _type,GameObject _gameObject , int _count)       //여러가지 폭탄을 생성할수 잇게끔
    {
        if (_type == TYPE.BOMB)SelectPoolType(bombPool, _gameObject, _count);    
        else if (_type == TYPE.BULLET) SelectPoolType(bulletPool, _gameObject, _count);
        else if (_type == TYPE.MONSTER) SelectPoolType(monsterPool, _gameObject, _count);
    }

    public void SpecialPrefab ()
    {

    }

   
    public GameObject ActivePrefab(TYPE _type)
    {
        if (_type == TYPE.BOMB) return ChoicePool(bombPool);
        else if (_type == TYPE.BULLET) return ChoicePool(bulletPool);
        else if (_type == TYPE.MONSTER) return ChoicePool(monsterPool);
        return null;
    }

    private GameObject ChoicePool<T>(T _pool) where T : List<GameObject>            //수많은풀을 if문으로 하지않고 일반화
    {
        foreach( var obj in _pool)
        {
            if (obj.activeSelf == false)
            {
                obj.SetActive(true);
                return obj;               //꼭 반환아니여도 다른방법이 있을지 생각해볼필요
            }
        }
        return null;
    }


    private void SelectPoolType<T>(T _pool,GameObject _gameObject, int _count ) where T : List<GameObject>
    {
        for (int i = 0; i < _count; i++)
        {
            var obj = Instantiate<GameObject>(_gameObject, this.transform);
            obj.SetActive(false);

            _pool.Add(obj);
            
        }
    }

    public List<GameObject> ActiveMonsterList()
    {
        return monsterPool;
    }
}
