using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Behaviour
{

    public void Init(string _link, int _maxCount, float _bulletSpeed, int _damage )//총알이름//총알오브젝트풀수//총알스피드//총알데미지
    {
        base.Init();
        GameObject bullet = Resources.Load("Prefabs/" + _link) as GameObject;
        bullet.GetComponent<Bullet>().StatSetting();
        bullet.GetComponent<Bullet>().DamageSetting(_damage);
        PrefabSystem.instance.CreatePrefab(TYPE.BULLET,bullet, _maxCount);


    }

    public override void Work()
    {
        var bullet = PrefabSystem.instance.ActivePrefab(TYPE.BULLET).GetComponent<Bullet>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        
        bullet.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bullet.SetLaunchRot(this_transForm.localRotation);
        bullet.SetActiveLaunch();
    }

    
}
