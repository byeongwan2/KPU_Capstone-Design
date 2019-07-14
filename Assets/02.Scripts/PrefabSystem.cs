using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//게임전체에 한개만 존재하는 스크립트
public enum TYPE { BOMB, BULLET ,MONSTER  , ADVANCEBULLET,ROBOT,BOMBEFFECT,ENEMYBULLET}
public class PrefabSystem : MonoBehaviour {                //프리팹시스템에서 모든 오브젝트풀을 관리하니까 나중에 분리할필요가있음
    public static PrefabSystem instance = null;             //제네릭클래스로 바꿔야함
    public List<GameObject> allMonster;
    public Player2 player;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this.gameObject); Debug.Log("매니저중복추적"); }
        //DontDestroyOnLoad(this.gameObject);

        //프리팹시스템에서 플레이어 인스턴스를 가지고있음 
        player = GameObject.Find("Player2").GetComponent<Player2>();
    }
   
    enum eWHO { PLAYER }

    private List<GameObject> bombPool = new List<GameObject>();         //폭탄을 미리생성
    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> advanceBulletPool = new List<GameObject>();
    public StringBuilder st = new StringBuilder();
    List<GameObject> activeBullet = new List<GameObject>();

    private List<GameObject> enemyBulletPool = new List<GameObject>();

    private List<GameObject> bombEffectPool = new List<GameObject>();
    //이 함수가 불리는시점에 켜져있는 플레이어 1번총알을 리스트에 담아서 반환
    public List<GameObject> Get_BulletPool()
    {
        activeBullet.Clear();
        foreach(var a in bulletPool)
        {
            if(a.activeSelf == true)
            {
                activeBullet.Add(a);
            }
        }
        return activeBullet;
    }
    // 오브젝트풀을 만드는 함수  게임실행시 한번만 호출
    public void Create_Prefab(TYPE _type,GameObject _gameObject , int _count)       //여러가지 폭탄을 생성할수 잇게끔
    {
        if (_type == TYPE.BOMB) Select_PoolType(bombPool, _gameObject, _count);
        else if (_type == TYPE.BULLET) Select_PoolType(bulletPool, _gameObject, _count);
        else if (_type == TYPE.ADVANCEBULLET) Select_PoolType(advanceBulletPool, _gameObject, _count);
        else if (_type == TYPE.BOMBEFFECT) Select_PoolType(bombEffectPool, _gameObject, _count);
        else if (_type == TYPE.ENEMYBULLET) Select_PoolType(enemyBulletPool, _gameObject, _count);
    }
   
    //오브젝트가 필요할떄마다 이 함수가 한개씩 켜줌
    public GameObject Active_Prefab(TYPE _type) 
    {
        if (_type == TYPE.BOMB) return Choice_Pool(bombPool);
        else if (_type == TYPE.BULLET) return Choice_Pool(bulletPool);
        else if (_type == TYPE.ADVANCEBULLET) return Choice_Pool(advanceBulletPool);
        else if (_type == TYPE.BOMBEFFECT) return Choice_Pool(bombEffectPool);
        else if (_type == TYPE.ENEMYBULLET) return Choice_Pool(enemyBulletPool);
        return null;
    }
    //실제 해당오브젝트에서 한개씩 켜주고 반환
    private GameObject Choice_Pool<T>(T _pool) where T : List<GameObject>            //수많은풀을 if문으로 하지않고 일반화
    {
        foreach( var obj in _pool)
        {
            if (obj.activeSelf == false)
            {
                obj.SetActive(true);
                return obj;               //꼭 반환아니여도 다른방법이 있을지 생각해볼필요
            }
        }
        return null;
    }


    private void Select_PoolType<T>(T _pool,GameObject _gameObject, int _count ) where T : List<GameObject>
    {
        for (int i = 0; i < _count; i++)
        {
            var obj = Instantiate<GameObject>(_gameObject, this.transform);

            _pool.Add(obj);
            obj.active = false;
        }

        
    }

    
}
