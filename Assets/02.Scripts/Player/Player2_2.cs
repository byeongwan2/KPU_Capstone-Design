using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//내가 직접 호출안하는 함수는 On 같은걸로시작하는 유니티내장을 쓰지만 직접만든거는 복잡하니까 전부 여기로 옮기기
//여기에서만 쓰이는게 아니라면 변수는 1번클래스에서 선언해야함
public partial class Player2
{
    //변수선언금지
    private void Event_ThrowBomb()
    {
        bombThrow.Work(TYPE.BOMB);
    }

    private void Event_MouseExit()
    {
        isMouse = false;
        isAttackStop = false;
    }
    private void Event_ReloadExit()
    {
        isReload = false;
    }

    private void Event_RollingExit()
    {
        eState = STATE.WALK;
        isMouse = false;
        isRoll = false;
        isKey = false;
    }
    private void Event_JumpingExit()
    {
        isMouse = false;
        eState = ePreState;
        if (isJumpDelay == true) eState = STATE.STAND;     //버그방지
        isJumpDelay = false;
    }

    private void Event_HitJumping()
    {
        isJumpHit = false;
    }

    private void Event_AttackExit()
    {
        attackCoolTime = false;
    }
}
