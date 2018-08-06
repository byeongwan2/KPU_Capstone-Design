using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Behaviour {

    private GameObject bullet;
    float bulletSpeed;
    void Start()
    {
        base.Init();
    }

    public void Init(string _link, int _maxCount, float _bulletSpeed = 1000.0f)
    {
        bulletSpeed = _bulletSpeed;
        bullet = Resources.Load("Prefabs/" + _link) as GameObject;
        system.CreatePrefab(TYPE.BULLET,bullet, _maxCount);
    }

    public override void Work()
    {
        var bullet = system.ActivePrefab(TYPE.BULLET).GetComponent<Bullet>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        bullet.SetLaunchPos(this_transForm.position);     //출발하는장소
        bullet.SetLaunchRot(this_transForm.localRotation);


    }
}
