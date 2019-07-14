using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Object_Id
{
    NONE,PLAYER,MONSTER
}
//움직이는놈들은 모두 이 클래스 상속
public class MoveObject : MonoBehaviour
{
    public int vitality = 5;   // 체력

    //자신이 플레이어인지 적군인지 코드로 보관
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

   
}
