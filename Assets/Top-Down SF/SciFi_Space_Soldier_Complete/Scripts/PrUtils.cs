using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrUtils{

    public static string[] SetMultiplayerInputs(int playerNmb, PrPlayerSettings playerSettings, string[] playerCtrlMap)
    {

        if (playerNmb > 1)
        {
            int values = 0;
            foreach (string ctrl in playerSettings.playerCtrlMap)
            {
                playerCtrlMap[values] = ctrl + playerNmb.ToString();
                values += 1;
            }
        }
        else
        {
            playerCtrlMap = playerSettings.playerCtrlMap;
        }

        return(playerCtrlMap);
    }
}
