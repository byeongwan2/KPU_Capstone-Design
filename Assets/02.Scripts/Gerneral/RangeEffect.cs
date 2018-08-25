using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    LineRenderer m_line;

    void Start()
    {
        m_line = GetComponent<LineRenderer>();
        m_line.SetVertexCount(10);
        m_line.useWorldSpace = false;
        RangeMaker();
    }
    void RangeMaker()
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

}
