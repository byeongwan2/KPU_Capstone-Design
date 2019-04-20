using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Object_Id
{
    NONE,PLAYER,MONSTER
}
public class MoveObject : MonoBehaviour
{
    protected Object_Id m_id;
   
    public Object_Id Get_Id()
    {
        return m_id;
    }

    public bool Compare_This(Object_Id _id)         //자기자신이 아니라면 false 리턴
    {
        if (m_id == _id) return true;
        else return false;
    }

    protected void Gravity()
    {
        
    }
}
