using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {
    private GameSystem system;
    bool isRun = false;
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
            if (system.pPlayer2.IsJump() == true) return;
            isRun = true;
            system.pPlayer2.MinusHp(10);
            StartCoroutine(CoolTime());
        }
    }

   IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(2.0f);
        isRun = false;
    }
}
