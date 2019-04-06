using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eCHAPTER { ONE, TWO, THREE , FOUR }
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
        switch(chap)
        {
            case eCHAPTER.ONE:
                stage01.Appear_Sight_Height();
                chap = eCHAPTER.TWO;
                break;
            case eCHAPTER.TWO:
                stage01.Open_Door_One();
                chap = eCHAPTER.THREE;
                break;
            case eCHAPTER.THREE:
                stage01.Open_Door_Two();
                chap = eCHAPTER.FOUR;
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

    public void Input_EventKey()
    {
        if(stage01.Is_Get_Event_One())
        {
            Renew();
        }
    }
}
