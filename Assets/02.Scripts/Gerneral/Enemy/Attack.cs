using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    MoveObject target;
    private int attackDamage = 10;
    public void Init_Target(MoveObject _target)
    {
        target = _target;
    }
    public void Init(int _attackDamage)
    {
        attackDamage = _attackDamage;
    }
    
    public bool Work()
    {
        float rotateDegree = Mathf.Atan2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, rotateDegree, 0.0f), Time.deltaTime * 10.0f);
        return true;
    }
}
