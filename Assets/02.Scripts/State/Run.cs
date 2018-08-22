using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : State
{

    public override void Work()
    {

    }
    public override void PlayAnimation(Animator _animator)
    {
        _animator.SetBool("IsRun", true);
        _animator.SetBool("IsWalk", false);
    }
}
