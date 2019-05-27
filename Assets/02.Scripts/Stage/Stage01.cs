using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage01 : MonoBehaviour
{
    Alien monster;
    GameSystem system;
    GameObject []mapObject1_sight = new GameObject[15];
    Door one_door;
    Door two_door;
    Door three_door;
    public Chapter[] chapter= new Chapter[5];
    public Chapter_Back[] chapter_back= new Chapter_Back[5];
    public Chapter_Event[] chapter_event = new Chapter_Event[5];

    bool IsNewMonster = false;
    //GameObject basic_monster = new

    Vector3[] newPos = new Vector3[7];
    void Start ()
    {
        monster = GameObject.Find("SciFi_Alien_Soldier_Melee").GetComponent<Alien>();
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        mapObject1_sight = GameObject.FindGameObjectsWithTag("Map_Object");
        foreach (var obj in mapObject1_sight)
        {
            obj.SetActive(false);
        }
        one_door = GameObject.Find("Door1").GetComponent<Door>();
        one_door.Setting(eDir.FORWARD);
        two_door = GameObject.Find("Door2").GetComponent<Door>();
        two_door.Setting(eDir.BACK);
        three_door = GameObject.Find("Door3").GetComponent<Door>();
        three_door.Setting(eDir.UP);
        chapter[0].Set_Init(eCHAPTER.ONE);
        chapter[1].Set_Init(eCHAPTER.TWO);
        chapter_back[0].Set_Init(eCHAPTER.ONE);

        chapter_event[0].Set_Init(eCHAPTER.THREE);

        chapter[2].Set_Init(eCHAPTER.FOUR);

        newPos[0] = GameObject.Find("newPoint").transform.position;
        newPos[1] = GameObject.Find("newPoint2").transform.position;
        Create_Monster();
    }
    void Update()
    {
        if (!IsNewMonster) 
        {
            if (monster.Get_State() == ENEMY_STATE.DIE)
            {
                IsNewMonster = true;
                Pour_Monster();
            }
        }
    }

    public void Appear_Sight_Height()
    {
        Debug.Log("1번스테이지시작");
        foreach (var obj in mapObject1_sight)
        {
            obj.SetActive(true);
        }
    }

    void Create_Monster()
    {    
        PrefabSystem.instance.Create_Prefab(TYPE.MONSTER, GameObject.Find("SciFi_Alien_Soldier_Melee"), 50);      //오브젝트풀        
    }

    void Pour_Monster()
    {
        for (int i = 0; i < 15; i++)
        {
            Debug.Log("에일리언생성");
            var monster = PrefabSystem.instance.Active_Prefab(TYPE.MONSTER).GetComponent<Alien>();           //게임오브젝트가 리턴되므로 이부분 수정해야함 // SetActive(true) 상태로 리턴
            monster.Set_NewPosition(newPos[0]);
        }

    }

    public void Close_Door_One()
    {
        one_door.Close();
        Debug.Log("1번문 닫힘");
        three_door.Close();
    }

    public void Culling_Sight_Height()
    {
        foreach (var obj in mapObject1_sight)
        {
            obj.SetActive(false);
        }
    }

    public void Open_Door_One()
    {
        one_door.Active(0.0f);
        Debug.Log("1번문 열림");
    }

    public void Open_Door_Two()
    {
        two_door.Active(0.0f);
        Debug.Log("2번문 열림");
    }

    public bool Is_Get_Event_One()
    {
     //   if (chapter_event[0].isReady)
     //       return true;
        return false;
    }





}
