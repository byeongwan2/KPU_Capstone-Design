using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    private GameObject bomb;
 

    private Transform this_transForm;

    private PrefabSystem system;

    void Start()            
    {
        this_transForm = GetComponent<Transform>();
    }

    public void Init(string _link, int _maxCount)
    {
        system = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
        bomb = Resources.Load("Prefabs/" + _link) as GameObject;
        system.CreatePrefab(bomb, _maxCount);
    }
    
    public void Work(float _bombPower)
    {
        var bomb = system.ActivePrefab().GetComponent<Bomb>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        bomb.SetPower(_bombPower);
        bomb.SetLaunchPos(this_transForm.position);
       // bomb.SetVelocity          //폭탄을 던질때마다 던지는놈의 속성을 대입만해주면댐
    }
}
