using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Around : MonoBehaviour {               //특정 포인트들을 왓다리 갓다리

    public List<Transform> wayPoints;

    private Transform tr;
    [SerializeField]
    private Vector3 destination;

    private bool arounding = true;
    void Start()
    {
        tr = GetComponent<Transform>();
        arounding = true;
        AroundAction();
        StartCoroutine(DestinationCheck());
    }

    void AroundAction()
    {
        var dest = Random.Range(0, wayPoints.Count);
        destination = wayPoints[dest].position;
    }

    IEnumerator DestinationCheck()              //사실상 Around의 Update대체
    {
        yield return new WaitForSeconds(1.0f);
        if(1.0f >Check.Distance(tr.position, destination) && arounding == true)
        {
            AroundAction();
        }
        StartCoroutine(DestinationCheck());
    }

    public void SwitchAround()        //전원
    {
        if (arounding) arounding = false;
        else arounding = true;
    }

    public bool IsArounding()
    {
        return arounding;
    }

    public float GetDirection(Vector3 _vec)        //방향을 결정해줌  // Enemy들 전용
    {
        float x = destination.x - _vec.x;
        float z = destination.z - _vec.z;
        return Mathf.Atan2(x , z) * Mathf.Rad2Deg;
    }

}
