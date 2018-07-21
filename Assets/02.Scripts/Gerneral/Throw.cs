using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    private GameObject bomb;
 

    private Transform this_transForm;

    private PrefabSystem system;

    void Awake()
    {
        this_transForm = GetComponent<Transform>();
        system = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
    }
    public void Init(string _link, int _maxCount)
    {

        bomb = Resources.Load("Prefabs/" + _link) as GameObject;
        system.CreatePrefab(bomb, _maxCount);
    }
    
    public void Work(float _bombPower)
    {
        var bomb = system.ActivePrefab();
        
    }
}
