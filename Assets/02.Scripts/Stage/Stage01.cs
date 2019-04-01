using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage01 : MonoBehaviour {

    GameSystem system;
    GameObject []mapObject1_sight = new GameObject[15];
    Door one_door;
    Door two_door;
    public ParentChapter[] chapter= new ParentChapter[10];
	void Start ()
    {
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
        chapter[0].Set_Init(eCHAPTER.ONE);
        chapter[1].Set_Init(eCHAPTER.ONE);
        chapter[2].Set_Init(eCHAPTER.TWO);
    }

    public void Appear_Sight_Height()
    {
        foreach (var obj in mapObject1_sight)
        {
            obj.SetActive(true);
        }
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
    }






}
