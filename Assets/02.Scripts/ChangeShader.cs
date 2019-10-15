using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{    
    new Renderer renderer;
    
    string shaderOrigin;    // 원래 쉐이더
    string shaderOutLine;   // 외곽선 쉐이더
    string shaderHit;       // 피격 쉐이더
    bool isHit;
    
    void Start()
    {        
        renderer = GetComponent<Renderer>();
        
        isHit = false;
        //shaderOrigin = "PolygonR/PBR_Character_Overlay";
        shaderOrigin = renderer.material.shader.name;
        shaderOutLine = "N3K/Outline";
        shaderHit = "Unlit/Color";
    }

    public void SetOutLine()
    {
        if (isHit)        
            return;       
        renderer.material.shader = Shader.Find(shaderOutLine);
    }
    
    public void SetOrigin()
    {
        if (isHit)        
            return;        
        renderer.material.shader = Shader.Find(shaderOrigin);
    }

    public void SetHit()
    {   if(isHit)        
            renderer.material.shader = Shader.Find(shaderHit);
    }
    
    public void SetIsHit(bool val)
    {
        isHit = val;
    }
}