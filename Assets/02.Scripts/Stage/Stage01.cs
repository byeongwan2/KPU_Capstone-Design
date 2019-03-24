using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum eCHAPTER { ONE,TWO,THREE}
public class Stage01 : MonoBehaviour {

    GameSystem system;
    RezenCore rezenCore;
    GameObject []mapObject1 = new GameObject[1500];
    GameObject[] mapObject2 = new GameObject[1500];
    eCHAPTER nowChapter = eCHAPTER.ONE;

    public Chapter[] chapter= new Chapter[10];
	void Start () {
        //rezenCore = GetComponent<RezenCore>();
        //rezenCore.Init(20);
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        mapObject1 = GameObject.FindGameObjectsWithTag("Map_Object");
        Debug.Log(system.pPlayer2.transform.position.y);
        foreach (var obj in mapObject1)
        {
            if (Check.Distance(obj.transform.position, system.pPlayer2.transform.position) > 30.0f)
            {
                obj.SetActive(false);
                continue;
            }
            if (obj.transform.position.y > system.pPlayer2.transform.position.y + 5)
            {
                obj.SetActive(false);
            }
        }
        chapter[0].sight_range = 5.0f;          //숫자가 클수록 플에이어 위에 위치한 사물들이보임
        chapter[0].culling_range = 5.0f;        //숫자가클수록 플레이어 밑에위치한 사물들이보임
        chapter[1].sight_range = 6.0f;
        chapter[1].culling_range = 6.0f;
    }

    public void Culling_Sight_Height(float _sight_range,float _culling_range)
    {
        foreach (var obj in mapObject1)
        {
            if(Check.Distance(obj.transform.position,system.pPlayer2.transform.position) > 30.0f){
                obj.SetActive(false);
                continue;
            }
            if(obj.transform.position.y  <= system.pPlayer2.transform.position.y+ _sight_range && obj.transform.position.y >= system.pPlayer2.transform.position.y - _culling_range)
            {
                obj.SetActive(true);
            }
            else 
            {
                obj.SetActive(false);
            }
        }
    }

    
  
  
	
	
}
