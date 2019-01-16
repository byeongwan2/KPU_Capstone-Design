using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Alien : MonoBehaviour
{    
    Animator animator;
    BehaviorTree bt;
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    
    private void Awake()
    {
        animator = GetComponent<Animator>();                
    }

    // Start is called before the first frame update
    void Start()
    {
        Build_BT();
        bt.Run();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Attack()   
    {
        animator.SetBool(hashAttack, true);
    }

    void Build_BT() // 행동트리 생성
    {
        Sequence root = new Sequence();
        Selector behaviour = new Selector();
        Leaf_Node Attack_Node = new Leaf_Node(Attack);

        root.AddChild(behaviour);
        behaviour.AddChild(Attack_Node);

        bt = new BehaviorTree(root);    // 트리가 완성되면 Alien 행동트리 멤버변수에 적용
    }
    
}
