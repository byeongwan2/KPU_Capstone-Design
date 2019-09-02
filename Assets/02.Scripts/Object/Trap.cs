using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//현재는 안쓰고 있음  장애물밟으면 체력소모
public class Trap : MonoBehaviour {
    bool isRun = false;     //작동하는지 달리기아님
    void Start()
    {
        isRun = false;
    }
	void OnTriggerStay(Collider _col)
    {
        if (isRun) return;
        if (_col.gameObject.tag.Equals("Player"))
        {
            if (PrefabSystem.instance.player.IsJumpHit() == true) return;
            isRun = true;
            // system.pPlayer2.MinusHp(10);
            PrefabSystem.instance.player.Wound_Effect();
            StartCoroutine(CoolTime());
        }
    }

   IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(1.5f);
        isRun = false;
    }
}
