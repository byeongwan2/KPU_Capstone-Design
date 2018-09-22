using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {
    private GameSystem system;
    bool isRun = false;     //작동하는지 달리기아님
    void Start()
    {
        isRun = false;
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
	void OnTriggerStay(Collider _col)
    {
        if (isRun) return;
        if (_col.gameObject.tag.Equals("Player"))
        {
            if (system.pPlayer2.IsJumpHit() == true) return;
            isRun = true;
            system.pPlayer2.MinusHp(10);
            system.pPlayer2.WoundEffect();
            StartCoroutine(CoolTime());
        }
    }

   IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(1.5f);
        isRun = false;
    }
}
