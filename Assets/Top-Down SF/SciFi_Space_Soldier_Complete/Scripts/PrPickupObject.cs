using UnityEngine;
using System.Collections;

public class PrPickupObject : MonoBehaviour {
   
      
    [HideInInspector]
    public GameObject Player;

    public Renderer MeshSelector;

    [HideInInspector]
    public bool ShowSelectorAlways = false;
    //[HideInInspector]
    public PrPlayerSettings ColorSetup;
    private bool activeColor = false;

    [Header("HUD")]
    public bool showText = false;
    private UnityEngine.UI.Text UseText;

    [HideInInspector]
    public string itemName = "item";
    [HideInInspector]
    public string[] weaponNames;

    // Use this for initialization
    void Start () {
        gameObject.tag ="Pickup";
        if (MeshSelector)
        {
            if (ColorSetup)
            {
                ShowSelectorAlways = ColorSetup.AlwaysShowPickups;
                ChangeColor();
            }

            if (!ShowSelectorAlways)
                MeshSelector.enabled = false;
            else
                MeshSelector.enabled = true;
        }
        
        UseText = GetComponentInChildren<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    protected virtual void SetName()
    {
        //set Name
    }

    public virtual void Initialize()
    {
        SetName();

        if (MeshSelector)
        {
            if (ColorSetup)
            {
                ShowSelectorAlways = ColorSetup.AlwaysShowPickups;
                ChangeColor();
            }
                
            if (!ShowSelectorAlways)
                MeshSelector.enabled = false;
            else
                MeshSelector.enabled = true;
        }

        if (ColorSetup)
        {
            UseText = GetComponentInChildren<UnityEngine.UI.Text>() as UnityEngine.UI.Text;

            if (UseText)
            {
                SetName();
                showText = ColorSetup.showPickupText;
                UseText.text = itemName;
                UseText.color = ColorSetup.PickupTextColor;
                UseText.lineSpacing = 1f;
                UseText.enabled = false;
            }
            else
            {
                Debug.Log("No Text Found");
            }
        }
    }

    protected virtual void ChangeColor()
    {
        if (ColorSetup && activeColor)
        {
            MeshSelector.material.SetColor("_TintColor", ColorSetup.ActivePickupColor);
            if (showText && UseText)
            {
                UseText.enabled = true;
            }
        }
            
        else if (ColorSetup && !activeColor)
        {
            MeshSelector.material.SetColor("_TintColor", ColorSetup.InactivePickupColor);
            if (showText && UseText)
            {
                UseText.enabled = false;
            }
        }
    
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
            if (MeshSelector && !activeColor)
            {
                MeshSelector.enabled = true;
                activeColor = true;
                ChangeColor();

            }
               
            Player = other.gameObject;
        }
  
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (MeshSelector)
            {
                MeshSelector.enabled = false;
                activeColor = false;
                ChangeColor();
            }
            Player = null;
        }

       
    }
   
}
