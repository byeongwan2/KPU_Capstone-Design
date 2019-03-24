using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentChapter : MonoBehaviour
{
    protected eCHAPTER mychap;
    public void Set_Init(eCHAPTER _init)
    {
        mychap = _init;
    }
}

public class Chapter : ParentChapter
{

    void OnCollisionEnter(Collision _player)
    {
        if (StageManager.instance.Get_NowChapter() != mychap) return;
        if(_player.collider.CompareTag("Player"))
        {
            StageManager.instance.Renew();
        }
    }
}
