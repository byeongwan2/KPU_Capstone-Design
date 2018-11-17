using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurvivalWave", menuName = "SurvivalMode/SurvivalWave", order = 1)]
public class PrSurvivalWave : ScriptableObject
{
    public GameObject[] Enemies;
    public int enemiesCount = 10;
    public Vector2 timeBetweenSpawn = new Vector2(0.0f, 3.0f);
}

