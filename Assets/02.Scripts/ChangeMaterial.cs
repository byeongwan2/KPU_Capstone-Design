using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    Material origin_mt;     // 기존 재질
    Material outLine_mt;    // Outline 재질
    new Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        // 재질 할당
        origin_mt = renderer.material;
        outLine_mt = Resources.Load("OutLine", typeof(Material)) as Material;
        
    }    

    public void SetOutLine()
    {
        // 재질 변경
        renderer.material = outLine_mt;
    }
    
    public void SetOrigin()
    {
        // 재질 변경
        renderer.material = origin_mt;
    }
}
