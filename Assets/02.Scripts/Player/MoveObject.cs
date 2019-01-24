using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Object_Id
{
    NONE,PLAYER,MONSTER
}
public class MoveObject : MonoBehaviour
{
    protected STATE eState = STATE.STAND;
    Object_Id m_id;
    public Object_Id Get_Id()
    {
        return m_id;
    }
    [SerializeField]
    protected int hp;
    public void Init(int _initHp)      //초기화
    {
        hp = _initHp;
    }

    public void MinusHp(int _minusHp)
    {
        hp -= _minusHp;
        Debug.Log(this+" "  + hp);
        if (hp <= 0)
        {
            Death();
        }

    }
    public void PlusHp(int _plusHp)
    {
        Debug.Log("플레이어 체력 " + hp);
        hp += _plusHp;
    }

    public int getHp()
    {
        return hp;
    }

    public void Death()
    {
        eState = STATE.DIE;
    }

    public bool Compare_This(Object_Id _id)
    {
        if (m_id == _id) return true;
        else return false;
    }
}
