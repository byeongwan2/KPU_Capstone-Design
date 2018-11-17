using UnityEngine;
using System.Collections;

public class PrItemsSpawner : MonoBehaviour {

    [Header("Basic Settings")]
    public GameObject[] items;
    public Transform[] spawnPositions;
    private GameObject spawnedItem;

    public bool spawnOnce = false;

    public float timeAfterRespawn = 10.0f;
    private float timer = 0.0f;

    [Header("Visual Settings")]
    public GameObject baseMesh;
    public GameObject spawnVFX;
    private GameObject[] actualSpawnVFX;

    // Use this for initialization
    void Start()
    {
        timer = timeAfterRespawn;

        if (spawnOnce)
            SpawnItem();

        if (baseMesh)
        {
            if (spawnVFX)
                actualSpawnVFX = new GameObject[spawnPositions.Length];

            for (int i = 0; i < spawnPositions.Length; i++)
            {
                GameObject tempMesh = Instantiate(baseMesh, spawnPositions[i].position, spawnPositions[i].rotation) as GameObject;
                tempMesh.transform.parent = spawnPositions[i];
                if (spawnVFX)
                {

                    actualSpawnVFX[i] = Instantiate(spawnVFX, spawnPositions[i].position + new Vector3(0, 1, 0), spawnPositions[i].rotation) as GameObject;
                    actualSpawnVFX[i].transform.parent = spawnPositions[i];
                    actualSpawnVFX[i].SetActive(false);
                }
            }
        }

        //Preload
        for (int i = 0; i < items.Length; i++)
        {
            GameObject tempI = Instantiate(items[i], transform.position, transform.rotation) as GameObject;
            tempI.SetActive(false);
            tempI.AddComponent<PrDestroyTimer>();
        }
    }	
	// Update is called once per frame
	void Update () {
        if (!spawnOnce && spawnedItem == null)
        {
            if (timer >= 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                SpawnItem();
            }
        }
	    
	}

    void PlayVFX(int i)
    {
        actualSpawnVFX[i].SetActive(true);
        actualSpawnVFX[i].GetComponent<ParticleSystem>().Play();
    }

    void SpawnItem()
    {
        // Instantiate
        int randomPos = Random.Range(0, spawnPositions.Length);

        if (spawnPositions.Length == 0)
        {
            spawnedItem = Instantiate(items[Random.Range(0, items.Length )], transform.position, transform.rotation) as GameObject;
        }
        else
        {
            spawnedItem = Instantiate(items[Random.Range(0, items.Length )], spawnPositions[randomPos].position, spawnPositions[randomPos].rotation) as GameObject;
        }

        if (spawnVFX)
            PlayVFX(randomPos);

        spawnedItem.transform.parent = this.transform;
        timer = timeAfterRespawn;
    }

    void OnDrawGizmos()
    {
       
        if (spawnPositions.Length > 0)
        {
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(spawnPositions[i].position + Vector3.up, Vector3.one);

            }
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position + Vector3.up, Vector3.one);
        }

    }
}
