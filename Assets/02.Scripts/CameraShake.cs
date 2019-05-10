using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{    
    public Transform camTransform;
    
    public float shakeDuration = 0f;        // 주기
    
    public float shakeAmount = 0.7f;        // 진폭
    public float decreaseFactor = 1.0f;     // 감소

    Vector3 originalPos;

    void Awake()
    {
        // 굳이 안넣어도 실행되게 null값이면 해당 객체 Transform 가져옴
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }    

    void Update()
    {
        if (shakeDuration > 0)
        {
            originalPos = camTransform.localPosition;
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            originalPos = camTransform.localPosition;
        }
    }
    
    // 총쏠때 주기를 증가시켜서 흔들리게끔 하기 위함
    public void SetDruation(float _shakeDuration)
    {
        shakeDuration += _shakeDuration;
    }
}
