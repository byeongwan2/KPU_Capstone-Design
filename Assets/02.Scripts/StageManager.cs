using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//원래는 산맵을 그대로 이용하는줄 알고 만들었던 스테이지매니저  지금은 필요없어졌음
public enum eCHAPTER { ONE, TWO, THREE , FOUR ,FIVE}
public class StageManager : MonoBehaviour {

    Stage01 stage01;
    eCHAPTER chap = eCHAPTER.ONE;
    public static StageManager instance = null;
    bool ischap = false;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this.gameObject); Debug.Log("매니저중복추적"); }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        stage01 = GameObject.Find("Stage01").GetComponent<Stage01>();
        ischap = true ;
    }
    public void Renew()
    {
        if (!ischap) return;
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
            case eCHAPTER.FOUR:
                stage01.Close_Door_One();
                chap = eCHAPTER.FIVE;
                break;

        }
        ischap = false;
    }
    public void Set_IsChap()
    {
        ischap = true;
    }

    public void Backnew()
    {
        switch (chap)
        {
          
            case eCHAPTER.TWO:
                stage01.Culling_Sight_Height();
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
