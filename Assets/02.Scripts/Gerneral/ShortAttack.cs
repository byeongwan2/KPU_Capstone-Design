using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortAttack : MonoBehaviour
{
    private GameSystem system; 

    void Awake()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }

   
}
