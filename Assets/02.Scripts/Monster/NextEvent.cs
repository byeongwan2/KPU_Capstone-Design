using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextEvent : MonoBehaviour
{
    public void Create_First_Monster()
    {
        StageManager2.instance.NextChapter();
    }
}
