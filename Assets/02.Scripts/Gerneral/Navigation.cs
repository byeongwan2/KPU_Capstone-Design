using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct BlockPoint
{
    public float node_x;
    public float node_y;
}

class BlockNode
{
    BlockPoint point;
    BlockNode[] Node;

    public BlockNode(float _x, float _y)
    {
        point.node_x = _x;
        point.node_y = _y;
        Node = new BlockNode[8];
        
    }
}

class FieldNodeSetting
{
    void Init()
    {
        for (int i = 0; i < 30; i++)
        {
            BlockNode node = new BlockNode(i * 50.0f , 50.0f);

        }
    }
    
}







public class Navigation : MonoBehaviour {

    GameObject trace_obj;
    void Start()
    {
        trace_obj = GameObject.Find("Player2");

    }
    List<GameObject> nodeList;

    void Divide_Node()
    {

    }

    void Select_Now_Point()
    {

    }
}
