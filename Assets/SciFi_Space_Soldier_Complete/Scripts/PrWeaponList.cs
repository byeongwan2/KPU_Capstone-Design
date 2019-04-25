using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "WeaponData/WeaponList", order = 1)]
public class PrWeaponList : ScriptableObject
{
    public string objectName = "Weapon List";
    public GameObject[] weapons;
}