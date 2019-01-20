using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    Node root;

    public BehaviorTree(Node node)
    {
        root = node;
    }

    public bool Run()
    {
        Debug.Log("vvvv");
        if (root.Run()) return true;
        Debug.Log("vvvvwqwe");
        return false;
        
    }
}
