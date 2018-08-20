using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MoveObject {

    GameSystem system;
    protected Animator enemyAni;

    protected void Init()        //하위에서 호출해야함       //그리고 웬만하면 모든 멤버변수를 여기서 초기화해야함
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        enemyAni = GetComponent<Animator>();
    }

    
}
