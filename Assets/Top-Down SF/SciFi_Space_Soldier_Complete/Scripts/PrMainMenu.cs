using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrMainMenu : MonoBehaviour {

    [Header("HUD Setup")]
    public PrPlayerSettings playersSettings;
    public string[] levelsToLoad;

    public string[] menus;
    public GameObject[] actualMenus;
    public float menusOffset = 100.0f;
    public GameObject menuText;

    //Selected variables
    public int menuSelected = 0;
    [HideInInspector]
    public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Reload",
        "EquipWeapon", "Sprint", "Aim", "ChangeWTrigger", "Roll", "Use", "Crouch", "ChangeWeapon", "Throw"  ,"Fire", "Mouse ScrollWheel"};
    private bool m_isAxisInUse = false;
    //private bool m_isAxisInUse2 = false;
    //private bool m_isAxisInUse3 = false;
    //private bool m_isAxisInUse4 = false;

    public Camera menuCamera;

    // Use this for initialization
    void Start () {

        if (playersSettings)
        {
            playerCtrlMap = playersSettings.playerCtrlMap;
        }

        if (menus.Length > 0 && menuText)
        {
            actualMenus = new GameObject[menus.Length];
            int menuInt = 0;
            Vector3 originalPos = menuText.GetComponent<RectTransform>().localPosition;
            Vector3 originalScale = menuText.GetComponent<RectTransform>().localScale;
            float offset = 0.0f;
            foreach (string m in menus)
            {
                GameObject mTemp = Instantiate(menuText, menuText.transform.position, menuText.transform.rotation);
                //mTemp.transform.parent = menuText.transform.parent;
                mTemp.transform.SetParent(menuText.transform.parent);
                mTemp.GetComponent<RectTransform>().localPosition = originalPos;
                mTemp.GetComponent<RectTransform>().localScale = originalScale;
                mTemp.GetComponent<UnityEngine.UI.Text>().text = m;
                mTemp.name = m + "_GameObj";
                mTemp.GetComponent<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
                mTemp.GetComponent<RectTransform>().localPosition += new Vector3(0, offset, 0);
                mTemp.GetComponent<PrTitleID>().menuID = menuInt;
                actualMenus[menuInt] = mTemp;
                offset -= menusOffset;
                menuInt += 1;
            }

            menuText.SetActive(false);

            ChangeSelection(0);
        }
        
	}
	
    void ChangeSelection(int value)
    {
        actualMenus[menuSelected].GetComponent<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;

        if (value > 0)
        {
            if (menuSelected < menus.Length -1)
            {
                menuSelected += 1;
            }
            else
            {
                menuSelected = 0;
            }
        }
        else if (value < 0)
        {
            if (menuSelected > 0)
            {
                menuSelected -= 1;
            }
            else
            {
                menuSelected = menus.Length - 1;
            }
        }
        else
        {
            menuSelected = 0;
        }

        actualMenus[menuSelected].GetComponent<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
        actualMenus[menuSelected].GetComponent<PrTitleID>().StartAnim();
    }

    void ChangeSelectionMouse(int value)
    {
        actualMenus[menuSelected].GetComponent<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
        menuSelected = value;
        actualMenus[menuSelected].GetComponent<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
    }

    void GetMenuSelected()
    {
        if (Input.GetButtonDown(playerCtrlMap[11]) || Input.GetButton(playerCtrlMap[15]))
        {
           
            if (menuSelected == 0)
            {
                playersSettings.TypeSelected = PrPlayerSettings.GameMode.SinglePlayer;
            }
            else if (menuSelected == 1)
            {
                playersSettings.TypeSelected = PrPlayerSettings.GameMode.Cooperative;
            }
            else if (menuSelected == 2)
            {
                playersSettings.TypeSelected = PrPlayerSettings.GameMode.DeathMatch;
            }
            else if (menuSelected == 3)
            {
                playersSettings.TypeSelected = PrPlayerSettings.GameMode.Survival;
            }
            else if (menuSelected == 4)
            {
                playersSettings.TypeSelected = PrPlayerSettings.GameMode.TowerDefense;
            }
            Debug.Log("selected " + menuSelected);

            SceneManager.LoadScene(levelsToLoad[menuSelected]);

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxisRaw(playerCtrlMap[1]) > 0)
        {
            if (m_isAxisInUse == false)
            {
                // Call your event function here.
                Debug.Log("Changing menu");
                ChangeSelection(-1);
                m_isAxisInUse = true;
            }
        }
        else if (Input.GetAxisRaw(playerCtrlMap[1]) < 0)
        {
            if (m_isAxisInUse == false)
            {
                // Call your event function here.
                Debug.Log("Changing menu");
                ChangeSelection(1);
                m_isAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw(playerCtrlMap[1]) == 0.0f)
        {
            m_isAxisInUse = false;
        }

        RaycastHit hit;

        Ray ray = menuCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Canvas")
            {
                //Debug.Log(hit.collider.name);
                ChangeSelectionMouse(hit.collider.GetComponent<PrTitleID>().menuID);
            }
            
            
        }

        GetMenuSelected();
    }
}

