using UnityEngine;
using System.Collections;

public class PrUsableDevice : MonoBehaviour {

    [Header("Setings")]
    public bool IsEnabled = true;
	public GameObject AffectedTarget;
	public string MessageToSend = "Action";

    public float UseDelay = 1.0f;
	private bool InUse = false;
	private float InUseTimer = 0.0f;

    [HideInInspector]
	public GameObject User;

    public enum Key
    {
        None,
        Blue,
        Yellow,
        Red
    }

    [Header("Key Settings")]
    public Key KeyType;
    //private bool UnlockedKey = false;

    [Header("HUD")]
    public GameObject MeshSelector;
    public GameObject UseBar;
    public Color UseBaseColor = Color.white;
    private GameObject UseBarParent;
    private UnityEngine.UI.Text UseText;

    [Header("Debug")]
    public Mesh InteractIcon;

    // Use this for initialization
    void Start () {

        if (MeshSelector)
        {
            MeshSelector.SetActive(false);
            UseText = MeshSelector.GetComponentInChildren<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
            if (UseBar)
            {
                UseBar.GetComponent<UnityEngine.UI.Image>().color = UseBaseColor;
                UseBarParent = UseBar.transform.parent.gameObject;
                UseBarParent.SetActive(false);
            }
              
        }
            

        if (KeyType != Key.None)
        {
            if (AffectedTarget)
                AffectedTarget.SendMessage("SetKeyColors",(int)KeyType,  SendMessageOptions.DontRequireReceiver);
        }
     
     

    }
	
	// Update is called once per frame
	void Update () {
		if (IsEnabled && AffectedTarget)
		{
			if (InUse)
			{
				if (InUseTimer < UseDelay)
				{
					InUseTimer += Time.deltaTime;
                    if (UseBar)
                        UseBar.GetComponent<UnityEngine.UI.Image>().transform.localScale = new Vector3((1 / UseDelay) * InUseTimer, 0.6f, 1.0f);

                }
				else if (InUseTimer >= UseDelay)
                {
					ResetUse();
				}
			}
		}
        if (!AffectedTarget && IsEnabled)
        {
            IsEnabled = false;
        }
	}

	public void Use()
	{
        if (IsEnabled && !InUse)
        {
            if (KeyType == Key.None)
            {
                InUse = true;
            }
            else if (KeyType == Key.Blue)
            {
                if (User.GetComponent<PrTopDownCharInventory>().BlueKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().BlueKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else if (User.GetComponent<PrTopDownCharInventory>().FullKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().FullKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else
                {
                    CancelUse();
                }
            }
            else if (KeyType == Key.Yellow)
            {
                if (User.GetComponent<PrTopDownCharInventory>().YellowKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().YellowKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else if (User.GetComponent<PrTopDownCharInventory>().FullKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().FullKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else
                {
                    CancelUse();
                }
            }
            else if (KeyType == Key.Red)
            {
                if (User.GetComponent<PrTopDownCharInventory>().RedKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().RedKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else if (User.GetComponent<PrTopDownCharInventory>().FullKeys > 0)
                {
                    User.GetComponent<PrTopDownCharInventory>().FullKeys -= 1;
                    KeyType = Key.None;
                    InUse = true;
                }
                else
                {
                    CancelUse();
                }
            }
           

        }
        if (KeyType == Key.None && AffectedTarget)
        {
            AffectedTarget.SendMessage("ResetActiveColor", SendMessageOptions.DontRequireReceiver);
        }
        if (InUse && UseBarParent)
            UseBarParent.SetActive(true);
        InUseTimer = 0.0f;

    }
    
    public void CancelUse()
    {
        if (UseBarParent)
            UseBarParent.SetActive(false);
        InUse = false;
        InUseTimer = 0.0f;
        User.SendMessage("StopUse", SendMessageOptions.DontRequireReceiver);
        User = null;
    }

	public void ResetUse()
	{
        if (UseBarParent)
            UseBarParent.SetActive(false);
        InUse = false;
		InUseTimer = 0.0f;
        if (AffectedTarget)
		    AffectedTarget.SendMessage(MessageToSend, SendMessageOptions.DontRequireReceiver);
		User.SendMessage("StopUse", SendMessageOptions.DontRequireReceiver);
		User = null;
        if (UseBar)
            UseBar.GetComponent<UnityEngine.UI.Image>().transform.localScale = new Vector3(0f, 1.0f, 1.0f);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && MeshSelector && IsEnabled)
        {
            MeshSelector.SetActive(true);
            if (UseText && KeyType != Key.None)
            {
                if (other.gameObject.GetComponent<PrTopDownCharInventory>().RedKeys > 0)
                {
                    UseText.text = "Use";
                    UseText.color = Color.white;
                    UseText.lineSpacing = 1f;
                }
                
                else
                {
                    UseText.text = "Need Red Key";
                    UseText.color = Color.red;
                    UseText.lineSpacing = 1f;
                }
                   


            }

        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player") && MeshSelector)
        {
            MeshSelector.SetActive(false);
        }


    }


    void OnDrawGizmos()
    {
        if (AffectedTarget)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, AffectedTarget.transform.position);
            Gizmos.color = Color.cyan * 3;
            Gizmos.DrawMesh(InteractIcon, new Vector3(transform.position.x, 3.5f, transform.position.z), transform.rotation, Vector3.one);
        }
    }
}
