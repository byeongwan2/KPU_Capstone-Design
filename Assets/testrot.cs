using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testrot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float t = Time.realtimeSinceStartup * 5.0f;

        Vector3 vec = PrefabSystem.instance.player.transform.position ;
        //vec.y = vec.y;
        vec.x = vec.x + 10.0f * Mathf.Cos(t);
        vec.z = vec.z + 10.0f * Mathf.Sin(t);

        //vec.Normalize();
        transform.position = vec * 10.0f * Time.deltaTime ;
        //transform.position = vec;
    }
}
