using UnityEngine;
using System.Collections;

public class PrHUDLerpBar : MonoBehaviour {

    public GameObject ReferenceGO;
    public float Speed = 1.0f;
    private Vector3 ActualScale = Vector3.one; 

	// Use this for initialization
	void Start () {
        if (ReferenceGO)
            GetComponent<RectTransform>().localScale = ReferenceGO.GetComponent<RectTransform>().localScale;

    }
	
	// Update is called once per frame
	void Update () {
        ActualScale = Vector3.Lerp(ActualScale, ReferenceGO.GetComponent<RectTransform>().localScale, Time.deltaTime * Speed);
        GetComponent<RectTransform>().localScale = ActualScale;
    }
}
