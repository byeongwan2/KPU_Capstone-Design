using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class TempDebug : MonoBehaviour {
    Player2 player;
    Text playerState;
    void Start()
    {
        player = GameObject.Find("Player2").GetComponent<Player2>();
        playerState = GetComponent<Text>();
    }

    void Update()
    {
        playerState.text = player.TempStateReturn();
    }
}
