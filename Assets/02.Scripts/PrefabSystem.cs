using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSystem :MonoBehaviour {
    [SerializeField]
    private int MAXPLAYERBOMB = 10;


    private GameObject bomb;
    private List<GameObject> bombPool = new List<GameObject>();
	void Start()
    { 

        bomb = Resources.Load("Prefabs/PlayerBomb") as GameObject;
        for(int i = 0; i < MAXPLAYERBOMB; i++)
        {
            var obj = Instantiate<GameObject>(bomb,this.transform);
            obj.SetActive(false);
            bombPool.Add(obj);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActiveBomb()
    {
        for (int i = 0; i < MAXPLAYERBOMB; i++)
        {
            if (bombPool[i].activeSelf == false)
            {
                bombPool[i].SetActive(true);
                return;
            }
        }
    }
}
