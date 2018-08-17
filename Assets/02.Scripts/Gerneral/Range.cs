using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour {

    GameSystem system;          //갖가지 변수를 다 갖고올수 있음  성능은 쥐약이지만 한번만 불러오면되기때문에 좋음

	void Start () {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();

        StartCoroutine(CheckPlayerDistance());
    }
	
	IEnumerator CheckPlayerDistance()
    {
        yield return new WaitForSeconds(1.0f);

    }
}
