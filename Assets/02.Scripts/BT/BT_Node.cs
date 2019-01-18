using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class Node
{
    public virtual bool Run()
    {
        throw new NotImplementedException();
    }
}

public class CompositeNode : Node
{
    public override bool Run()     
    {
        throw new NotImplementedException();
    }

    public void AddChild(Node node) 
    {
        childrens.Enqueue(node);
    }

    public Queue<Node> GetChildrens()  
    {
        return childrens;
    }
    private Queue<Node> childrens = new Queue<Node>();  
}

public class Selector : CompositeNode
{
    public override bool Run()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Run())
            {
                Debug.Log("Selector");
                return true;
            }
        }
        return false;

    }
}

public class Sequence : CompositeNode
{
    public override bool Run()
    {        
        foreach (var node in GetChildrens())
        {            
            if (!node.Run())
            {
                Debug.Log("Sequence");
                return false;
            }
        }
        return true;
    }
}


public class Leaf_Node : Node
{
    public delegate bool func_delegate();
    func_delegate m_func;
    public Leaf_Node(func_delegate _func)
    {
        m_func = _func;
        Debug.Log(m_func);
    }
    public override bool Run()
    {
        if (m_func())
        {
            Debug.Log("vv");
            return true;
        }
        else
        {
            Debug.Log("yy");
            return false;
        }
    }
}


