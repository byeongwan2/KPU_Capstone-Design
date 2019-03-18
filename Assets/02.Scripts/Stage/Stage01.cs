using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CHAPTER { ONE,TWO,THREE}
public class Stage01 : MonoBehaviour {

    GameSystem system;
    RezenCore rezenCore;
    GameObject []mapObject = new GameObject[3000];
    CHAPTER nowChapter = CHAPTER.ONE;
	void Start () {
        //rezenCore = GetComponent<RezenCore>();
        //rezenCore.Init(20);
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        mapObject = GameObject.FindGameObjectsWithTag("Map_Object");
        Debug.Log(system.pPlayer2.transform.position.y);
        Culling_Sight_Height();
    }

    void Culling_Sight_Height()
    {
        foreach (var obj in mapObject)
        {
            
            if(obj.transform.position.y < system.pPlayer2.transform.position.y + 3 && obj.transform.position.y > system.pPlayer2.transform.position.y -3)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
            if(obj.transform.position.y == 0)
            {
                obj.SetActive(true);
            }
        }
    }

    

    void Update()
    {
       switch (nowChapter)
        {
            case CHAPTER.ONE:
                if(-4.1 < system.pPlayer2.transform.position.y)
                {
                    nowChapter = CHAPTER.TWO;
                    Culling_Sight_Height();
                }
                break;
            case CHAPTER.TWO:
                if(-0.1 < system.pPlayer2.transform.position.y)
                {
                    nowChapter = CHAPTER.THREE;
                    Culling_Sight_Height();
                }
                break;
        }
    }


  
	
	
}
