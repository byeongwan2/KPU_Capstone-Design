using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrEndLevelZone : MonoBehaviour {

    [Header("Debug")]

    public Mesh areaMesh;
    public Mesh textMesh;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        Color green = new Vector4(0, 1, 0, 0.2f);
        Gizmos.color = green * 2;
        if (areaMesh)
        {
            Gizmos.DrawMesh(areaMesh, transform.position, Quaternion.identity, transform.localScale);
        }
        if (textMesh)
        {
            Gizmos.DrawMesh(textMesh, transform.position, Quaternion.identity, transform.localScale);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("entering End Zone");
            SendMessageUpwards("PlayerReachedEndZone", other.GetComponent<PrTopDownCharController>().playerNmb, SendMessageOptions.DontRequireReceiver);
        }

    }

    
}
