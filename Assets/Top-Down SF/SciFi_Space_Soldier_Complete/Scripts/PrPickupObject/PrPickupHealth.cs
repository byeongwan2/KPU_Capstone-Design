using UnityEngine;
using System.Collections;

public class PrPickupHealth : PrPickupObject {
   
     
 	// Update is called once per frame
	void Update () {
	
	}

    protected override void PickupObjectNow(int ActiveWeapon)
    {

        if (Player != null)
        {
            PrTopDownCharInventory PlayerInv = Player.GetComponent<PrTopDownCharInventory>();

            PlayerInv.SetHealth(PlayerInv.Health);

        }

        base.PickupObjectNow(ActiveWeapon);
    }
   
   
}
