using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum RESULT { SUCCESS, FAIL, RUNNING }
public abstract class Node
{
    protected const string debug_String = "실행중  ";
    public virtual bool Run()
    {
        return true;
    }
}

public class CompositeNode : Node
{
    public override bool Run()     
    {
        return true;
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
                //Debug.Log(this.GetType());
                return false;
            }
        }
        return true;
    }
}

public class Leaf_Node: Node 
{
    private Func<bool> m_func;
    public Leaf_Node(Func< bool> _func)
    {
        m_func = _func;
    }
    
    public override bool Run()
    {
        Debug.Log(debug_String + m_func.Method);
        if (m_func())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class Leaf_Node_Float : Node
{
    private Func<float,bool> m_func;
    float data;
    public Leaf_Node_Float(Func<float,bool> _func,float _data)
    {
        m_func = _func;
        data = _data;
    }

    public override bool Run()
    {
        Debug.Log(debug_String + m_func.Method);
        if (m_func(data))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


