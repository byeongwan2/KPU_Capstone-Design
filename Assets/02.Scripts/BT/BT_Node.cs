using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

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
                case RESULT.RUNNING:
                    return RESULT.RUNNING;
            }
            
        }
        return RESULT.FAIL;
    }
}

public class Sequence : CompositeNode
{
    Leaf_Node behaviour;
    public Sequence()
    {
        behaviour = new Leaf_Node();
    }
    public Sequence(Func<RESULT> _func)
    {
        behaviour = new Leaf_Node(_func);
    }
    public override RESULT Run()
    {
        RESULT result = RESULT.FAIL;
        foreach (var node in GetChildrens())
        {
            switch(node.Run())
            {
                case RESULT.FAIL:
                    result = RESULT.FAIL;
                    break;
                case RESULT.RUNNING:
                    result = RESULT.RUNNING;
                    break;
                case RESULT.SUCCESS:
                    result = RESULT.SUCCESS;
                    break;
            }
            if (result == RESULT.SUCCESS) continue;                             //차일드는 일단 계속 돌아야함
            if (result== RESULT.FAIL || result == RESULT.RUNNING) break;        //하위에서 실패하면 자신의 행동만 실행하고 리턴
        }
        if (result == RESULT.SUCCESS) return RESULT.SUCCESS;            //시퀸스는 차일드가 우선순위가 높으므로 성공한자식이있다면 그냥리턴
        if (behaviour.Get_m_func() == null)  return result;                             //시퀸스가 아무 행동도 안보유하면 그냥 리턴
        
        return behaviour.Run();
    }
}

public class Leaf_Node: Node 
{
    private Func<RESULT> m_func;
    public Func<RESULT>  Get_m_func() { return m_func; }
    public Action m_end;
    public Leaf_Node()
    {
        m_func = null;
    }
    public Leaf_Node(Func<RESULT> _func)
    {
        m_func = _func;
    }
    public Leaf_Node(Func<RESULT> _func,Action _end)
    {
        m_func = _func;
        m_end = _end;
    }

    public override RESULT Run()
    {
        //Debug.Log(Define.DEBUG_STRING + m_func.Method);
        switch(m_func())
        {
            case RESULT.SUCCESS:

                return RESULT.SUCCESS;
            case RESULT.FAIL:
                return RESULT.FAIL;
            case RESULT.RUNNING:
                return RESULT.RUNNING;
            default:
                return RESULT.FAIL;
        }
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
        //Debug.Log(Define.DEBUG_STRING + m_func.Method);
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


