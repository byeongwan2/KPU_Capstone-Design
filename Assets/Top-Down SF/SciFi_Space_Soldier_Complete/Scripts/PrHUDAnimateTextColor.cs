using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrHUDAnimateTextColor : MonoBehaviour {

    public float Speed = 1.0f;
    public AnimationCurve colorCurve;
    public Color targetColor = new Color(0, 0, 0, 1);
    private Text textComponent;
    private Color originalColor;
    // Use this for initialization
    void Start () {
        textComponent = GetComponent<Text>();
        originalColor = textComponent.color;
    }

    // Update is called once per frame
    void Update() {
        textComponent.color = Color.Lerp(originalColor, targetColor, colorCurve.Evaluate(Time.time * Speed));
    }
}
