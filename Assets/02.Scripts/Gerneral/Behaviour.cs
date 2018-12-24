using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour {

    [SerializeField]
    protected GameObject startPosition;


    public abstract void Work(TYPE _type);
}
