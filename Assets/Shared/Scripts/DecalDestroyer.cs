using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour {

	public float lifeTime = 4.0f;

	private void Start()
	{
        StartCoroutine(Active_Life());
	}

    IEnumerator Active_Life()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine(Active_Life());
    }
}
