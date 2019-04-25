using UnityEngine;
using System.Collections;

public class PrDebugTime : MonoBehaviour {
    public float TimeSpeed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = TimeSpeed;
	}
}
