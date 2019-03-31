using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {           //어떤오브젝트에서 필요한 다른객체들을 일일히 컴포넌트로 받지말고
    //이거 하나받아놓으면 모든걸(주력오브젝트위주) 접근할수있게끔 현재는 플레이어만있음
    private Player2 player2;

    public Player2 pPlayer2 { get { return player2; } }
    public Vector3 mousePoint;
    void Awake()
    {

        player2 = GameObject.Find("Player2").GetComponent<Player2>();
    }

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
