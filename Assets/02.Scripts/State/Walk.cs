using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : State
{
    public override void Work()
    {

    }

    public override void PlayAnimation(Animator _animator)
    {
        _animator.SetBool("IsWalk", true);
        _animator.SetBool("IsRun", false);
    }
}
