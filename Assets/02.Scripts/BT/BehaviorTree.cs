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
        if (root.Run()) return true;
        return false;
        
    }
}
