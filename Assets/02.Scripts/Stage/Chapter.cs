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
        if (StageManager.instance.Get_NowChapter() != mychap) return;       //현재 챕터와 다음으로넘어갈 챕터가 같지 않아야 유효
        if(_player.collider.CompareTag("Player"))
        {
            StageManager.instance.Set_IsChap();
            StageManager.instance.Renew();
        }
    }

}
