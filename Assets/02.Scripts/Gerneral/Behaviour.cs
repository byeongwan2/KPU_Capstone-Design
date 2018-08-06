using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour {

    protected Transform this_transForm;

    protected PrefabSystem prefabSystem;

    protected void Init()
    {
        this_transForm = GetComponent<Transform>();
        prefabSystem = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
        Debug.Log(prefabSystem);
    }

    public abstract void Work();
}
