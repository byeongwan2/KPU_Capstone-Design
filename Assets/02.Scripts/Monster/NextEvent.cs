using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//챕터가 넘어가는지 체크하는 스크립트 
public class NextEvent : MonoBehaviour
{
    public eCHAPTER chapter ;

    public void Create_Monster()
    {
        StageManager2.instance.NextChapter(chapter);
    }
}
