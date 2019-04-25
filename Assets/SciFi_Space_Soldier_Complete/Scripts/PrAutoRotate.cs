using UnityEngine;
using System.Collections;

public class PrAutoRotate : MonoBehaviour {

    public float Speed = 1.0f;
    public Vector3 Direction = Vector3.up;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Direction.x * Time.deltaTime * Speed, Direction.y * Time.deltaTime * Speed, Direction.z * Time.deltaTime * Speed);
	}
}
