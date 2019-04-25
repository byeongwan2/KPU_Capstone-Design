using UnityEngine;
using System.Collections;

public class PrEnvironmentZone : MonoBehaviour {
    [Range(-10f, 10f)]
    public float CameraHeight = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
       
    }
}
