using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageManager2 : MonoBehaviour
{
    Stage1 stage1;
    public static StageManager2 instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this.gameObject); Debug.Log("매니저중복추적"); }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        stage1 = GameObject.Find("Stage1").GetComponent<Stage1>();

    }

    public void NextChapter(eCHAPTER _chapter)
    {
        stage1.CreateRobotFirst(_chapter);
    }

    public bool Is_AllMonsterDie()
    {
        return true;                        //테스트용
        if (stage1.Check_MonsterLife()) return true;
        else return false;
    }

    public void Start_Stage2()
    {
        Debug.Log("2스테이지");
        SceneManager.LoadScene("SceneLoader");
    }
}
