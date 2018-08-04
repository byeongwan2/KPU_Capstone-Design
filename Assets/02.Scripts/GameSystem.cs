using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {
    private Player player;
    public Player Player  {
        get
        {
            return player;
        }
        set
        {
            player = value;
        }
    }

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }


}
