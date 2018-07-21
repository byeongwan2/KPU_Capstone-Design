using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {
    public Player player;
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


}
