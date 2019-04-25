using UnityEngine;
using System.Collections;

public class PrPickupObject : MonoBehaviour {
   
      
    [HideInInspector]
    public GameObject Player;

    public Renderer MeshSelector;

    // Use this for initialization
    void Start () {
        gameObject.tag ="Pickup";
        if (MeshSelector)
            MeshSelector.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    protected virtual void PickupObjectNow(int ActiveWeapon)
    {
        SendMessageUpwards("TargetPickedUp", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
        
    }
    void OnTriggerStay(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            if (MeshSelector)
                MeshSelector.enabled = true;
            Player = other.gameObject;
        }
  
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (MeshSelector)
                MeshSelector.enabled = false;
            Player = null;
        }

       
    }
   
}
