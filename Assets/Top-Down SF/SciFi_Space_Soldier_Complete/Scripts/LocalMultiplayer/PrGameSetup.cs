using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PrGameSetup : MonoBehaviour {

    public enum GameMode
    {
        SinglePlayer = 0,
        Coop = 1,
        DeathMatch = 2,
        TeamDeathMatch = 3,
        TowerDefense = 4,
        Survival = 5
    }

    [Header("Game Setup")]
    public GameMode mode = GameMode.Coop;
    public int actualPlayerCount = 4;
    public GameObject[] playersPrefabs;
    private GameObject[] actualPlayerPrefabs;
    private PrTopDownCharController[] playersControllers;
    private PrTopDownCharInventory[] playersInventorys;
    private GameObject[] playersForCamera;
    public Transform[] playersSpawnPos;
    public bool[] spawnPointFull;
    public PrPlayerSettings playersSettings;
    public PrLevelSettings levelSettings;
    public PrWeaponList weaponList;
    private string[] weaponNames;

    public enum GameStage
    {
        inGame,
        EndedMatch,
        levelComplete
    }

    

    [Header("In Game Stats")]
    public GameStage stage = GameStage.inGame;
    public int actualLevel = 0;

    //pickups
    private GameObject[] pickups;
    private Vector3[] playersDeathPos;
    private bool useLives = false;
    private int[] livesPerPlayer;
    private bool[] playerReachedEndZone;


    [Header("SinglePlayer and coop Setup")]

    public GameObject LevelCompleteUI;

    [Header("DeathMatch Setup")]
    public int fragsToWin = 10;
    public GameObject[] fragCounter;
    private int[] playersFrags;
    public GameObject playerWinsText;
    public PrPlayerSettings playerSettings;

    [Header("Team DeathMatch HUD")]
    public GameObject[] teamfragCounter;
    private int[] teamFrags;

    public enum SurvivalStage
    {
        Start,
        InWave,
        WaitingWave,
        EndedMatch,
    }
    [Header("Survival Setup")]
    public SurvivalStage survivalStage = SurvivalStage.Start;

    public PrSurvivalWavesSetup survivalSetup;
    public PrEnemySpawner[] enemySpawners;

    private int actualWave = -1;
    private int actualWaveHUD = 0;
    private int waveEnemiesSpawned = 0;
    private int waveEnemiesDeath = 0;
    private int totalEnemiesSpawned = 0;
    //timers
    public bool displayTimer = true;
    public GameObject HUDSurvivalTimer;
    public GameObject HUDSurvivalActualWave;

    private float survivalTimer = 0.0f;
    private string minString = "";
    private string secString = "";

    private float interWavesTimer = 0.0f;
    private float actualWaveTimer = 0.0f;
    private float waveSpawnTimer = 0.0f;
    private float timeBetweenSpawn = 0.0f;

    
    [Header("Camera Setup")]
    public bool useSplitScreen = false;
    public bool useSingleScreenCameraLimits = false;
    public GameObject multiplayerCam;
    public Vector2 targetHeightVariation = new Vector2(8, 50);
    public float targetHeightDistanceFactor = 1.0f;
    public float targetHeightCorrection = -3.0f;
    [HideInInspector]
    public PrTopDownMutiplayerCam actualCameraScript;

    [Header("Debug")]
    
    public Mesh areaMesh;
    public Mesh targetArrow;

    // Use this for initialization
    void Start () {

        if (playerSettings && playersSettings.selectedCharacters.Length > 0)
        {
            playersPrefabs = playersSettings.selectedCharacters;
            actualPlayerCount = playersSettings.finalPlayerCount;
        }

        if (mode == GameMode.TeamDeathMatch)
            actualPlayerCount = 4;

        //Set initialarrays
        playersControllers = new PrTopDownCharController[4];
        playersInventorys = new PrTopDownCharInventory[4];

        

        actualPlayerPrefabs = new GameObject[4];
        spawnPointFull = new bool[playersSpawnPos.Length];

        if (playersSettings)
        {
            if (weaponList)
            {
                weaponNames = new string[weaponList.weapons.Length];
                int index = 0;
                foreach (GameObject w in weaponList.weapons)
                {
                    weaponNames[index] = w.gameObject.GetComponent<PrWeapon>().WeaponName;
                    index++;
                }
            }
            pickups = GameObject.FindGameObjectsWithTag("Pickup");
            if (pickups.Length > 0)
            {
                foreach (GameObject pickup in pickups)
                {
                    if (pickup.GetComponent<PrPickupObject>())
                    {
                        pickup.GetComponent<PrPickupObject>().ColorSetup = playerSettings;
                        pickup.GetComponent<PrPickupObject>().weaponNames = weaponNames;
                        pickup.GetComponent<PrPickupObject>().Initialize();
                    }
                }
            }

            PrItemsSpawner[] itemSpawners = FindObjectsOfType<PrItemsSpawner>();
            if (itemSpawners.Length > 0)
            {
                foreach (PrItemsSpawner itemSpawner in itemSpawners)
                {
                    itemSpawner.settings = playerSettings;
                    itemSpawner.weaponNames = weaponNames;
                }
            }
        }

        //Initialize Endzones
        initializeEndzones();
        //Set Actual Level from levelsettings file
        actualLevel = levelSettings.actualLevel;
        //Start Game Setup
        StartGame();

    }
	
    void initializeEndzones()
    {
        //EndZone Initialization
        playerReachedEndZone = new bool[4];
        int indexReached = 0;
        foreach (bool reached in playerReachedEndZone)
        {
            playerReachedEndZone[indexReached] = false;
            indexReached = indexReached + 1;
        }
    }

    void ResetLives()
    {
        playersDeathPos = new Vector3[4];
        useLives = playerSettings.useLives;
        livesPerPlayer = new int[4];

        for (int x = 0; x < 4; x++)
        {
            playersDeathPos[x] = playersSpawnPos[x].position;
            if (playerSettings.livesPerPlayer >= 0)
            {
                livesPerPlayer[x] = playerSettings.livesPerPlayer;
            }
            else
            {
                livesPerPlayer[x] = 99999;
            }
        }
    }

    // Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.F5))
        {
            NewMultiplayerMatch(1);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            NewMultiplayerMatch(2);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            NewMultiplayerMatch(3);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            NewMultiplayerMatch(4);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (useSplitScreen)
                useSplitScreen = false;
            else
                useSplitScreen = true;

            NewMultiplayerMatch(actualPlayerCount);
        }

        if (mode == GameMode.SinglePlayer || mode == GameMode.Coop)
        {
            if (stage != GameStage.levelComplete)
            {
                if (Input.GetButtonDown("Player1Start") && actualPlayerCount >= 1)
                {
                    if (stage == GameStage.EndedMatch)
                    {
                        NewMultiplayerMatch(1);
                    }
                    else
                    {
                        if (playersInventorys[0].isDead == true)
                        {
                            DestroyPlayer(0);
                            if (useLives && livesPerPlayer[0] > 0)
                            {
                                SpawnPlayer(0, true);
                                PlayerSpawned();
                            }
                            else
                            {
                                Debug.Log("Player can´t respawn, you must restart the game using F5");
                            }
                        }
                    }

                }
            }
            else
            {
                if (Input.GetButtonDown("Player1Start") && actualPlayerCount >= 1)
                {
                    LevelDone();
                }
            }
           
        }

        if (mode == GameMode.DeathMatch || mode == GameMode.TeamDeathMatch)
        {
            if (Input.GetButtonDown("Player1Start") && playersSettings.playersInGame[0])
            {
                if (stage == GameStage.EndedMatch)
                {
                    NewMultiplayerMatch(actualPlayerCount);
                }
                else
                {
                    if (playersInventorys[0].isDead == true)
                    {
                        DestroyPlayer(0);
                        SpawnPlayer(0, true);
                        PlayerSpawned();
                    }
                }
                    
            }

            if (Input.GetButtonDown("Player2Start") && playersSettings.playersInGame[1])
            {
                if (stage == GameStage.EndedMatch)
                {
                    NewMultiplayerMatch(actualPlayerCount);
                }
                else
                {
                    if (playersInventorys[1].isDead == true)
                    {
                        DestroyPlayer(1);
                        SpawnPlayer(1, true);
                        PlayerSpawned();
                    }
                }

            }
            if (Input.GetButtonDown("Player3Start") && playersSettings.playersInGame[2])
            {
                if (stage == GameStage.EndedMatch)
                {
                    NewMultiplayerMatch(actualPlayerCount);
                }
                else
                {
                    if (playersInventorys[2].isDead == true)
                    {
                        DestroyPlayer(2);
                        SpawnPlayer(2, true);
                        PlayerSpawned();
                    }
                }
            }
            if (Input.GetButtonDown("Player4Start") && playersSettings.playersInGame[3])
            {
                if (stage == GameStage.EndedMatch)
                {
                    NewMultiplayerMatch(actualPlayerCount);
                }
                else
                {
                    if (playersInventorys[3].isDead == true)
                    {
                        DestroyPlayer(3);
                        SpawnPlayer(3, true);
                        PlayerSpawned();
                    }
                }
            }
        }
        
        if (spawnPointFull.Length > 0 && playersSpawnPos.Length > 0)
        {
            for (int i = 0; i < spawnPointFull.Length; i++)
            {
                spawnPointFull[i] = playersSpawnPos[i].GetComponent<PrSpawnPoint>().isFull;
            }
        }

        if (mode == GameMode.Survival)
        {
            UpdateSurvivalGame();
        }
    }

    void NewMultiplayerMatch(int playerCount)
    {
        for (int i = 0; i < 4; i++)
        {
            if (actualPlayerPrefabs[i] != null)
                DestroyPlayer(i);
            
            if (playersInventorys[i] != null)
                playersInventorys[i].DestroyHUD();

        }

        actualPlayerCount = playerCount;
        if (mode == GameMode.TeamDeathMatch)
            actualPlayerCount = 4;

        StartGame();
        
    }
    /*
    void HideSpawnPoints()
    {
        foreach (Transform spawn in playersSpawnPos)
        {
            spawn.gameObject.SetActive(false);
        }
    }*/

    void PlayerReachedEndZone(int playerNumber)
    {
        if (stage != GameStage.levelComplete)
        {
            if (actualPlayerPrefabs[playerNumber - 1] != null)
            {
                Debug.Log("player is here " + playerNumber);
                playerReachedEndZone[playerNumber - 1] = true;
            }
            int count = 0;
            foreach (bool reached in playerReachedEndZone)
            {
                if (reached)
                    count = count + 1;
            }
            if (count >= actualPlayerCount)
            {
                SetLevelComplete();
            }
        }
        
    }

    void SetLevelComplete()
    {
        if (LevelCompleteUI)
        {
            Instantiate(LevelCompleteUI, Vector3.zero, Quaternion.identity);
        }
        CallEnemiesToStop();
        stage = GameStage.levelComplete;
    }

    void CallEnemiesToStop()
    {
        PrEnemyAI[] Enemies = FindObjectsOfType(typeof(PrEnemyAI)) as PrEnemyAI[];
        if (Enemies.Length != 0)
        {
            foreach (PrEnemyAI enemy in Enemies)
            {
                enemy.StopAllActivities();
            }
        }
        PrEnemySpawner[] spawners = FindObjectsOfType(typeof(PrEnemySpawner)) as PrEnemySpawner[];
        foreach (var s in spawners)
        {
            s.SpawnerEnabled = false;
        }

    }

    void LevelDone()
    {
        
        Debug.Log("Level Complete - Loading Next Level");

        if (mode == GameMode.SinglePlayer)
        {
            foreach (PrTopDownCharController player in playersControllers)
            {
                if (player != null)
                    player.SavePlayerInfo();
            }
        }


        LoadNextLevel();
        
            
    }

    void LoadNextLevel()
    {
        int nextLevel = actualLevel + 1;
        levelSettings.actualLevel = nextLevel;

        if (mode == GameMode.SinglePlayer)
        {
            if (levelSettings.availableSinglePlayerLevels.Length > nextLevel)
                SceneManager.LoadScene(levelSettings.availableSinglePlayerLevels[nextLevel]);
        }
        else if (mode == GameMode.Coop)
        {
            SceneManager.LoadScene(levelSettings.availableCoopLevels[nextLevel]);
        }
        else if (mode == GameMode.DeathMatch)
        {
            SceneManager.LoadScene(levelSettings.availableMultiplayerLevels[nextLevel]);
        }
        else if (mode == GameMode.Survival)
        {
            SceneManager.LoadScene(levelSettings.availableSurvivalLevels[nextLevel]);
        }
        else if (mode == GameMode.TowerDefense)
        {
            SceneManager.LoadScene(levelSettings.availableTowerDefenseLevels[nextLevel]);
        }


    }

    void StartGame()
    {
        ResetLives();

        playersFrags = new int[4];
        for (int i = 0; i < 4; i++)
        {
            playersFrags[i] = 0;
        }

        teamFrags = new int[2];
        for (int i = 0; i < 2; i++)
        {
            teamFrags[i] = 0;
        }

        stage = GameStage.inGame;

        if (playersPrefabs.Length >= actualPlayerCount && playersSpawnPos.Length >= actualPlayerCount)
        {
           /* for (int x = 0; x < actualPlayerCount; x++)
            {
                SpawnPlayer(x, false);
            }*/
            for (int x = 0; x < 4; x++)
            {
                if (playersSettings.playersInGame[x])
                    SpawnPlayer(x, false);
            }

        }

        if (mode == GameMode.DeathMatch || mode == GameMode.TeamDeathMatch || mode == GameMode.Coop || mode == GameMode.Survival)
        {
            if (playerWinsText)
                playerWinsText.SetActive(false);

            CreateCamera();//Create the Camera

        }

        //HUD Reset
        if (mode == GameMode.DeathMatch || mode == GameMode.TeamDeathMatch )
        {
            ResetFragHUD();
            UpdateFragHUD();
            OrganizeFragHUD();

        }
        else if (mode == GameMode.Survival)
        {
            StartSurvivalMode();
        }

        Debug.Log(actualPlayerPrefabs[0]);
    }

    void UpdateLastPlayerPos(int thePlayer)
    {
        playersDeathPos[thePlayer] = playersControllers[thePlayer].transform.position;
    }

    void StartSurvivalMode()
    {
        if (survivalSetup != null && enemySpawners.Length >= 1)
        {
            survivalStage = SurvivalStage.Start;
            foreach (PrEnemySpawner spawner in enemySpawners)
            {
                spawner.transform.parent = this.transform;
                spawner.SearchPlayerAfterSpawn = true;
                spawner.SpawnerEnabled = false;
                spawner.SpawnStartDelay = 0.0f;
            }
                
        }

    }

    void UpdateSurvivalTimer()
    {
        minString = Mathf.Floor(survivalTimer / 60).ToString("00");
        secString = Mathf.Floor(survivalTimer % 60).ToString("00");
    
    }

    void UpdateSurvivalGame()
    {
        if (stage == GameStage.inGame)
        {
            survivalTimer += Time.deltaTime;

            UpdateSurvivalTimer();
            
            HUDSurvivalTimer.GetComponent<Text>().text = minString + ":" + secString;
            if (survivalStage == SurvivalStage.Start)
            {
                HUDSurvivalTimer.GetComponent<Text>().text = "";
                HUDSurvivalActualWave.GetComponent<Text>().text = "";
                if (survivalTimer >= survivalSetup.initialTimer)
                    SetSurvivalWave();
            }
            
            if (survivalStage == SurvivalStage.InWave)
            {
                
                HUDSurvivalActualWave.GetComponent<Text>().text = "Wave " + (actualWaveHUD).ToString();
                actualWaveTimer += Time.deltaTime;
                waveSpawnTimer += Time.deltaTime;
                if (waveEnemiesSpawned < survivalSetup.waves[actualWave].enemiesCount)
                {
                    if (waveSpawnTimer >= timeBetweenSpawn)
                    {
                        SurvivalSpawnEnemy();
                    }
                }
                   
            }
            if (survivalStage == SurvivalStage.WaitingWave)
            {
                HUDSurvivalActualWave.GetComponent<Text>().text = "";
                interWavesTimer += Time.deltaTime;
                if (interWavesTimer >= survivalSetup.timeBetweenWaves)
                {
                    SetSurvivalWave();
                }
            }
        }

    }

    void SurvivalSpawnEnemy()
    {
        waveSpawnTimer = 0.0f;
        int actualSpawner = Random.Range(0, enemySpawners.Length);
        int enemyToSpawn = Random.Range(0, survivalSetup.waves[actualWave].Enemies.Length);

        enemySpawners[actualSpawner].SurvivalSpawnEnemy(survivalSetup.waves[actualWave].Enemies[enemyToSpawn]);

        waveEnemiesSpawned += 1;
        totalEnemiesSpawned += 1;
    }

    void InterWaveSetup()
    {
        survivalStage = SurvivalStage.WaitingWave;
        interWavesTimer = 0.0f;

    }

    void EnemyDead()
    {
        waveEnemiesDeath += 1;
        if (waveEnemiesDeath == survivalSetup.waves[actualWave].enemiesCount)
        {
            InterWaveSetup();
        }
    }

    void SetSurvivalWave()
    {
        SetSurvivalStage(SurvivalStage.InWave);
        actualWave += 1;
        actualWaveHUD += 1;

        //Debug.Log(actualWave + " " + survivalSetup.waves.Length);
        if (actualWave >= survivalSetup.waves.Length)
        {
            actualWave -= survivalSetup.repeatLastWaves;
        }

        actualWaveTimer = 0.0f;
        waveEnemiesSpawned = 0;
        waveEnemiesDeath = 0;
        timeBetweenSpawn = Random.Range(survivalSetup.waves[actualWave].timeBetweenSpawn[0], survivalSetup.waves[actualWave].timeBetweenSpawn[1]);
        //Debug.Log("Time Between Spawn" + timeBetweenSpawn);

        /*
        int enemiesPerSpawner = survivalSetup.waves[actualWave].enemiesCount / enemySpawners.Length;
        foreach(PrEnemySpawner spawner in enemySpawners)
        {
            spawner.SpawnerEnabled = true;
            spawner.SpawnStartDelay = 0.0f;
        }*/
    }

    void SetSurvivalStage(SurvivalStage sStage)
    {
        survivalStage = sStage;
    }


    void SetPlayersForCamera()
    {
        int playerCount = 0;

        for (int i = 0; i < 4; i++)
        {
            if (!playersSettings.playersInGame[i])
            {
                //DoNothing
            }
            else
            {
                if (playersInventorys[i].isDead)
                {
                    //DoNothing
                }
                else
                {
                    playerCount += 1;
                }
                
            }
        }

        if (playerCount > 0)
        {
            playersForCamera = new GameObject[playerCount];

            int finalCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!playersSettings.playersInGame[i])
                {
                    //DoNothing
                }
                else
                {
                    if (playersInventorys[i].isDead)
                    {
                        //DoNothing
                    }
                    else
                    {
                        playersForCamera[finalCount] = playersInventorys[i].gameObject;
                        finalCount += 1;
                    }
                }
               
            }
        }
        
    }

    void CreateCamera()
    {
        if (useSplitScreen)
        {
            //waits until players are created
        }
        else
        {
            if (!actualCameraScript)
            {

                GameObject actualCameraGO = Instantiate(multiplayerCam, GetCameraCenter(), Quaternion.Euler(0, 45, 0)) as GameObject;
                actualCameraGO.transform.parent = this.transform;
                actualCameraGO.name = "MutiplayerCamera";
                actualCameraScript = actualCameraGO.GetComponent<PrTopDownMutiplayerCam>();
            }

            SetPlayersForCamera();
            actualCameraScript.MultiplayerCam(playersForCamera, actualPlayerCount);
            actualCameraScript.ResetWalls();
            actualCameraScript.targetHeightVariation = targetHeightVariation;
            actualCameraScript.targetHeightDistanceFactor = targetHeightDistanceFactor;
            actualCameraScript.targetHeightCorrection = targetHeightCorrection;
            actualCameraScript.useCameraColisions = useSingleScreenCameraLimits;


        }
        
    }

    Vector3 GetCameraCenter()
    {
        int a = 0;
        Vector3 cameraCenter = Vector3.zero;
        //if (actualPlayerCount == 1)
       // {
        foreach (bool x in playersSettings.playersInGame)
        {
            if (x)
            {
                cameraCenter += actualPlayerPrefabs[a].transform.position;
            }
            a += 1;
        }
            
       /* }
        else if (actualPlayerCount == 2)
        {
            // cameraCenter = (actualPlayerPrefabs[0].transform.position + actualPlayerPrefabs[1].transform.position) / actualPlayerCount;
            foreach (bool x in playersSettings.playersInGame)
            {
                if (x)
                {
                    cameraCenter = actualPlayerPrefabs[a].transform.position;
                }
                a += 1;
            }
            
        }
        else if (actualPlayerCount == 3)
        {
            cameraCenter = (actualPlayerPrefabs[0].transform.position + actualPlayerPrefabs[1].transform.position + actualPlayerPrefabs[2].transform.position) / actualPlayerCount;
        }
        else if (actualPlayerCount == 4)
        {
            cameraCenter = (actualPlayerPrefabs[0].transform.position + actualPlayerPrefabs[1].transform.position + actualPlayerPrefabs[2].transform.position + actualPlayerPrefabs[3].transform.position) / actualPlayerCount;
        }*/
    
        return cameraCenter / actualPlayerCount; 
    }

    void DestroyPlayer(int playerNumber)
    {
        Destroy(actualPlayerPrefabs[playerNumber]);
    }

    int RandomNum(int lastRandNum)
    {
        int randNum = Random.Range(0, playersSpawnPos.Length);
        
        return randNum;
    }
    
    void SpawnPlayer(int playerNumber, bool randomPos)
    {
        if (playersSettings.playersInGame[playerNumber])
        {
            int posInt = playerNumber;

            if (randomPos)
            {
                posInt = RandomNum(posInt);
                int tries = 0;
                while (spawnPointFull[posInt] == true && tries < 12)
                {
                    posInt = RandomNum(posInt);
                    tries += 1;
                }

            }
            //set last position if using lives
            Vector3 finalSpawnPos = playersSpawnPos[posInt].position;
            if (useLives && playersDeathPos[playerNumber] != playersSpawnPos[playerNumber].position)
            {
                Debug.Log("aaaa" + playersDeathPos[playerNumber]);
                finalSpawnPos = playersDeathPos[playerNumber];
            }

            Debug.Log(finalSpawnPos);
            //Instantiate player Prefab in Scene
            GameObject tempPlayer = Instantiate(playersPrefabs[playerNumber], finalSpawnPos, playersSpawnPos[posInt].rotation) as GameObject;
            tempPlayer.transform.parent = this.transform;
            actualPlayerPrefabs[playerNumber] = tempPlayer;
            playersControllers[playerNumber] = tempPlayer.transform.GetComponentInChildren<PrTopDownCharController>();
            playersInventorys[playerNumber] = tempPlayer.transform.GetComponentInChildren<PrTopDownCharInventory>();

            playersControllers[playerNumber].playerNmb = playerNumber + 1;
            //set split screen var
            playersInventorys[playerNumber].SetSplitScreen(useSplitScreen, actualPlayerCount);

            //Sets player Team settings
            if (mode == GameMode.DeathMatch)
            {
                playersInventorys[playerNumber].team = playerNumber;
                playersControllers[playerNumber].JoystickEnabled = true;
            }
            else if (mode == GameMode.TeamDeathMatch)
            {
                if (playerNumber < 2)
                {
                    playersInventorys[playerNumber].team = 1;
                }
                else
                {
                    playersInventorys[playerNumber].team = 2;
                }
                playersControllers[playerNumber].JoystickEnabled = true;

            }
            //Set player Colors
            if (mode == GameMode.SinglePlayer)
            {
                playersInventorys[playerNumber].SetPlayerColors(0, playerNumber, playerSettings);
            }
            else if (mode == GameMode.DeathMatch)
            {
                playersInventorys[playerNumber].SetPlayerColors(1, playerNumber, playerSettings);
            }
            else if (mode == GameMode.Coop || mode == GameMode.Survival)
            {
                playersInventorys[playerNumber].SetPlayerColors(2, playerNumber, playerSettings);
            }
            else if (mode == GameMode.TeamDeathMatch)
            {
                playersInventorys[playerNumber].SetPlayerColors(3, playersInventorys[playerNumber].team - 1, playerSettings);
            }

            if (mode != GameMode.SinglePlayer)
            {
                if (useSplitScreen)
                {
                    //Debug.Log("Split Screen Active");
                    //Get Player Camera
                    playersControllers[playerNumber].CamScript.gameObject.SetActive(true);
                    Camera tempCam = playersControllers[playerNumber].CamScript.transform.GetComponentInChildren<Camera>();

                    if (actualPlayerCount == 1)
                    {
                        //DoNothing
                    }
                    else if (actualPlayerCount == 2)
                    {
                        if (playerNumber == 0)
                            SetCamSplitScreen(tempCam, 0, 0, 0.5f, 1);
                        else if (playerNumber == 1)
                            SetCamSplitScreen(tempCam, 0.5f, 0, 0.5f, 1);
                    }
                    else if (actualPlayerCount == 3)
                    {
                        if (playerNumber == 0)
                            SetCamSplitScreen(tempCam, 0.25f, 0.5f, 0.5f, 0.5f);
                        else if (playerNumber == 1)
                            SetCamSplitScreen(tempCam, 0.0f, 0, 0.5f, 0.5f);
                        else if (playerNumber == 2)
                            SetCamSplitScreen(tempCam, 0.5f, 0, 0.5f, 0.5f);
                    }
                    else if (actualPlayerCount == 4)
                    {
                        if (playerNumber == 0)
                            SetCamSplitScreen(tempCam, 0.0f, 0.5f, 0.5f, 0.5f);
                        else if (playerNumber == 1)
                            SetCamSplitScreen(tempCam, 0.5f, 0.5f, 0.5f, 0.5f);
                        else if (playerNumber == 2)
                            SetCamSplitScreen(tempCam, 0.0f, 0, 0.5f, 0.5f);
                        else if (playerNumber == 3)
                            SetCamSplitScreen(tempCam, 0.5f, 0, 0.5f, 0.5f);

                    }
                    //SetCamSplitScreen()
                    // playersControllers[playerNumber].CamScript.transform.GetComponentInChildren<Camera>().rect.
                }
                else
                {
                    playersControllers[playerNumber].CamScript.gameObject.SetActive(false);
                    playersInventorys[playerNumber].HUDDamageFullScreen.SetActive(false);

                }
            }
        }
        
        

    }

    void SetCamSplitScreen(Camera cam, float x, float y, float width, float height)
    {
        cam.rect = new Rect(x, y, width, height);
    }

    void OnDrawGizmos()
    {
        if (playersSettings && targetArrow)
        {
            int n = 0;
            foreach (Transform spawnPos in playersSpawnPos)
            {
                Gizmos.color = playersSettings.playerColor[n] * 2;
                Gizmos.DrawMesh(targetArrow, spawnPos.position + Vector3.up, Quaternion.Euler(0, 10, 0), Vector3.one);
                n += 1;

               // Gizmos.color = Color.white;
                Gizmos.DrawMesh(areaMesh, spawnPos.position, Quaternion.Euler(0, 0, 0), Vector3.one);

            }
        }
   
    }

    void ResetFragHUD()
    {
        
        int i = 1;

        foreach (GameObject text in fragCounter)
        {
            if (playersSettings.playersInGame[i-1])
            {
                playersFrags[i - 1] = 0;
                text.GetComponent<Text>().text = "P" + i.ToString() + " " + playersFrags[i - 1].ToString();
            }
           
        }

        i = 1;

        if (mode == GameMode.TeamDeathMatch)
        {
            foreach (GameObject text in teamfragCounter)
            {
                teamFrags[i - 1] = 0;
                text.GetComponent<Text>().text = teamFrags[i - 1].ToString();
            }
        }
        
    }

    void OrganizeFragHUD()
    {
        if (mode == GameMode.DeathMatch)
        {
            fragCounter[0].transform.parent.gameObject.SetActive(true);
            teamfragCounter[0].transform.parent.gameObject.SetActive(false);
            int i = 1;
            foreach (GameObject text in fragCounter)
            {
                if (actualPlayerCount == 1)
                {
                    Debug.Log("Un Solo player");
                    text.GetComponent<Text>().text = "";
                }
                else
                {
                    if (playersSettings.playersInGame[i-1])
                    {
                        text.GetComponent<Text>().color = playerSettings.playerColor[i - 1];
                        text.GetComponent<Text>().text = "P" + i.ToString() + " " + playersFrags[i - 1].ToString() + "0";
                    }
                    else
                    {
                        text.GetComponent<Text>().text = "";
                    }
                    
                }

                i += 1;
            }
        }
        else if (mode == GameMode.TeamDeathMatch)
        {
            fragCounter[0].transform.parent.gameObject.SetActive(false);
            teamfragCounter[0].transform.parent.gameObject.SetActive(true);
            
        }


    }

    void UpdateFragHUD()
    {
        if (mode == GameMode.DeathMatch)
        {
            int i = 1;
            foreach (GameObject text in fragCounter)
            {
                if (actualPlayerCount >= i)
                {
                    text.GetComponent<Text>().text = "P" + i.ToString() + " " + playersFrags[i - 1].ToString("00");

                    i += 1;
                }

            }
        }
        else if (mode == GameMode.TeamDeathMatch)
        {
            teamfragCounter[0].GetComponent<Text>().text = teamFrags[0].ToString("00");
            teamfragCounter[1].GetComponent<Text>().text = teamFrags[1].ToString("00");

        }
    }

    public void NewFrag(int team)
    {
        if (stage == GameStage.inGame)
        {
            if (mode == GameMode.DeathMatch )
            {
                if (team < 0)
                {
                    team = team * -1;
                    team -= 1;
                    playersFrags[team] -= 1;

                    Debug.Log("EnemyTeam Game" + team);
                }
                else
                {
                    playersFrags[team] += 1;
                }

                UpdateFragHUD();

                if (playersFrags[team] >= fragsToWin)
                {
                    SetPlayerWin(team);
                }


            }
            else if (mode == GameMode.TeamDeathMatch)
            {
                teamFrags[team - 1] += 1;
                Debug.Log("Frag By " + (team - 1));

                UpdateFragHUD();

                if (teamFrags[team - 1] >= fragsToWin)
                {
                    SetTeamWin(team - 1);
                }
            }
        }

    }

    public void PlayerSpawned()
    {
        SetPlayersForCamera();
        if (actualCameraScript)
            actualCameraScript.MultiplayerCam(playersForCamera, actualPlayerCount);
    }

    public void PlayerDied(int playerNumber)
    {
        if (mode == GameMode.DeathMatch || mode == GameMode.TeamDeathMatch || mode == GameMode.Coop || mode == GameMode.Survival)
        {
            SetPlayersForCamera();
            if (!useSplitScreen)
            {
                actualCameraScript.MultiplayerCam(playersForCamera, actualPlayerCount);

            }
        }

        UpdateLastPlayerPos(playerNumber-1);

        if (mode == GameMode.SinglePlayer || mode == GameMode.SinglePlayer)
        {
            if (useLives && livesPerPlayer[playerNumber] > 0 && playerSettings.livesPerPlayer >= 0)
            {
                livesPerPlayer[playerNumber-1] -= 1;
            }
        }

        //Do whatever you need
    }

    public void SetTeamWin(int teamToWin)
    {
        Debug.Log("Wining Team " + teamToWin);
        stage = GameStage.EndedMatch;

        playerWinsText.SetActive(true);

        playerWinsText.GetComponent<Text>().color = playerSettings.teamColor[teamToWin];

        string finalText = "";

        if (teamToWin == 0)
        {
            finalText = "RED TEAM WINS";
        }
        else
        {
            finalText = "BLUE TEAM WINS";
        }
        
        playerWinsText.GetComponent<Text>().text = finalText;
    }

    public void SetPlayerWin(int playerToWin)
    {
        stage = GameStage.EndedMatch;

        playerWinsText.SetActive(true);

        playerWinsText.GetComponent<Text>().color = playerSettings.playerColor[playerToWin];

        int finalPlayer = playerToWin + 1;
        string finalText = "Player " + finalPlayer.ToString() + " Wins";

        playerWinsText.GetComponent<Text>().text = finalText;
    }


}
