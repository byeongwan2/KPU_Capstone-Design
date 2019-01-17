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
                return false;
            }
        }
        return true;
    }
}


public class Leaf_Node : Node
{    
    Func<bool> NodeFunc;
    public Leaf_Node(Func<bool> func)
    {
        NodeFunc = func;
    }
    public override bool Run()
    {        
        if(NodeFunc())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


