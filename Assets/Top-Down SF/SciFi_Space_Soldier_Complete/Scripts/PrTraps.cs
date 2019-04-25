using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrTraps : MonoBehaviour {

    public enum Traps
    {
        AreaDamage = 0,
        Explosive = 1
    }
    [Header("Trap General Settings")]
    public Traps TrapType;

    public bool isActive = true;
    public bool affectPlayers = true;
    public bool affectEnemys = false;

    private float damageTimer = 0.0f;
    private List<GameObject> playerGO;
    private List<GameObject> enemiesGO;
    private bool damageOn = false;

    [Header("Area Damage Trap")]
    public float movSpeedFactor = 1.0f;
    public int damage = 10;
    public float damageByNSeconds = 1.0f;

    [Header("VFX")]
    public bool useTargetTransform = false;
    public Transform vfxTransform;
    public Vector3 vfxPosOffset = Vector3.zero;
    public GameObject vfxPrefab;
    private GameObject vfxInstance;

    // Use this for initialization
    void Start () {
        playerGO = new List<GameObject>();
        enemiesGO = new List<GameObject>();

        damageTimer = damageByNSeconds - 0.1f;

        if (vfxPrefab)
        {
            vfxInstance = Instantiate(vfxPrefab, transform.position, Quaternion.identity) as GameObject;
            vfxInstance.SetActive(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
	    if (damageOn)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageByNSeconds)
            {
                damageTimer = 0.0f;
                ApplyDamage();
            }
        }
	}

    void PlayVFX(Transform TargetPos)
    {
        if (useTargetTransform)
        {
            vfxInstance.transform.position = TargetPos.position;
        }
        else
        {
            vfxInstance.transform.position = vfxTransform.position + vfxPosOffset;
            vfxInstance.transform.rotation = vfxTransform.rotation;
        }

        vfxInstance.SetActive(true);
        ParticleSystem[] vfxArray = vfxInstance.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem vfx in vfxArray)
        {
            vfx.Play();
        }
        
    }

    void ApplyDamage()
    {
        foreach (GameObject p in playerGO )
        {
            p.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            if (vfxPrefab)
                PlayVFX(p.transform);
            if (TrapType == Traps.Explosive)
            {
                GetComponent<Collider>().enabled = false;
                damageOn = false;
                gameObject.SetActive(false);
            }
        }
        foreach (GameObject e in enemiesGO)
        {
            if (e != null)
            {
                e.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                if (vfxPrefab)
                    PlayVFX(e.transform);
                if (TrapType == Traps.Explosive)
                {
                    GetComponent<Collider>().enabled = false;
                    damageOn = false;
                    gameObject.SetActive(false);
                }
            }
            
        }
    }

    void ApplySpeed(GameObject target, float speedFactor)
    {
        if (movSpeedFactor != 1.0f)
        {
            foreach (GameObject p in playerGO)
            {
                if (p != null)
                    p.SendMessage("SetNewSpeed", speedFactor, SendMessageOptions.DontRequireReceiver);
            }
            foreach (GameObject e in enemiesGO)
            {
                if (e != null)
                    e.SendMessage("SetNewSpeed", speedFactor, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player" && affectPlayers )
            {
                damageOn = true;
                playerGO.Add(other.gameObject);
                ApplySpeed(other.gameObject, movSpeedFactor);

            }
            if (other.gameObject.tag == "Enemy" && affectEnemys )
            {
                damageOn = true;
                enemiesGO.Add(other.gameObject);
                ApplySpeed(other.gameObject, movSpeedFactor);
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player" && affectPlayers)
            {
                damageOn = false;
                ApplySpeed(other.gameObject, 1.0f);
                playerGO.Remove(other.gameObject);
               
            }
            if (other.gameObject.tag == "Enemy" && affectEnemys)
            {
                damageOn = false;
                ApplySpeed(other.gameObject, 1.0f);
                enemiesGO.Remove(other.gameObject);
                
            }
        }

    }
}
