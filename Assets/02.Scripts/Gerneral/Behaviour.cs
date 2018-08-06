using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour {

    protected Transform this_transForm;

    protected PrefabSystem system;

    protected void Init()
    {
        this_transForm = GetComponent<Transform>();
        system = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
    }

    public abstract void Work();
}
