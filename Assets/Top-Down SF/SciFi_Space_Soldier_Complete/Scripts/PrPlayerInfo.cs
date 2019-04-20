using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrPlayerInfo : MonoBehaviour {

    public static PrPlayerInfo player1;

    [Header("Player Controller Variables")]
    public int playerNumber = 0;
    public bool usingJoystick = false;

    [Header("Player Inventory Variables")]
    public string playerName = "none";
    public int health = 100;
    public int actualHealth = 100;
    public int maxWeaponCount = 2;
    public int[] weapons;
    public int[] weaponsAmmo;
    public int[] weaponsClips;

    public GameObject[] grenadeType;
    public int grenades;

    void Awake()
    {
        if (player1 == null)
        {
            DontDestroyOnLoad(gameObject);
            player1 = this;
        }
        else if (player1 != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
