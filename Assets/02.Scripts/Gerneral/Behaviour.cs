using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour {
    protected PrefabSystem prefabSystem;

    protected Transform this_transForm;
    [SerializeField]
    protected GameObject startPosition;
    protected void Init()
    {
        this_transForm = GetComponent<Transform>();
        prefabSystem = GameObject.Find("GameSystem").GetComponent<PrefabSystem>();
    }

    public abstract void Work();
}
