using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage01 : MonoBehaviour {

    RezenCore rezenCore;
	// Use this for initialization
	void Start () {
        rezenCore = new RezenCore();
        rezenCore.Init(20);
	}

	
	
}
