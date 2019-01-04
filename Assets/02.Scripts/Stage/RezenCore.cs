using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RezenCore : MonoBehaviour {

    public void Init(int _monsterNumber = 20)
    {
        GameObject monster = GameObject.FindGameObjectWithTag("Enemy");
        PrefabSystem.instance.Create_Prefab(TYPE.MONSTER, monster, _monsterNumber);
    }

    public void Work()
    {
        var monster = PrefabSystem.instance.Active_Prefab(TYPE.MONSTER).GetComponent<Monster>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        
        
    }
}
