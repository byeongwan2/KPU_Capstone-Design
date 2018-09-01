using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AttackObject {
    // Use this for initialization


    Rigidbody rb;

	void Awake ()
    {
        rb = GetComponent<Rigidbody>();         //성능이슈를 위해 미리 받아놓을뿐
    }

    private void LifeOff()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector4.zero);
        gameObject.SetActive(false);
    }

    public void SetActiveLaunch()          //총알이 켜지면서 초기화
    {
        transform.position = launchPos;
         transform.Rotate(launchRot.eulerAngles);


        Vector3 mpos = Input.mousePosition; //마우스 좌표 저장

        Vector3 pos = transform.position; //게임 오브젝트 좌표 저장
        Vector3 targetPos = new Vector3(mpos.x, mpos.y, Camera.main.transform.position.y);

        Vector3 aim = Camera.main.ScreenToWorldPoint(targetPos);

        float dx = aim.x - pos.x;
        float dz = aim.z - pos.z;

        float rotateDegree = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;
        Debug.Log(rotateDegree);
        Debug.Log(transform.localRotation.y);

        Invoke("LifeOff", 2.0f);        //2초뒤 총알삭제
    }
    [SerializeField]
    float speed = 10.0f;
    [SerializeField]
    int damage = 0;
    public int Damage
    {
        get{ return damage; }
    }
    void FixedUpdate()
    {
        if (gameObject.activeSelf == false) return;
        transform.localPosition += transform.forward * speed * Time.deltaTime;
    }

    public override void StatSetting()
    {
        
    }

    public void DamageSetting(int _damage)
    {
        damage = _damage;
    }
    public void SpeedSetting(float _speed )
    {
        speed = _speed;
    }
}
