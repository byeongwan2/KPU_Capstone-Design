using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Behaviour
{
    [SerializeField]
    protected GameObject startPosition;
    //총알이름//총알오브젝트풀수//총알스피드//총알데미지
    public void Init(string _link, int _maxCount, float _bulletSpeed, int _damage,TYPE _type )
    {
        GameObject bullet = Resources.Load("Prefabs/" + _link) as GameObject;      
        bullet.GetComponent<Bullet>().StatSetting();
        bullet.GetComponent<Bullet>().DamageSetting(_damage);
        bullet.GetComponent<Bullet>().SpeedSetting(_bulletSpeed);
        PrefabSystem.instance.Create_Prefab(_type, bullet, _maxCount);      //오브젝트풀           
    }

    public override void Work(TYPE _type)
    {
        var bullet = PrefabSystem.instance.Active_Prefab(_type).GetComponent<Bullet>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        bullet.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bullet.SetLaunchRot(transform.localRotation);
        bullet.SetActiveLaunch(_type);

    }
    
}
