using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eCHAPTER { ONE, TWO, THREE }
public class StageManager : MonoBehaviour {

    Stage01 stage01;
    eCHAPTER chap = eCHAPTER.ONE;
    public static StageManager instance = null;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this.gameObject); Debug.Log("매니저중복추적"); }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        stage01 = GameObject.Find("Stage01").GetComponent<Stage01>();
    }
    public void Renew()
    {
        stage01.Appear_Sight_Height();
        switch(chap)
        {
            case eCHAPTER.ONE:
                chap = eCHAPTER.TWO;
                break;
            case eCHAPTER.TWO:
                chap = eCHAPTER.THREE;
                break;
        }
    }

    public void Backnew()
    {
        stage01.Culling_Sight_Height();
        switch (chap)
        {
            case eCHAPTER.TWO:
                chap = eCHAPTER.ONE;
                break;
            case eCHAPTER.THREE:
                chap = eCHAPTER.TWO;
                break;
        }
    }

    public eCHAPTER Get_NowChapter()
    {
        return chap;
    }
}
