using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNpc : Npc {

    // Use this for initialization
    void Start()
    {

        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Work();
    }
}
