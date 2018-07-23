using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PrefabSystem : MonoBehaviour {
    enum WHO { PLAYER }
    private List<GameObject> bombPool = new List<GameObject>();         //폭탄을 미리생성
    public void CreatePrefab(GameObject _gameObject , int _count)       //여러가지 폭탄을 생성할수 잇게끔
    {
        for (int i = 0; i < _count; i++)
        {
           var obj =   Instantiate<GameObject>(_gameObject, this.transform);
            obj.SetActive(false);
            bombPool.Add(obj);
        }
    }
   
    public GameObject ActivePrefab()
    {
        foreach(var obj in bombPool)
        {
            if(obj.activeSelf == false)
            {
                obj.SetActive(true);      
                return obj;               //꼭 반환아니여도 다른방법이 있을지 생각해볼필요
            }
        }
        return null;
    }
}
