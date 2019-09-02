using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    public Boss boss;
    int shotCount = 0;      //한번 공격하면 3번공격하기
    public void Exit_Shot_Anim()
    {
        shotCount++;
        if (shotCount == 3)
        {
            shotCount = 0;
            boss.weaponAnimator.SetTrigger("ShotFinish");
            boss.OffOther_State_Change();
            Debug.Log("발사중지");
        }
        else
        {
            boss.weaponAnimator.SetTrigger("Shot");
        }
    }

    public void Exit_Shot_Work()
    {
        boss.WorkShot();
        Debug.Log("발사!!");
    }
}
