using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Behaviour
{
    [SerializeField]
    protected GameObject startPosition;
    private ParticleSystem muzzleFlash;
   // List<Bullet> list = new List<Bullet>(); 
    //총알이름//총알오브젝트풀수//총알스피드//총알데미지//프리팹시스템기준 타입 // 총알을 발사하는 주체
    public void Init(string _link, int _maxCount, float _bulletSpeed, int _damage,TYPE _type ,Object_Id _id)
    {
        GameObject bullet = Resources.Load("Prefabs/" + _link) as GameObject;      
        bullet.GetComponent<Bullet>().DamageSetting(_damage);
        bullet.GetComponent<Bullet>().SpeedSetting(_bulletSpeed);
        bullet.GetComponent<Bullet>().Resister_ID(_id);
        PrefabSystem.instance.Create_Prefab(_type, bullet, _maxCount);      //오브젝트풀

        muzzleFlash = startPosition.GetComponentInChildren<ParticleSystem>();
    }

    //총알 발사
    public override void Work(TYPE _type)
    {
        var bullet = PrefabSystem.instance.Active_Prefab(_type).GetComponent<Bullet>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
        bullet.SetLaunchPos(startPosition.transform.position);     //출발하는장소
        bullet.SetLaunchRot(transform.localRotation);
        bullet.SetActiveLaunch(_type);
        muzzleFlash.Play();
        /* foreach (var obj in list)
         {
             if (obj.gameObject.activeSelf == false)
             {
                 obj.gameObject.SetActive(true);
                 obj.SetLaunchPos(startPosition.transform.position);     //출발하는장소
                 obj.SetLaunchRot(transform.localRotation);
                 obj.SetActiveLaunch(_type);
                 return;
             }
         }*/


    }
    
    
}
