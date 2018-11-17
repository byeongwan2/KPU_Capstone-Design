using UnityEngine;
using System.Collections;

public class PrEnemySpawner : MonoBehaviour {

    [Header("Enemy Settings")]
    public GameObject Enemy;
	private GameObject EnemyParent;
    public PrWaypointsRoute EnemyPatrolRoute;
    public bool startInRandomWaypoint = false;
    public bool SearchPlayerAfterSpawn = false;

    public bool towerDefenseAI = false;
    public Transform towerDefenseTarget;

    [Header("Spawner Settings")]
	public float SpawnerRadius = 1.0f;
    public bool spawnInCircle = false;
	public int SpawnPerCycle = 1;
    public int waveCount = 1;
    private int actualWaveCount = 0;
	public int MaxCount = 0;
	public bool SpawnerEnabled = true;
	public float SpawnRate = 1.0f;
    public float SpawnRateAcceleration = 0.0f;

    public float SpawnStartDelay = 0.0f;
    [HideInInspector] public float SpawnTimer = 0.0f;
	private int TotalSpawned = 0;
    private int TotalSpawnedDead = 0;

    [Header("Display & Debug Settings")]
    public Mesh AreaMesh;
    public Mesh IconMesh;


    // Use this for initialization
    void Start () {
		EnemyParent = new GameObject();
		EnemyParent.name = gameObject.name + "-" + Enemy.name + "_ROOT";
		EnemyParent.transform.position = transform.position;
		EnemyParent.transform.rotation = transform.rotation;
        EnemyParent.transform.SetParent(this.transform);
        
	}
	
	// Update is called once per frame
	void Update () {

		if (TotalSpawned < MaxCount && SpawnerEnabled)
		{
            if (SpawnStartDelay <= 0.0f)
            {
                SpawnTimer += Time.deltaTime;
                if (SpawnTimer >= SpawnRate)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                SpawnStartDelay -= Time.deltaTime;
            }
			
		}
        
	}

    void EnemyDead()
    {
        TotalSpawnedDead += 1;
        actualWaveCount -= 1;
        if (TotalSpawnedDead == MaxCount)
        {
            SpawnerEnabled = false;
            SendMessageUpwards("SpawnerCompleted", SendMessageOptions.DontRequireReceiver);
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y ;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public void SurivvalSpawnEnemy(GameObject enemyToSpawn)
    {
        
        float RandomRadius = Random.Range(-SpawnerRadius, SpawnerRadius);
        float RandomRadius2 = Random.Range(-SpawnerRadius, SpawnerRadius);

        Vector3 FinalSpawnPosition = transform.position + new Vector3(RandomRadius, 0.1f, RandomRadius2);
        Quaternion rot = transform.rotation;

        if (spawnInCircle)
        {
            FinalSpawnPosition = RandomCircle(transform.position, SpawnerRadius);
            rot = Quaternion.FromToRotation(Vector3.forward, transform.position - FinalSpawnPosition);
        }

        GameObject EnemySpawned = Instantiate(enemyToSpawn, FinalSpawnPosition, rot) as GameObject;
        EnemySpawned.name = enemyToSpawn.name + "_" + TotalSpawned;

        if (EnemyPatrolRoute)
        {
            EnemySpawned.GetComponent<PrEnemyAI>().waypointRoute = EnemyPatrolRoute;
        }

        if (SearchPlayerAfterSpawn)
        {
            EnemySpawned.GetComponent<PrEnemyAI>().FindPlayers();
            EnemySpawned.GetComponent<PrEnemyAI>().lookForPlayer = true;
        }

        if (towerDefenseAI)
        {
            EnemySpawned.GetComponent<PrEnemyAI>().towerDefenseAI = true;
            if (towerDefenseTarget)
                EnemySpawned.GetComponent<PrEnemyAI>().towerDefenseTarget = towerDefenseTarget;
        }

        EnemySpawned.GetComponent<PrEnemyAI>().SetWaypoints();

        if (EnemyPatrolRoute && startInRandomWaypoint)
        {
            int max = EnemySpawned.GetComponent<PrEnemyAI>().waypoints.Length - 1;
            int rndm = Random.Range(0, max);
            FinalSpawnPosition = EnemySpawned.GetComponent<PrEnemyAI>().waypoints[rndm].position;
        }

        EnemySpawned.transform.parent = EnemyParent.transform;
        EnemySpawned.transform.position = FinalSpawnPosition;

        GameObject[] AIs = GameObject.FindGameObjectsWithTag("AIPlayer");
        foreach (GameObject AI in AIs)
        {
            AI.SendMessage("FindPlayers", SendMessageOptions.DontRequireReceiver);
        }

    }

    void SpawnEnemy()
	{
		if (Enemy != null && actualWaveCount < waveCount)
		{
            for (int i = 0; i < SpawnPerCycle; i++)
            {
                float RandomRadius = Random.Range(-SpawnerRadius, SpawnerRadius);
                float RandomRadius2 = Random.Range(-SpawnerRadius, SpawnerRadius);

                Vector3 FinalSpawnPosition = transform.position + new Vector3(RandomRadius, 0.1f, RandomRadius2);
                Quaternion rot = transform.rotation;

                if (spawnInCircle)
                {
                    FinalSpawnPosition = RandomCircle(transform.position, SpawnerRadius);
                    rot = Quaternion.FromToRotation(Vector3.forward, transform.position - FinalSpawnPosition);
                }

                GameObject EnemySpawned = Instantiate(Enemy, Vector3.zero , rot) as GameObject;
                EnemySpawned.name = Enemy.name + "_" + TotalSpawned;

                if (EnemyPatrolRoute)
                {
                    EnemySpawned.GetComponent<PrEnemyAI>().waypointRoute = EnemyPatrolRoute;
                       
                }

                if (SearchPlayerAfterSpawn)
                {
                    //EnemySpawned.GetComponent<PrEnemyAI>().player = GameObject.Find("Player");
                    //EnemySpawned.GetComponent<PrEnemyAI>().playerTransform = EnemySpawned.GetComponent<PrEnemyAI>().player.transform;
                    EnemySpawned.GetComponent<PrEnemyAI>().FindPlayers();
                    EnemySpawned.GetComponent<PrEnemyAI>().lookForPlayer = true;
                }
                    
                if (towerDefenseAI)
                {
                    EnemySpawned.GetComponent<PrEnemyAI>().towerDefenseAI = true;
                    if (towerDefenseTarget)
                        EnemySpawned.GetComponent<PrEnemyAI>().towerDefenseTarget = towerDefenseTarget;
                }

                EnemySpawned.GetComponent<PrEnemyAI>().SetWaypoints();

                if (EnemyPatrolRoute && startInRandomWaypoint)
                {
                    int max = EnemySpawned.GetComponent<PrEnemyAI>().waypoints.Length - 1;
                    int rndm = Random.Range(0, max);
                    FinalSpawnPosition = EnemySpawned.GetComponent<PrEnemyAI>().waypoints[rndm].position;
                }

                EnemySpawned.transform.position = FinalSpawnPosition;
                
                EnemySpawned.transform.parent = EnemyParent.transform;
                //Debug.Log(EnemySpawned.name);
                TotalSpawned += 1;
                actualWaveCount += 1;
            }    
        }

        
        SpawnRate -= SpawnRateAcceleration;
        SpawnTimer = 0.0f;
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow * 3f;
		Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * SpawnerRadius);
        Gizmos.color = Color.red * 1.5f;
        Gizmos.DrawMesh(IconMesh, transform.position + new Vector3(0,0.75f,0), Quaternion.identity, Vector3.one );
    }
}
