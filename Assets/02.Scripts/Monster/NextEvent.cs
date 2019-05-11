using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextEvent : MonoBehaviour
{
    public void Create_Monster(eCHAPTER _chapter)
    {
        StageManager2.instance.NextChapter(_chapter);
    }
}
