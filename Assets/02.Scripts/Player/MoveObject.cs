using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    [SerializeField]
    public HP hp ;                      //움직이는건 다 hp가 있을꺼라고 생각 따로 컴포넌트로 해도되지만 그러면 public어차피쓰는거

    protected void Setting()
    {
        hp = new HP();
    }
}
