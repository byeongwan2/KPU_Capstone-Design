using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage01 : MonoBehaviour {

    RezenCore rezenCore;
	// Use this for initialization
	void Start () {
        rezenCore = GetComponent<RezenCore>();
        rezenCore.Init(20);
        TestZen();

    }


    void TestZen()
    {
        for(int i = 0; i < 1; i++) 
        rezenCore.Work();
    }
	
	void Create_NodeMap()
    {

    }
}
