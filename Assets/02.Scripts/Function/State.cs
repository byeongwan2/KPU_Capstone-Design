using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE { STAND, JUMP, WALK, RUN , ATTACK,HIT,ROLL,DIE, DASH}
enum WAY { FORWARD, RIGHT, LEFT}


public abstract class State               //상태에 따른 클래스를 두고 필요할때마다 갱신해서 쓰는 클래스 패턴
{
    public abstract void Work();
    public abstract void PlayAnimation(Animator _animator);
    public void Init()
    {
        
    }
}
