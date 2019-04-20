using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrPlayerSelection : MonoBehaviour {

    [Header("Game Setup")]
    public PrPlayerSettings playersSettings;
    public PrWeaponList weaponList;
    public PrLevelSettings levelSettings;

    public Transform[] playerSelectPos;
    [HideInInspector]
    public GameObject[] allPlayerPrefabs;
    private GameObject[] actualPlayerPrefabs;
    public int playerCount = 0;
    public int playerReadyCount = 0;
    public bool autoStartPlayer1 = false;
    private int[] actualPlayerSelected;
    private bool[] playersInGame;
    private bool[] characterSelected;

    [HideInInspector]
    public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Reload",
        "EquipWeapon", "Sprint", "Aim", "ChangeWTrigger", "Roll", "Use", "Crouch", "ChangeWeapon", "Throw"  ,"Fire", "Mouse ScrollWheel"};

    private bool m_isAxisInUse = false;
    private bool m_isAxisInUse2 = false;
    private bool m_isAxisInUse3 = false;
    private bool m_isAxisInUse4 = false;

    [Header("Debug")]
    public Mesh targetArrow;

    // Use this for initialization
    void Start() {
        //reset actual Level parameter from levelSettings
        levelSettings.actualLevel = 0;

        //Initialize variables
        actualPlayerPrefabs = new GameObject[4];
        /*
        if (allPlayerPrefabs.Length > 0)
        {
            for (int x = 0; x < playerCount; x++)
            {
                if (playerSelectPos.Length == 0)
                {
                    Debug.LogError("You need to assign a spawn position for each character");
                }

                SpawnPrefab(x, 0);
            }
        }
        */
        if (playersSettings)
        {
            playersSettings.finalPlayerCount = 0;

            allPlayerPrefabs = new GameObject[playersSettings.availableCharacters.Length];
            allPlayerPrefabs = playersSettings.availableCharacters;

            playerCtrlMap = playersSettings.playerCtrlMap;
            playersSettings.playersInGame = new bool[4];
            playersSettings.selectedCharacters = new GameObject[4];
        }
        actualPlayerSelected = new int[4];
        playersInGame = new bool[4];
        characterSelected = new bool[4];

        if (autoStartPlayer1)
        {
            if (!playersInGame[0])
            {
                SpawnPrefab(0, 0);
                playersInGame[0] = true;
                playersSettings.playersInGame[0] = true;
                playerCount += 1;
            }
        }
    }

    void SpawnPrefab(int x, int playerPrefabInt)
    {
        GameObject tempPlayer = Instantiate(allPlayerPrefabs[playerPrefabInt], playerSelectPos[x].transform.position, playerSelectPos[x].transform.rotation) as GameObject;
        tempPlayer.transform.parent = this.transform;
        actualPlayerPrefabs[x] = tempPlayer.GetComponentInChildren<PrTopDownCharController>().transform.gameObject;
        if (playersSettings.TypeSelected != PrPlayerSettings.GameMode.SinglePlayer)
            tempPlayer.GetComponentInChildren<PrTopDownCharInventory>().SetPlayerColors(1, x, playersSettings);
        string characterDesc = tempPlayer.GetComponentInChildren<PrTopDownCharInventory>().characterName;
        playerSelectPos[x].GetComponentInChildren<UnityEngine.UI.Text>().text = "Player " + (x + 1) + "\n" + characterDesc;
        playerSelectPos[x].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
        DeactivateAllComponents(tempPlayer, playerSelectPos[x]);

    }

    void DeactivateAllComponents(GameObject playerPrefab, Transform spawnPos)
    {
        GameObject playerNode = playerPrefab.GetComponentInChildren<PrTopDownCharController>().transform.gameObject;
        playerNode.transform.parent = spawnPos;
        playerNode.transform.Find("HUD").gameObject.SetActive(false);
        playerNode.transform.Find("PlayerSelection").gameObject.SetActive(false);
        playerNode.GetComponent<PrTopDownCharController>().enabled = false;
        GameObject weapon = weaponList.weapons[1].gameObject;
        Transform weaponNode = playerNode.GetComponent<PrTopDownCharInventory>().WeaponR;
        SpawnWeaponAndDeactivate(weapon, weaponNode, playerNode);
        playerNode.GetComponent<PrTopDownCharInventory>().enabled = false;
        //playerNode.GetComponent<PrCharacterIK>().enabled = false;
        Destroy(playerPrefab);


    }

    void SpawnWeaponAndDeactivate(GameObject weapon, Transform weaponNode, GameObject playerNode)
    {
        GameObject tempWeapon = Instantiate(weapon, weaponNode.position, weaponNode.rotation) as GameObject;
        tempWeapon.transform.parent = weaponNode;
        tempWeapon.GetComponent<PrWeapon>().TurnOffLaser();
        playerNode.GetComponent<PrCharacterIK>().leftHandTarget = tempWeapon.transform.Find("ArmIK").transform;
        tempWeapon.GetComponent<PrWeapon>().enabled = false;
        tempWeapon.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    void DestroyPlayerPrefab(int playerNmb)
    {

        DestroyImmediate(actualPlayerPrefabs[playerNmb]);
    }

    void ChangeActualSelectedPlayer(int playerNmb, int Add)
    {
        if (Add == 1)
        {
            if (actualPlayerSelected[playerNmb] < (allPlayerPrefabs.Length - 1))
            {
                actualPlayerSelected[playerNmb] += 1;
            }
            else
            {
                actualPlayerSelected[playerNmb] = 0;
            }
        }
        else
        {
            if (actualPlayerSelected[playerNmb] > 0)
            {
                actualPlayerSelected[playerNmb] -= 1;
            }
            else
            {
                actualPlayerSelected[playerNmb] = allPlayerPrefabs.Length - 1;
            }
        }

        DestroyPlayerPrefab(playerNmb);

        SpawnPrefab(playerNmb, actualPlayerSelected[playerNmb]);
    }

    void GetPlayerStart()
    {
        if (Input.GetButtonDown("Player1Start"))
        {
            if (!playersInGame[0])
            {
                SpawnPrefab(0, 0);
                playersInGame[0] = true;
                playersSettings.playersInGame[0] = true;
                playerCount += 1;
            }
        }
        if (Input.GetButtonDown("Player2Start"))
        {
            if (!playersInGame[1])
            {
                SpawnPrefab(1, 0);
                playersInGame[1] = true;
                playersSettings.playersInGame[1] = true;
                playerCount += 1;
            }
        }
        if (Input.GetButtonDown("Player3Start"))
        {
            if (!playersInGame[2])
            {
                SpawnPrefab(2, 0);
                playersInGame[2] = true;
                playersSettings.playersInGame[2] = true;
                playerCount += 1;
            }
        }
        if (Input.GetButtonDown("Player4Start"))
        {
            if (!playersInGame[3])
            {
                SpawnPrefab(3, 0);
                playersInGame[3] = true;
                playersSettings.playersInGame[3] = true;
                playerCount += 1;
            }
        }

        playersSettings.finalPlayerCount = playerCount;
    }

    void GetChangePlayer()
    {
        if (playersInGame[0] && !characterSelected[0])
        {
            //Player 1 Selection
            if (Input.GetAxisRaw(playerCtrlMap[0]) > 0)
            {
                if (m_isAxisInUse == false)
                {
                    // Call your event function here.
                    // Debug.Log("Changing Player Right");
                    ChangeActualSelectedPlayer(0, 1);
                    m_isAxisInUse = true;
                }
            }
            else if (Input.GetAxisRaw(playerCtrlMap[0]) < 0)
            {
                if (m_isAxisInUse == false)
                {
                    // Call your event function here.
                    //Debug.Log("Changing Player Left");
                    ChangeActualSelectedPlayer(0, 0);
                    m_isAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw(playerCtrlMap[0]) == 0.0f)
            {
                m_isAxisInUse = false;
            }
        }

        if (playersInGame[1] && !characterSelected[1])
        {
            //Player 2 Selection
            if (Input.GetAxisRaw(playerCtrlMap[0] + "2") > 0)
            {
                if (m_isAxisInUse2 == false)
                {
                    // Call your event function here.
                    // Debug.Log("Changing Player 2 Right");
                    ChangeActualSelectedPlayer(1, 1);
                    m_isAxisInUse2 = true;
                }
            }
            else if (Input.GetAxisRaw(playerCtrlMap[0] + "2") < 0)
            {
                if (m_isAxisInUse2 == false)
                {
                    // Call your event function here.
                    //Debug.Log("Changing Player 2 Left");
                    ChangeActualSelectedPlayer(1, 0);
                    m_isAxisInUse2 = true;
                }
            }
            if (Input.GetAxisRaw(playerCtrlMap[0] + "2") == 0.0f)
            {
                m_isAxisInUse2 = false;
            }
        }

        if (playersInGame[2] && !characterSelected[2])
        {
            //Player 3 Selection
            if (Input.GetAxisRaw(playerCtrlMap[0] + "3") > 0)
            {
                if (m_isAxisInUse3 == false)
                {
                    // Call your event function here.
                    //Debug.Log("Changing Player 3 Right");
                    ChangeActualSelectedPlayer(2, 1);
                    m_isAxisInUse3 = true;
                }
            }
            else if (Input.GetAxisRaw(playerCtrlMap[0] + "3") < 0)
            {
                if (m_isAxisInUse3 == false)
                {
                    // Call your event function here.
                    // Debug.Log("Changing Player 3 Left");
                    ChangeActualSelectedPlayer(2, 0);
                    m_isAxisInUse3 = true;
                }
            }
            if (Input.GetAxisRaw(playerCtrlMap[0] + "3") == 0.0f)
            {
                m_isAxisInUse3 = false;
            }
        }
        if (playersInGame[3] && !characterSelected[3])
        {
            //Player 4 Selection
            if (Input.GetAxisRaw(playerCtrlMap[0] + "4") > 0)
            {
                if (m_isAxisInUse4 == false)
                {
                    // Call your event function here.
                    //Debug.Log("Changing Player 4 Right");
                    ChangeActualSelectedPlayer(3, 1);
                    m_isAxisInUse4 = true;
                }
            }
            else if (Input.GetAxisRaw(playerCtrlMap[0] + "4") < 0)
            {
                if (m_isAxisInUse4 == false)
                {
                    // Call your event function here.
                    //Debug.Log("Changing Player 4 Left");
                    ChangeActualSelectedPlayer(3, 0);
                    m_isAxisInUse4 = true;
                }
            }
            if (Input.GetAxisRaw(playerCtrlMap[0] + "4") == 0.0f)
            {
                m_isAxisInUse4 = false;
            }
        }
    }

    void GetPlayerSelected()
    {
        if (Input.GetButtonDown(playerCtrlMap[11]))
        {
            if (playersInGame[0])
            {
                characterSelected[0] = true;
                playerSelectPos[0].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
                playersSettings.selectedCharacters[0] = allPlayerPrefabs[actualPlayerSelected[0]];
                playerReadyCount += 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[11] + "2"))
        {
            if (playersInGame[1])
            {
                characterSelected[1] = true;
                playerSelectPos[1].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
                playersSettings.selectedCharacters[1] = allPlayerPrefabs[actualPlayerSelected[1]];
                playerReadyCount += 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[11] ))
        {
            if (playersInGame[2])
            {
                characterSelected[2] = true;
                playerSelectPos[2].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
                playersSettings.selectedCharacters[2] = allPlayerPrefabs[actualPlayerSelected[2]];
                playerReadyCount += 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[11] ))
        {
            if (playersInGame[3])
            {
                characterSelected[3] = true;
                playerSelectPos[3].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.SelectedTextColor;
                playersSettings.selectedCharacters[3] = allPlayerPrefabs[actualPlayerSelected[3]];
                playerReadyCount += 1;
            }
        }
        //Get Unselected
        if (Input.GetButtonDown(playerCtrlMap[13]))
        {
            if (playersInGame[0])
            {
                characterSelected[0] = false;
                playerSelectPos[0].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
                playerReadyCount -= 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[13] + "2"))
        {
            if (playersInGame[1])
            {
                characterSelected[1] = false;
                playerSelectPos[1].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
                playerReadyCount -= 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[13] + "3"))
        {
            if (playersInGame[2])
            {
                characterSelected[2] = false;
                playerSelectPos[2].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
                playerReadyCount -= 1;
            }
        }
        if (Input.GetButtonDown(playerCtrlMap[13] + "4"))
        {
            if (playersInGame[3])
            {
                characterSelected[3] = false;
                playerSelectPos[3].GetComponentInChildren<UnityEngine.UI.Text>().color = playersSettings.UnselectedTextColor;
                playerReadyCount -= 1;
            }
        }
    }

    void LoadLevel()
    {
        if (playersSettings.TypeSelected == PrPlayerSettings.GameMode.DeathMatch)
        {
            int levelToLoad = 0;
            if (levelSettings.selectRandomLevel)
            {
                levelToLoad = Random.Range(0, levelSettings.availableMultiplayerLevels.Length - 1);
            }
            //Debug.Log(levelToLoad);
            levelSettings.actualLevel = levelToLoad;

            SceneManager.LoadScene( levelSettings.availableMultiplayerLevels[levelToLoad]);
        }
        else if (playersSettings.TypeSelected == PrPlayerSettings.GameMode.SinglePlayer)
        {
            int levelToLoad = 0;
            if (levelSettings.selectRandomLevel)
            {
                levelToLoad = Random.Range(0, levelSettings.availableSinglePlayerLevels.Length - 1);
            }
            //Debug.Log(levelToLoad);
            levelSettings.actualLevel = levelToLoad;

            SceneManager.LoadScene(levelSettings.availableSinglePlayerLevels[levelToLoad]);
        }

        else if (playersSettings.TypeSelected == PrPlayerSettings.GameMode.Cooperative)
        {
            int levelToLoad = 0;
            if (levelSettings.selectRandomLevel)
            {
                levelToLoad = Random.Range(0, levelSettings.availableCoopLevels.Length - 1);
            }
            //Debug.Log(levelToLoad);
            levelSettings.actualLevel = levelToLoad;

            SceneManager.LoadScene(levelSettings.availableCoopLevels[levelToLoad]);
        }

        else if (playersSettings.TypeSelected == PrPlayerSettings.GameMode.Survival)
        {
            int levelToLoad = 0;
            if (levelSettings.selectRandomLevel)
            {
                levelToLoad = Random.Range(0, levelSettings.availableSurvivalLevels.Length - 1);
            }
            //Debug.Log(levelToLoad);
            levelSettings.actualLevel = levelToLoad;

            SceneManager.LoadScene(levelSettings.availableSurvivalLevels[levelToLoad]);
        }

        else if (playersSettings.TypeSelected == PrPlayerSettings.GameMode.TowerDefense)
        {
            int levelToLoad = 0;
            if (levelSettings.selectRandomLevel)
            {
                levelToLoad = Random.Range(0, levelSettings.availableSurvivalLevels.Length - 1);
            }
            //Debug.Log(levelToLoad);
            levelSettings.actualLevel = levelToLoad;

            SceneManager.LoadScene(levelSettings.availableTowerDefenseLevels[levelToLoad]);
        }

    }

    void GetAllPlayersState()
    {
        if (playerCount > 0 && playerReadyCount == playerCount)
        {
            //Debug.Log("Game Ready. Loading selected level");
            if (levelSettings && levelSettings.availableMultiplayerLevels.Length > 0)
            {
                LoadLevel();
            }
        }
    }

    // Update is called once per frame
    void Update () {

        GetPlayerStart();
        GetChangePlayer();
        GetPlayerSelected();
        GetAllPlayersState();
        
    }

    void OnDrawGizmos()
    {
        if (playersSettings && targetArrow)
        {
            int n = 0;
            foreach (Transform spawnPos in playerSelectPos)
            {
                Gizmos.color = playersSettings.playerColor[n] * 2;
                Gizmos.DrawMesh(targetArrow, spawnPos.position + Vector3.up, Quaternion.Euler(0, 10, 0), Vector3.one);
                n += 1;
            }
        }

    }
}
