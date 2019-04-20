using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerSettings/LevelsSetup", order = 1)]
public class PrLevelSettings : ScriptableObject
{
    public bool selectRandomLevel;
    public int actualLevel;

    [Header("Available singlePlayer Levels")]
    public string[] availableSinglePlayerLevels;

    [Header("Available cooperative Levels")]
    public string[] availableCoopLevels;

    [Header("Available Multiplayer Levels")]
    public string[] availableMultiplayerLevels;

    [Header("Available Survival Levels")]
    public string[] availableSurvivalLevels;

    [Header("Available Survival Levels")]
    public string[] availableTowerDefenseLevels;
} 
