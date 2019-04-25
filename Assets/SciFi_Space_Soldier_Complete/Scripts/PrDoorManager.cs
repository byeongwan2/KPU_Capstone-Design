using UnityEngine;
using System.Collections;

public class PrDoorManager : MonoBehaviour {

    public bool IsActive = true;

    [Header("Generic Vars")]
    public GameObject[] TargetGameObjects;
	public float DoorX = 1.0f;
	public float DoorSpeed = 2.0f;

	public bool AffectCharacters = true;
	public bool AffectBullets = false;
	public bool AffectEnemys = true;

	private bool Action = false;
	

    [Header("Sound FX")]
    public AudioClip DoorSlideSFX;
    private AudioSource Audio;

    [Header("Visuals")]
    public Renderer[] DoorPanels;
    public Light[] PanelLights;

    public Color ActiveColor  = Color.green;
    public Color InactiveColor = Color.green * 0.25f;

    private Color ActualColor = Color.green;

    private Color LightActualColor = Color.green;
    public Color LightActiveColor;
    public Color LightInactiveColor;
    public Color LightLockedColor;

    // Use this for initialization
    void Start () {
        //ResetActiveColor();
        SetPanelColors();
        SetNavigationCarve();
        //SetKeyColors(3);
        Audio = GetComponent<AudioSource>() as AudioSource;
    }
	
    void SetKeyColors(int KeyType)
    {
        if (KeyType == 1)
            InactiveColor = Color.blue * 0.8f;
        else if (KeyType == 2)
            InactiveColor = Color.yellow * 0.8f;
        else if (KeyType == 3)
            InactiveColor = Color.red * 0.8f;

        LightInactiveColor = LightLockedColor;

        SetPanelColors();
    }

    void ResetActiveColor()
    {
        //InactiveColor = InactiveColor;
        SetPanelColors();
    }

    void SetPanelColors()
    {
        if (IsActive)
        {
            ActualColor = ActiveColor;
            LightActualColor = LightActiveColor;
        }
        else if (!IsActive)
        {
            ActualColor = InactiveColor;
            LightActualColor = LightInactiveColor;
        }

        if (DoorPanels.Length > 0)
        {
            foreach (Renderer Panel in DoorPanels)
            {
                Panel.materials[0].SetColor("_EmissionColor", ActualColor);
            }
            foreach (Light PanelLight in PanelLights )
            {
                PanelLight.color = LightActualColor;
            }
        }

        

    }

	// Update is called once per frame
	void Update () {
		if (IsActive && Action)
		{
			foreach (GameObject GOTarget in TargetGameObjects)
			{
				GOTarget.transform.localPosition = Vector3.Lerp(GOTarget.transform.localPosition, new Vector3( DoorX , GOTarget.transform.localPosition.y, GOTarget.transform.localPosition.z), Time.deltaTime  * DoorSpeed);
			}
		}
		else 
		{
			foreach (GameObject GOTarget in TargetGameObjects)
			{
				if (Mathf.Approximately( GOTarget.transform.localPosition.x , 0.0f) == false)
				{
					GOTarget.transform.localPosition = Vector3.Lerp(GOTarget.transform.localPosition, new Vector3(0,GOTarget.transform.localPosition.y,GOTarget.transform.localPosition.z), Time.deltaTime  * DoorSpeed);
				}
			}
		}
	
	}

    void SetNavigationCarve()
    {
        foreach (GameObject Door in TargetGameObjects)
        {
            if (Door.GetComponent<UnityEngine.AI.NavMeshObstacle>() != null)
            {
                Door.GetComponent<UnityEngine.AI.NavMeshObstacle>().carving = !IsActive;
            }
        }
    }

    public void SetActive()
    {
        /*
        if (IsActive)
        {
            IsActive = false;
        }

        else
        {
            IsActive = true;
        }*/
        IsActive ^= true;

        SetNavigationCarve();

        SetPanelColors();
    }

	void OnTriggerStay(Collider other) {
        if (IsActive)
        {
            if (other.gameObject.tag == "Player" && AffectCharacters && TargetGameObjects != null)
            {
                if (!Action)
                    Audio.PlayOneShot(DoorSlideSFX);
                Action = true;
            }
            else if (other.gameObject.tag == "Enemy" && AffectEnemys && TargetGameObjects != null)
            {
                if (!Action)
                    Audio.PlayOneShot(DoorSlideSFX);
                Action = true;
            }
        }
		
    }

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && AffectCharacters && TargetGameObjects != null)
		{
            if (Action)
                Audio.PlayOneShot(DoorSlideSFX);
            Action = false;
		}
        else if (other.gameObject.tag == "Enemy" && AffectEnemys && TargetGameObjects != null)
        {
            if (Action)
                Audio.PlayOneShot(DoorSlideSFX);
            Action = false;
        }
    }
	
}
