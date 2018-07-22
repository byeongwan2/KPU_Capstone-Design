using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    private GameObject bomb;
 

    private Transform this_transForm;

    private PrefabSystem system;

    void Start()            //Awake 종속 수정필요 Awake이여만 되는코드는 나중에 문제가 생길수있음
    {
       
    }
    public void Init(string _link, int _maxCount)
    {
        this_transForm = GetComponent<Transform>();
        system = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
        bomb = Resources.Load("Prefabs/" + _link) as GameObject;
        system.CreatePrefab(bomb, _maxCount);
    }
    
    public void Work(float _bombPower)
    {
        var bomb = system.ActivePrefab();           //게임오브젝트가 리턴되므로 이부분 수정해야함
        
    }
}
