using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurvivalWavesSetup", menuName = "SurvivalMode/SurvivalWavesSetup", order = 1)]
public class PrSurvivalWavesSetup : ScriptableObject
{
    [Header("Waves")]
    public float initialTimer = 5.0f;
    public PrSurvivalWave[] waves;
    public int repeatLastWaves = 1;
    public float timeBetweenWaves = 5.0f;

    [Header("Items")]
    public bool spawnItems = true;
    public GameObject[] items;

}
