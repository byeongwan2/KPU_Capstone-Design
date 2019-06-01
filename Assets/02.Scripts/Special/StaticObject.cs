using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    public GameObject sparkEffect;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            if(other.collider.name.Equals("PlayerBasicBullet(Clone)"))
            {
                other.gameObject.SetActive(false);
                ShowEffect(other);
            }
        }
    }

    void ShowEffect(Collision coll)
    {
        ContactPoint contact = coll.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        Instantiate(sparkEffect, contact.point, rot);
        //총알 이펙트 넣으면댐
    }
}
