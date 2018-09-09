using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acquire : MonoBehaviour {

    //아이템습득
    void OnTriggerEnter(Collider _obj)
    {
        if (_obj.CompareTag("Item"))
        {
            _obj.gameObject.SetActive(false);       // 필드에서 아이템흡수
        }

    }
}
