using UnityEngine;
using System.Collections;

public class PrSpawnPoint : MonoBehaviour {

    public bool isFull = false;
    Collider otherCollider;

	// Use this for initialization
	void Start () {
	    if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (isFull && !otherCollider)
        {
            isFull = false;
        }
	}
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFull = true;
        }
    }*/

    private void OnTriggerStay (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFull = true;
            otherCollider = other;
           
        }
     
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFull = false;
        }
    }
}
