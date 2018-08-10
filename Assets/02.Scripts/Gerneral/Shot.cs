using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Behaviour
{

    private GameObject bullet;
    float bulletSpeed;

    [SerializeField]
    private GameObject startPosition;
 

    public void Init(string _link, int _maxCount, float _bulletSpeed = 1000.0f)
    {
        base.Init();
        bulletSpeed = _bulletSpeed;

        bullet = Resources.Load("Prefabs/" + _link) as GameObject;
        prefabSystem.CreatePrefab(TYPE.BULLET,bullet, _maxCount);
    }

    public override void Work()
    {
        var bullet = prefabSystem.ActivePrefab(TYPE.BULLET).GetComponent<Bullet>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        
        bullet.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bullet.SetLaunchRot(this_transForm.localRotation);
        bullet.SetActiveSetting();
    }
}
