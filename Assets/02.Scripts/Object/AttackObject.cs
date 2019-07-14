using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//총알 폭탄은 이 클래스를 상속받음
public abstract class AttackObject : MonoBehaviour {

    protected Vector3 launchPos;
    protected Quaternion launchRot;

    //총알이나 폭탄이 발사될때 발사시작위치를 셋팅 발사할때마다 호출
    public void SetLaunchPos(Vector3 _launchPos)
    {
        launchPos = _launchPos;
    }

    public void SetLaunchRot(Quaternion _launchRot)
    {
        launchRot = _launchRot;
    }
}
