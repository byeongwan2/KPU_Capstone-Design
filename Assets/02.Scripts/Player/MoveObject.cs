using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    [SerializeField]
    protected int hp ;

    public void SetHp(int _hp)
    {
        hp += _hp;
    }
}
