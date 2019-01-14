using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Behaviour
{
    GameObject particleGroup ;      //배열로
    public void Init(string _link)
    {
        particleGroup = GameObject.Find(_link);
    }

    public override void Work(TYPE _type)
    {
        
        Debug.Log("파티클작동");
    }

}
