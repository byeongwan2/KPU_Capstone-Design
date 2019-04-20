using UnityEngine;
using System.Collections;

public class PrPickupAmmo : PrPickupObject {

    public enum Ammo
    {
        Small = 4,
        Medium = 2,
        Full = 1
    }
    [Header("Type Ammo Settings")]
    public Ammo LoadType = Ammo.Full;

    // Update is called once per frame
    void Update () {
	
	}

    protected override void SetName()
    {
        itemName = LoadType.ToString() + " Ammo";

    }

    protected override void PickupObjectNow(int ActiveWeapon)
    {

        if (Player != null)
        {
            PrTopDownCharInventory PlayerInv = Player.GetComponent<PrTopDownCharInventory>();

            if (PlayerInv.Weapon[ActiveWeapon].GetComponent<PrWeapon>() != null)
            {
                PlayerInv.Weapon[ActiveWeapon].GetComponent<PrWeapon>().LoadAmmo((int)LoadType);
            }
            

        }

        base.PickupObjectNow(ActiveWeapon);
    }
   
   
}
