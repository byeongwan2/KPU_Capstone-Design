using UnityEngine;
using System.Collections;

public class PrLookToCamera : MonoBehaviour {

    //private Transform CameraTarget;

	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        //transform.LookAt(Camera.main.transform);
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
       // transform.localRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, 180, 0));
	}
}
