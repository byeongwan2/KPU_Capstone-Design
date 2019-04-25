using UnityEngine;
using System.Collections;

public class PrCopyPosition : MonoBehaviour {
    public Transform targetObject;
    public bool moveToFloor = true;
    public float floorOffset = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        if (targetObject)
            transform.position = targetObject.transform.position;

        if (moveToFloor)
            transform.position = new Vector3(transform.position.x, 0 + floorOffset, transform.position.z);
    }
}
