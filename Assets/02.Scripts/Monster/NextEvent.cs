using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextEvent : MonoBehaviour
{
    public eCHAPTER chapter ;
    public void Create_Monster()
    {
        StageManager2.instance.NextChapter(chapter);
    }
}
