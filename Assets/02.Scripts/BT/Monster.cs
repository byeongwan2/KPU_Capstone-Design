using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Animator animator;

    private readonly int hashAttack = Animator.StringToHash("isAttack");

    Monster_BT monster_BT = new Monster_BT();

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        monster_BT.Init();
    }

    // Update is called once per frame
    void Update()
    {
        monster_BT.Run();        
    }   
    

    public void Attack()
    {
        animator.SetBool(hashAttack, true);
    }
}
