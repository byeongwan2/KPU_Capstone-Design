using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MoveObject        //애니메이션 이벤트로만 발동되는 콜백함수모음 클래스   //내가호출하고싶어도 안하는함수모음
{           

    //착지후 다시움직일수있음
    private void LandingDoubleJumpExit()         //착지가 끝나면 자동으로 호출 
    {
        eSpecialState = SPECIAL_STATE.NONE;
        isKeyNone = false;
        isJumpNone = false;
    }


    //단순 총알발사
    private void ShotBullet()
    {
        bulletShot.Work();
    }

    //단순 폭탄던지기
    private void ThrowBomb()
    {
        bombThrow.Work();
    }
}
