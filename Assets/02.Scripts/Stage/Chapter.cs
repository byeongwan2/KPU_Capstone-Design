using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter : MonoBehaviour
{
    public float sight_range = 0;
    public float culling_range = 0;
    bool touch = false;
    void Start()
    {
        if(sight_range == 0)
        {
            sight_range = 3;
        }
        if(culling_range == 0)
        {
            culling_range = 1;
        }
    }
    
    void OnCollisionEnter(Collision _player)
    {
        if (touch) return;
        if(_player.collider.CompareTag("Player"))
        {
            StageManager.instance.Renew(sight_range,culling_range);
        }
        touch = true;
    }
}
