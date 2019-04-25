using UnityEngine;
using System.Collections;

public class PrLightAnimator : MonoBehaviour {

	public float Factor = 0.0f;
	public float initialFactor = 2.0f;
	public float EndFactor = 0.0f;
	public float Timer = 1.0f;
	public bool Animate = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Animate)
		{
			Factor -= Timer * Time.deltaTime;
			GetComponent<Light>().intensity = Factor;
			if (Factor <= EndFactor)
			{
				Animate = false;
			}
			//if (Mathf.Approximately( EndFactor, Factor))
			//	
		}
	}

	public void AnimateLight(bool AnimateLight)
	{
		Factor = initialFactor;
		Animate = AnimateLight;

	}

}
