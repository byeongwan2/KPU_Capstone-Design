using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrTitleID : MonoBehaviour {

    public int menuID = 0;

    public AnimationCurve scaleCurve;
    public float animationSpeed = 1.0f;

    private bool animate = false;
    private float timer = 0.0f;
    private Vector3 initialScale;

	// Use this for initialization
	void Start () {

        initialScale = transform.localScale;

    }
	
    public void StartAnim()
    {
        animate = true;
        timer = 0.0f;
    }

	// Update is called once per frame
	void Update () {

        if (animate)
        {
            timer = timer + Time.deltaTime;
            transform.localScale = initialScale * scaleCurve.Evaluate(timer * animationSpeed);
        }
		
	}
}
