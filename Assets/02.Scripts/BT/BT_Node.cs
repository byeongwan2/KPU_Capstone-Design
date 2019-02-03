using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum RESULT { SUCCESS, FAIL, RUNNING }
public abstract class Node
{
    public virtual RESULT Run()
    {
        return RESULT.SUCCESS;
    }
}

public class CompositeNode : Node
{
    public override RESULT Run()     
    {
        return RESULT.SUCCESS;
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
    public override RESULT Run()
    {
        foreach (var node in GetChildrens())
        {
            switch (node.Run())
            {
                case RESULT.SUCCESS:
                    return RESULT.SUCCESS;
                //case RESULT.RUNNING:
                //    return RESULT.RUNNING;
            }
            
        }
        return RESULT.FAIL;
    }
}

public class Sequence : CompositeNode
{
    public override RESULT Run()
    {
        foreach (var node in GetChildrens())
        {
            switch(node.Run())
            {
                case RESULT.FAIL:
                    return RESULT.FAIL;
               // case RESULT.RUNNING:
               //     return RESULT.RUNNING;
            }
            
        }
        if (GetChildrens().Count == 0) return RESULT.FAIL;
        return RESULT.SUCCESS;
    }
}

public class Leaf_Node: Node 
{
    private Func<RESULT> m_func;
    public Leaf_Node(Func<RESULT> _func)
    {
        m_func = _func;
    }
    
    public override RESULT Run()
    {
        Debug.Log(Define.DEBUG_STRING + m_func.Method);
        switch(m_func())
        {
            case RESULT.SUCCESS:
                return RESULT.SUCCESS;
            case RESULT.FAIL:
                return RESULT.FAIL;
            case RESULT.RUNNING:
                return RESULT.RUNNING;
        }
        return RESULT.FAIL;
    }
}

public class Leaf_Node_Float : Node
{
    private Func<float, RESULT> m_func;
    float data;
    public Leaf_Node_Float(Func<float, RESULT> _func,float _data)
    {
        m_func = _func;
        data = _data;
    }

    public override RESULT Run()
    {
        Debug.Log(data);
        Debug.Log(Define.DEBUG_STRING + m_func.Method);
        switch (m_func(data))
        {
            case RESULT.SUCCESS:
                return RESULT.SUCCESS;
            case RESULT.FAIL:
                return RESULT.FAIL;
            case RESULT.RUNNING:
                return RESULT.RUNNING;
        }
        return RESULT.FAIL;
    }
}


