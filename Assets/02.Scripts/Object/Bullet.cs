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
    Vector3 destination;
    public void SetActiveLaunch()          //총알이 켜지면서 초기화
    {
        transform.position = launchPos;
        transform.Rotate(launchRot.eulerAngles);
        /*
        Vector3 mpos2 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
        Debug.Log(mpos2);
        destination = Camera.main.ScreenToWorldPoint(mpos2);
        Debug.Log(destination);
       // destination.y = transform.position.y;
        transform.LookAt(destination);*/
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
