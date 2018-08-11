using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AttackObject {

    private Vector3 launchPos;
    public void SetLaunchPos(Vector3 _launchPos)
    {
        launchPos = _launchPos;
    }

    private Quaternion launchRot;
    public void SetLaunchRot(Quaternion _launchRot)
    {
        launchRot = _launchRot;
    }

    
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

       // rb.AddForce(transform.forward * speed);
        Invoke("LifeOff", 2.0f);        //2초뒤 총알삭제
    }
    float speed = 100.0f;
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
    public void SpeedSetting(float _speed)
    {
        speed = _speed;
    }
}
