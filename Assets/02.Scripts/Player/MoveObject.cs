using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    [SerializeField]
    protected HP hp ;

    public HP GetHp()
    {
        return hp;
    }

    protected void Setting()
    {
        hp = new HP();
    }
}
