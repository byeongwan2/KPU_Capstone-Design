using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_Event : ParentChapter
{
    void OnCollisionStay(Collision _player)
    {
        if (_player.collider.CompareTag("Player"))
        {
            if(StageManager2.instance.Is_AllMonsterDie())
            {
                StageManager2.instance.Start_Stage2();
            }

        }
    }
}