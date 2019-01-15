using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Attack_Action : Node
{
    private Monster monster = new Monster();

    public override bool Run()
    {
        monster.Attack();
        return true;
    }
}



public class Monster_BT : BT_build
{    
    
    private Sequence root = new Sequence();
    private Selector behaviour = new Selector();
    private Sequence Dead = new Sequence();
    private Attack_Action Attack = new Attack_Action();

    
    
    public override void Init()
    {
        
        root.AddChild(behaviour);
        behaviour.AddChild(Attack);
        
    }

    public void Run()
    {
        root.Run();
    }

}

