using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{    
    new Renderer renderer;

    Shader shaderOrigin;    // 원래 쉐이더
    string shaderOutLine;   // 외곽선 쉐이더
    
    void Start()
    {
        renderer = GetComponent<Renderer>();

        // 쉐이더 저장
        shaderOrigin = this.renderer.material.shader;
        shaderOutLine = "N3K/Outline";

    }    

    public void SetOutLine()
    {        
        renderer.material.shader = Shader.Find(shaderOutLine);
    }
    
    public void SetOrigin()
    {        
        renderer.material.shader = shaderOrigin;
    }
}
