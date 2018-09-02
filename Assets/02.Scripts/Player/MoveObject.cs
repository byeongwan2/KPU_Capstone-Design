using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    protected STATE eState = STATE.STAND;

    [SerializeField]
    protected int hp;
    public void Init(int _initHp)      //초기화
    {
        hp = _initHp;
    }

    public void MinusHp(int _minusHp)
    {
        hp -= _minusHp;
        Debug.Log(hp);
        if (hp <= 0)
        {
            Death();
        }

    }
    public void PlusHp(int _plusHp)
    {
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
}
