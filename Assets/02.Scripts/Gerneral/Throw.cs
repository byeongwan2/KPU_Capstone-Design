using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : Behaviour {

    private GameObject bomb;

    float bombPower;
   

    public void Init(string _link, int _maxCount, float _bombPower = 15.0f)
    {
        base.Init();
        bombPower = _bombPower;
        bomb = Resources.Load("Prefabs/" + _link) as GameObject;
        PrefabSystem.instance.CreatePrefab(TYPE.BOMB,bomb, _maxCount);
    }
    
    public override void Work(TYPE _type)
    {
        var bomb = PrefabSystem.instance.ActivePrefab(TYPE.BOMB).GetComponent<Bomb>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        bomb.SetPower(bombPower);
        bomb.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bomb.SetLaunchRot(this_transForm.localRotation);
        bomb.SetActiveLaunch();
       // bomb.SetVelocity          //폭탄을 던질때마다 던지는놈의 속성을 대입만해주면댐
    }


    public void SetBombPower(float _bombPower)
    {
        bombPower = _bombPower;
    }

   
}
