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

    public RESULT Run()
    {
        switch(root.Run())
        {
            case RESULT.SUCCESS:
                return RESULT.SUCCESS;
            case RESULT.RUNNING:
                return RESULT.RUNNING;
            case RESULT.FAIL:
                return RESULT.FAIL;
        }
        return RESULT.FAIL;
    }
}
