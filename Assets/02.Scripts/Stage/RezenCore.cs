using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RezenCore : MonoBehaviour {

    public void Init(int _monsterNumber = 20)
    {
        GameObject monster = GameObject.FindGameObjectWithTag("Enemy");
        PrefabSystem.instance.CreatePrefab(TYPE.MONSTER, monster, _monsterNumber);
    }
}
