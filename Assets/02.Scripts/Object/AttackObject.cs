using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackObject : MonoBehaviour {

    protected Vector3 launchPos;
    protected Quaternion launchRot;
    public abstract void StatSetting();

    public void SetLaunchPos(Vector3 _launchPos)
    {
        launchPos = _launchPos;
    }

    public void SetLaunchRot(Quaternion _launchRot)
    {
        launchRot = _launchRot;
    }
}
