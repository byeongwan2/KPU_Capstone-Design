using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {


	

    public void Dash_Destination(Vector3 _dest,float _speed)
    {
        _dest.y = transform.position.y;
        transform.LookAt(_dest);
        transform.position += transform.forward * _speed * Time.deltaTime;
    }
}
