using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//벽, 장애물들이 가지고 있는 컴포넌트
public class StaticObject : MonoBehaviour
{
    public GameObject sparkEffect;
    //플레이어 총알이 벽에 박히면 총알이 박힌 자국을 만듬
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Bullet") || other.collider.CompareTag("EnemyBullet"))
        {
            if(other.collider.name.Equals("PlayerBasicBullet(Clone)"))
            {
                other.gameObject.SetActive(false);
                ShowEffect_Player(other);
            }
        }
    }

    //실제 총알박힌자국을 생성
    void ShowEffect_Player(Collision coll)
    {
        ContactPoint contact = coll.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        //Instantiate(sparkEffect, contact.point, rot);
        var effect = PrefabSystem.instance.Active_Prefab(TYPE.BULLETMETALEFFECT);
        effect.transform.position = contact.point;
        effect.transform.rotation = rot;

    }
}
