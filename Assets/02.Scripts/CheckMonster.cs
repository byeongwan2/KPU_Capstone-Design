using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 마우스로 적을 감별하여 외곽선 셰이더를 구현하는 스크립트

public class CheckMonster : MonoBehaviour
{
    [SerializeField]    // 디버깅을 위해서
    public GameObject target;
    private ChangeShader cs;
    
    void Start()
    {
        cs = null;
    }

    // Update is called once per frame
    void Update()
    {
        target = GetObject();
        if (target == null)
        {
            return;
        }

        // 적을 감지하면
        if(target.CompareTag("Enemy"))
        {
            // 적의 ChangeMaterial 컴포넌트를 가지고 온다
            cs = target.GetComponentInChildren<ChangeShader>();
            
            // 외곽선 셰이더 설정
            cs.SetOutLine();
        }
        else
        {
            if(cs != null)
            {
                cs.SetOrigin();
                cs = null;
            }
        }
    }

    private GameObject GetObject()
    {
        RaycastHit hit;
        GameObject target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // 마우스 포인터 좌표 생성

        if((Physics.Raycast(ray.origin, ray.direction * 10, out hit)) == true)
        {
            target = hit.collider.gameObject;
            return target;
        }
        else
        {
            return null;
        }
    }
}
