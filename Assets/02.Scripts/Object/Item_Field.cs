using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Field : MonoBehaviour {
    GameSystem system;
    private bool isGain = false;
	void Start () {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

        StartCoroutine(MovePosition());
        isGain = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isGain == false) return;

        Vector3 vec = system.Player.transform.position - transform.position;
        Quaternion q = Quaternion.LookRotation(vec);
        Quaternion s = Quaternion.Slerp(transform.rotation, q, 5.0f * Time.deltaTime);
        transform.rotation = s;
        transform.Translate(new Vector3(0, 0, 1) * 10.0f * Time.deltaTime);





    }

    private IEnumerator MovePosition()
    {
        var dis = Check.Distance(system.Player.transform, this.transform);

        if (dis < 10.0f)
        {
            isGain = true;
            yield return null;
        }
        else isGain = false;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MovePosition());
    }
}
