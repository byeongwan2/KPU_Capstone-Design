using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {          
    private Player2 player2;

    public Player2 pPlayer2 { get { return player2; } } //어디서든지 플레이어객체에 접근할수 있게
    public Vector3 mousePoint;
    void Awake()
    {

        player2 = GameObject.Find("Player2").GetComponent<Player2>();
    }

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
