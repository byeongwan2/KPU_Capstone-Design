using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour {

    protected Transform this_transForm;
    [SerializeField]
    protected GameObject startPosition;
    protected void Init()
    {
        this_transForm = GetComponent<Transform>();
    }

    public abstract void Work();
}
