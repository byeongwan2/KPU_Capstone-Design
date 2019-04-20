using UnityEngine;
using System.Collections;

public class PrPickupGrenades : PrPickupObject {

    /*public enum Ammo
    {
        Explosive = 1,
        //Incendiary = 2, 
        //Ice = 3, 
    }*/
    [Header("Type Ammo Settings")]
    //public Ammo LoadType = Ammo.Explosive;
    public int quantity = 1;

    // Update is called once per frame
    void Update () {
	
	}

    protected override void SetName()
    {
        itemName = "Grenades x" + quantity;
    }

    protected override void PickupObjectNow(int ActiveWeapon)
    {

        if (Player != null)
        {
            PrTopDownCharInventory PlayerInv = Player.GetComponent<PrTopDownCharInventory>();

            PlayerInv.LoadGrenades(quantity);
            //PlayerInv.Weapon[ActiveWeapon].SendMessage("LoadAmmo", (int)LoadType);

        }

        base.PickupObjectNow(ActiveWeapon);
    }
   
   
}
