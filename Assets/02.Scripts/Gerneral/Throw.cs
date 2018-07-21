using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    private PrefabSystem prefabSystem;

    private Transform tr;
    void Start()
    {
        tr = GetComponent<Transform>();
        prefabSystem = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
    }
    public void Work(float _power)
    {
        prefabSystem.ActiveBomb();
    }
}
