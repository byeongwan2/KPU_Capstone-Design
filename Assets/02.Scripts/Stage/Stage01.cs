using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage01 : MonoBehaviour {

    GameSystem system;
    GameObject []mapObject1_sight = new GameObject[15];

    public ParentChapter[] chapter= new ParentChapter[10];
	void Start ()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        mapObject1_sight = GameObject.FindGameObjectsWithTag("Map_Object");
        foreach (var obj in mapObject1_sight)
        {
            obj.SetActive(false);
        }
        chapter[0].Set_Init(eCHAPTER.ONE);
        chapter[1].Set_Init(eCHAPTER.ONE);
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






}
