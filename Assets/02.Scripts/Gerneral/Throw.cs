using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : Behaviour
{
    [SerializeField]
    protected GameObject startPosition;
    private GameObject bomb;

    float bombPower;
   
    //게임시작시 단한번만 호출하는 초기화
    public void Init(string _link, int _maxCount, float _bombPower = 2.0f)
    {
        bombPower = _bombPower;
        bomb = Resources.Load("Prefabs/" + _link) as GameObject;
        PrefabSystem.instance.Create_Prefab(TYPE.BOMB,bomb, _maxCount);
        
    }

    // 실제로 폭탄을 던짐 폭탄 던질때마다 호출
    public override void Work(TYPE _type)
    {
        var bomb = PrefabSystem.instance.Active_Prefab(TYPE.BOMB).GetComponent<Bomb>();          
        bomb.SetPower(bombPower);
        bomb.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bomb.SetLaunchRot(transform.localRotation);
        bomb.SetActiveLaunch();
       // bomb.SetVelocity          //폭탄을 던질때마다 던지는놈의 속성을 대입만해주면댐
    }

    //폭탄의 데미지를 중간에 바꿀 필요가 있다면
    public void Set_BombPower(float _bombPower)
    {
        bombPower = _bombPower;
    }

   
}
