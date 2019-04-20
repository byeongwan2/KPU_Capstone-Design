using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerSettings/PlayerSetup", order = 1)]
public class PrPlayerSettings : ScriptableObject
{
    [Header("Player Names")]
    public string[] playerName = { "Player 1", "Player 2", "Player 3", "Player 4" };

    [Header("Player Control Mapping")]
    public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Reload",
        "EquipWeapon", "Sprint", "Aim", "ChangeWTrigger", "Roll", "Use", "Crouch", "ChangeWeapon", "Throw"  ,"Fire", "Mouse ScrollWheel"};

    [Header("Single Player && coop basic setup")]
    [Tooltip("UseLives boolean defines if the PLAYER will be able to CONTINUE playing from the last position or he will need to RE START the level to continue playing.")]
    public bool useLives = false;
    public int livesPerPlayer = 3;

    [Header("Character Selection Settings")]
    public Color UnselectedTextColor = new Color(0.2f, 0.2f, 0.8f, 1);
    public Color SelectedTextColor = new Color(0.3f, 0.8f, 0.3f, 1);

    public enum GameMode
    {
        SinglePlayer,
        Cooperative,
        DeathMatch,
        Survival,
        TowerDefense
    }

    [Header("Key Settings")]
    public GameMode TypeSelected;

    public GameObject[] availableCharacters;
    //[HideInInspector]
    public GameObject[] selectedCharacters;
    //[HideInInspector]
    public bool[] playersInGame;
    public int finalPlayerCount = 0;
    public int GameType = 0;

    [Header("Player Colors")]
    public bool useSinglePlayerColor = false;
    public Color singlePlayerColor;

    public bool useCoopPlayerColor = false;
    public Color[] coopPlayerColor;

    public Color[] playerColor;

    public Color[] teamColor;

    [Header("Pick-Ups Colors")]
    public bool AlwaysShowPickups = false;
    public Color ActivePickupColor = new Color(0.2f, 1, 0.2f, 1);
    public Color InactivePickupColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
    public bool showPickupText = false;
    public Color PickupTextColor = new Color(0.2f, 1, 0.2f, 1);
} 
