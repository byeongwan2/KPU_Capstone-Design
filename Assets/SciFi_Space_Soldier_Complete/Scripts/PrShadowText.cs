using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrShadowText : MonoBehaviour {

    public GameObject text;
    private Text textComponent;
    // Use this for initialization
    void Start () {
        textComponent = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        textComponent.enabled = text.gameObject.activeSelf;
        textComponent.text = text.GetComponent<Text>().text;
        
    }
}
