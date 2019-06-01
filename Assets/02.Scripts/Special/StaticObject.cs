using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if(other.name.Equals("PlayerBasicBullet(Clone)"))
            {
                other.gameObject.SetActive(false);
            }
        }
    }
}
