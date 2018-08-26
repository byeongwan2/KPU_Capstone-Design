using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    public LineRenderer m_line;
    public void Init()
    {
        m_line = GetComponent<LineRenderer>();
        m_line.SetVertexCount(10);
        m_line.useWorldSpace = false;
        RangeMaker();
        m_line.enabled = false;
    }

    public void RangeMaker()
    {
        float x, y,z = 0.0f;
        float angle = 20.0f;

        for(int i = 0; i < 10; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * 10.0f;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * 10.0f;

            m_line.SetPosition(i, new Vector3(x, y, z));
            angle += (360.0f / 9);
        }
    }

    void OnRangeLook()
    {
        m_line.enabled = true;
    }
    void OffRangeLook()
    {
        m_line.enabled = false;
    }

    public void RangeLookExit()
    {
        OffRangeLook();
    }

    public void RangeLook(float _dangerTime)
    {
        Invoke("OnRangeLook", _dangerTime);
    }
}
