using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    Transform parentTr;
    public LineRenderer m_line;
    public void Init()
    {
        m_line = GetComponent<LineRenderer>();
        m_line.SetVertexCount(20);
        m_line.useWorldSpace = false;
        RangeMaker();
        m_line.enabled = false;
        parentTr = GetComponentInParent<Transform>();
    }

    public void RangeMaker()
    {
        float x, y,z = 0.0f;
        float angle = 20.0f;

        for(int i = 0; i < 20; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * 10.0f;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * 10.0f;

            m_line.SetPosition(i, new Vector3(x, y, z));
            angle += (360.0f / 19);
        }
    }

    void OnRangeLook()
    {
        m_line.enabled = true;
        if (parentTr.rotation.x < -88.0f && parentTr.rotation.x > -92.0f 
            && parentTr.rotation.x > 88.0f && parentTr.rotation.x < 92.0f)
        return;
        
        transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0, 0));
    }
    void OffRangeLook()
    {
        m_line.enabled = false;
    }

    public void RangeLookExit()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        OffRangeLook();
    }

    public void RangeLook(float _dangerTime)
    {
        Invoke("OnRangeLook", _dangerTime);
    }
}
