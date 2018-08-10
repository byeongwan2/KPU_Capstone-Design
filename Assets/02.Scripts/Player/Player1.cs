using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour {           //애니메이션 이벤트로만 발동되는 콜백함수모음 클래스

    //단순 총알발사
    private void ShotBullet()
    {
        bulletShot.Work();
    }
}
