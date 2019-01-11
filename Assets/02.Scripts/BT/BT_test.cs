using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_build : BT_base
{
    private Sequence root = new Sequence();
    private Selector selector = new Selector();
    private readonly ActionA actionA = new ActionA();
        

    public override void Init()
    {
        root.AddChild(selector);

        selector.AddChild(actionA);        
        
    }
   
    public void Run()
    {
        root.Invoke();
    }
    
}

public class BT_test : MonoBehaviour
{
    BT_build bT_Build = new BT_build();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bT_Build.Run();
    }

    
}
