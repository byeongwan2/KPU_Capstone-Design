using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public abstract class Node
{
    public abstract bool Invoke();
}

public class CompositeNode : Node
{
    public override bool Invoke()
    {
        throw new NotImplementedException();
    }

    public void AddChild(Node node)
    {
        childrens.Push(node);
    }

    public Stack<Node> GetChildrens()
    {
        return childrens;
    }
    private Stack<Node> childrens = new Stack<Node>();
}

public class Selector : CompositeNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke())
            {
                return true;
            }
        }
        return false;

    }
}

public class Sequence : CompositeNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (!node.Invoke())
            {
                return false;
            }
        }
        return true;
    }
}

public class ActionA : Node
{
    public bool running = true;

    public override bool Invoke()
    {
        if (running)
        {
            Debug.Log("ActionA True!");
            running = false;
            return true;
        }
        else
        {
            Debug.Log("ActionA False!");
            running = true;
            return false;
        }
    }
}


public abstract class BT_base : MonoBehaviour
{
    public abstract void Init();        
}
