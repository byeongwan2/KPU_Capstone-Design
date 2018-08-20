using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster : MoveObject
{
    //단순 총알발사
    private void ShotBullet()
    {
        bulletShot.Work();
    }
}
