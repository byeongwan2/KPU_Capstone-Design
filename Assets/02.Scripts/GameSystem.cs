using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    public GameObject player;
	// Use this for initialization
	void Awake ()
    {
        player = GameObject.Find("Player");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
