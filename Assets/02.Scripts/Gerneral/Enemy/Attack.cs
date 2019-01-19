using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private int attackDamage = 10;
    public void Init(int _attackDamage)
    {
        attackDamage = _attackDamage;
    }
    
    public bool Work()
    {
        Debug.Log("pppp");
        return true;
    }
}
