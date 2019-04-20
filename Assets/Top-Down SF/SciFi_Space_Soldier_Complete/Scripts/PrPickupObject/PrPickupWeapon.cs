using UnityEngine;
using System.Collections;

public class PrPickupWeapon : PrPickupObject {
   
    public enum Weapons
    {
        Pistol = 0, 
        Rifle = 1, 
        Shotgun = 2,
        RocketLauncher = 3,
        Melee = 4,
        Laser = 5,
        FireThrower = 6,
        IceThrower = 7
    }
    [Header("Type Weapon Settings")]
    public Weapons WeaponType;
  
 	// Update is called once per frame
	void Update () {
	
	}

    protected override void SetName()
    {
        //Debug.Log(weaponNames[(int)WeaponType]);
        if (weaponNames.Length > 0)
            itemName = weaponNames[(int)WeaponType];
    }
    protected override void PickupObjectNow(int ActiveWeapon)
    {

        if (Player != null)
        {
            PrTopDownCharInventory PlayerInv = Player.GetComponent<PrTopDownCharInventory>();

            PlayerInv.PickupWeapon((int)WeaponType);

        }

        base.PickupObjectNow(ActiveWeapon);
    }
   
   
}
