using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check  {

	public static float Distance(Transform _a, Transform _b)
    {
        return Vector3.Distance(_a.position, _b.position);
    }
}
