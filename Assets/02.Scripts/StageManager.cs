using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    Stage01 stage01;
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
    public void Renew(float _sight_range,float _culling_range)
    {
        stage01.Culling_Sight_Height(_sight_range,_culling_range);
    }
}
