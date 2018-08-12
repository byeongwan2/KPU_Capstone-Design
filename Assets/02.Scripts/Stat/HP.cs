using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour {

    private int hp;

    public void SettingHp(int _initHp)      //초기화
    {
        hp = _initHp;
    }

    public void MinusHp(int _minusHp) { hp -= _minusHp; }        //마이너스 hp함수와 플러스 hp는 사실상 똑같은 함수 가독성을위함
    public void PlusHp(int _plusHp) { hp += _plusHp; }

    public int getHp()
    {
        return hp;
    }
}
