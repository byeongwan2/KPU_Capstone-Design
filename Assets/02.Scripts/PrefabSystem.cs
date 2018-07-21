using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PrefabSystem : MonoBehaviour {
    enum WHO { PLAYER }
    private List<GameObject> bombPool = new List<GameObject>();
    public void CreatePrefab(GameObject _gameObject , int _count)
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
        foreach(var o in bombPool)
        {
            if(o.activeSelf == false)
            {
                o.SetActive(true);
                return o;
            }
        }
        return null;
    }
}
