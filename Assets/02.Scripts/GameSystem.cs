using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {          

    public Vector3 mousePoint;

    //마우스 의 화면좌표를 얻어오는건 항상
    void Update()
    {
        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장       
        Vector3 mpos2 = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);
        mousePoint = Camera.main.ScreenToWorldPoint(mpos2);
    }

    public Vector3 MousePoint()
    {
        return mousePoint;
    }

  

}
