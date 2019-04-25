using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerSettings/PlayerSetup", order = 1)]
public class PrPlayerSettings : ScriptableObject
{

    public string[] playerName = { "Player 1", "Player 2", "Player 3", "Player 4" };

    public string[] playerCtrlMap = {"Horizontal", "Vertical", "LookX", "LookY","FireTrigger", "Reload",
        "EquipWeapon", "Sprint", "Aim", "ChangeWTrigger", "Roll", "Use", "Crouch", "ChangeWeapon", "Throw"  ,"Fire", "Mouse ScrollWheel"};

    public bool useSinglePlayerColor = false;
    public Color singlePlayerColor;

    public bool useCoopPlayerColor = false;
    public Color[] coopPlayerColor;

    public Color[] playerColor;

    public Color[] teamColor;
} 
