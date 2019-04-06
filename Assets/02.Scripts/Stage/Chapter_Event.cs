using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_Event : ParentChapter
{
    public bool isReady = false;
    void OnCollisionEnter(Collision _player)
    {
        if (StageManager.instance.Get_NowChapter() != mychap) return;
        if (_player.collider.CompareTag("Player"))
        {
            isReady = true;

        }
    }
}