using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Field : MonoBehaviour
{
    GameSystem system;
    private bool isGain = false;
    void Start()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

        StartCoroutine(MovePosition());
        isGain = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGain == false) return;

        Vector3 vec = system.pPlayer.transform.position - transform.position;            // 정확히 복붙한 5줄 수정좀해야함
        Quaternion q = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 3.0f * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, 0.6f) * 8.0f * Time.deltaTime);

     
    }

    private IEnumerator MovePosition()
    {
        var dis = Check.Distance(system.pPlayer.transform, this.transform);

        if (dis < 7.0f)
        {
            isGain = true;
            yield break;
        }
        else isGain = false;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MovePosition());
    }
  
  

    
   
}