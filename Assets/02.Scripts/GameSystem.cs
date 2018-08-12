using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {           //어떤오브젝트에서 필요한 다른객체들을 일일히 컴포넌트로 받지말고
    private Player player;                          //이거 하나받아놓으면 모든걸(주력오브젝트위주) 접근할수있게끔 현재는 플레이어만있음
    public Player Player  {
        get
        {
            return player;
        }
        set
        {
            player = value;
        }
    }

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }


}
