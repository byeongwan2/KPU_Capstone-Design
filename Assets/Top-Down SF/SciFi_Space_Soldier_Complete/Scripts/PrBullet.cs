using UnityEngine;
using System.Collections;

public class PrBullet : MonoBehaviour {

    public bool isGrenade = false;
    public float explodeTimer = 5.0f;
    public bool friendlyFire = true;
    
    [Header("General Stats")]
    public bool UsePhysicsToTranslate = false;
    public bool UsePhysicsCollisions = false;
    //[HideInInspector]
    public int team = 0;

    public int Damage = 0;
    [HideInInspector]
    public float HitForce = 100.0f;
    [HideInInspector]
    public float BulletSpeed = 1.0f;
    [HideInInspector]
    public float BulletAccel = 0.0f;
    [HideInInspector]
    public float temperatureMod = 0.0f;
    private Vector3 fwd = Vector3.zero;

    [Header("Explosive Stats")]
    public bool RadialDamage = false;
    public float DamageRadius = 3.0f;
    public float RadialForce = 600.0f;
    public float cameraShakeFactor = 3.0f;
    public float cameraShakeDuration = 0.5f;

    [Header("VFX")]
    public bool generatesBloodDamage = true;
    public GameObject DefaultImpactFX;
	private bool UseDefaultImpactFX = true;
	public GameObject DefaultImpactDecal;
    public GameObject[] DetachOnDie;
    

    //Object Pooling
    [Header("Object Pooling")]
    [HideInInspector]
    public bool usePooling = true;
    public float timeToLive = 3.0f;
    private Vector3[] DetachablePositions;
    private Quaternion[] DetachableRotations;
    private Vector3[] DetachableScales;
    [HideInInspector] public Vector3 OriginalScale;

    private bool alreadyDestroyed = false;

    private PrTopDownCamera playerCamera;

    // Use this for initialization
    void Start()
    {
        if (UsePhysicsToTranslate && !isGrenade)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.forward * BulletSpeed * 10);
        }

        if (GameObject.Find("PlayerCamera"))
            playerCamera = GameObject.Find("PlayerCamera").GetComponent<PrTopDownCamera>();

    }

    public void InitializePooling()
    {
        if (DetachOnDie.Length > 0 && !isGrenade)
        {
            DetachablePositions = new Vector3[DetachOnDie.Length];
            DetachableRotations = new Quaternion[DetachOnDie.Length];
            DetachableScales = new Vector3[DetachOnDie.Length];

            int i = 0;
            foreach (GameObject GO in DetachOnDie)
            {
                DetachablePositions[i] = GO.transform.localPosition;
                DetachableRotations[i] = GO.transform.localRotation;
                DetachableScales[i] = GO.transform.localScale;
                i = i + 1;

                //Object Pooling System
                GO.AddComponent<PrDestroyTimer>();
                GO.GetComponent<PrDestroyTimer>().UseObjectPooling = true;
                GO.GetComponent<PrDestroyTimer>().enabled = false;
            }

            
        }

        OriginalScale = this.transform.localScale;
    }


    public void ResetPooling()
    {
        alreadyDestroyed = false;
        UseDefaultImpactFX = true;

        if (DetachOnDie.Length > 0 && !isGrenade)
        {
            int i = 0;
            foreach (GameObject GO in DetachOnDie)
            {
                GO.transform.parent = this.transform;
                GO.transform.localPosition = DetachablePositions[i];
                GO.transform.localRotation = DetachableRotations[i];
                GO.transform.localScale = DetachableScales[i];
                i = i + 1;
      
                GO.GetComponent<PrDestroyTimer>().enabled = false;
            }
        }
        if (GetComponentInChildren<TrailRenderer>())
        {
            GetComponentInChildren<TrailRenderer>().Clear();
        }
    }

	// Update is called once per frame
	void Update () {
        
        if (!UsePhysicsToTranslate)
        {
            if (BulletSpeed > 0.01f)
                BulletSpeed += BulletAccel * Time.deltaTime;

            transform.Translate(Vector3.forward * BulletSpeed);

            fwd = transform.TransformDirection(Vector3.forward);
                       
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, fwd, out hit, BulletSpeed * 2.0f)) {
            if (!alreadyDestroyed && hit.collider.tag != "Bullets" && hit.collider.tag != "MainCamera")
			    DestroyBullet(hit.normal, hit.point, hit.transform, hit.collider.gameObject, hit.collider.tag);
		}

        if (isGrenade)
        {
            if (explodeTimer > 0.0f)
                explodeTimer -= Time.deltaTime;
            else
                DestroyGrenade(Vector3.down, this.transform.position, this.transform);
        }
        else
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive <= 0.0f)
            { 
                if (usePooling)
                    this.gameObject.SetActive(false);
                else
                    Destroy(this.gameObject);
            }
        }
        

    }


    void DestroyGrenade(Vector3 HitNormal, Vector3 HitPos, Transform HitTransform)
    {
        DestroyImmediate(GetComponent<Rigidbody>());
        DestroyImmediate(GetComponent<Collider>());
        Vector3 explosivePos = transform.position;
        Collider[] colls = Physics.OverlapSphere(explosivePos, DamageRadius);
        foreach (Collider col in colls)
        {

            if (col.CompareTag("Destroyable") && col.GetComponent<PrDestroyableActor>().team != team)
            {
                if (col.GetComponent<Rigidbody>())
                {
                    col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                }

                col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                

            }
            else if (col.CompareTag("Player"))
            {
                if (col.GetComponent<PrTopDownCharInventory>())
                {
                    if (!friendlyFire)
                    {
                        if (col.GetComponent<PrTopDownCharInventory>().team != team)
                        {
                            if (col.GetComponent<Rigidbody>())
                            {
                                col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                            }
                            col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                            col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                            col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                            col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                    else
                    {
                        if (col.GetComponent<Rigidbody>())
                        {
                            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                        }
                        col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                        col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                        col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                        col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    }
                    
                }
            }
            else if (col.CompareTag("AIPlayer") && col.GetComponent<PrEnemyAI>().team != team)
            {
                if (col.GetComponent<Rigidbody>())
                {
                    col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                }
                col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
            }


            else if (col.CompareTag("Enemy") && col.GetComponent<PrEnemyAI>().team != team)
            {
                if (col.GetComponent<Rigidbody>())
                {
                    col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                }
                col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);

            }
  
        }

        if (DefaultImpactFX && UseDefaultImpactFX)
            Instantiate(DefaultImpactFX, HitPos + Vector3.up * 0.32f, Quaternion.identity);

        if (DefaultImpactDecal)
        {
            GameObject BulletDecal = Instantiate(DefaultImpactDecal, HitPos, Quaternion.LookRotation(HitNormal)) as GameObject;
            BulletDecal.transform.localPosition += BulletDecal.transform.forward * 0.01f;
        }


        if (playerCamera)
            playerCamera.ExplosionShake(cameraShakeFactor, cameraShakeDuration);

        if (DetachOnDie.Length > 0)
        {
            foreach (GameObject GO in DetachOnDie)
            {
                GO.transform.parent = null;
                GO.AddComponent<PrDestroyTimer>();
                GO.GetComponent<PrDestroyTimer>().DestroyTime = 10f;
            }
        }

        Destroy(this.gameObject);
    }


    void DestroyBullet(Vector3 HitNormal, Vector3 HitPos, Transform HitTransform, GameObject Target, string HitTag)
	{
        alreadyDestroyed = true;

        if (!RadialDamage)
        {
            //Debug.Log("Destroyed");
            if (HitTag == "Destroyable" && Target.GetComponent<PrDestroyableActor>().team != team)
            {
                //Debug.Log("Bullet team = " + team + " Target Team = " + Target.GetComponent<PrDestroyableActor>().team);

                Target.SendMessage("BulletPos", transform.position, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                if (generatesBloodDamage)
                {
                    Target.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    if (Target.GetComponent<Rigidbody>())
                    {
                        Target.GetComponent<Rigidbody>().AddForceAtPosition(HitNormal * -HitForce, HitPos);
                    }
                }
                    
                else
                    Target.SendMessage("ApplyDamageNoVFX", Damage, SendMessageOptions.DontRequireReceiver);

                
            }
            else if (HitTag == "Player" && Target.GetComponent<PrTopDownCharInventory>().team != team)
            {
               
                //Debug.Log("Bullet team = " + team + " Target Team = " + Target.GetComponent<PrTopDownCharInventory>().team);
                Target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("BulletPos", transform.position, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                if (generatesBloodDamage)
                {
                    Target.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    if (Target.GetComponent<PrTopDownCharInventory>().DamageFX != null)
                    {
                        Instantiate(Target.GetComponent<PrTopDownCharInventory>().DamageFX, HitPos, Quaternion.LookRotation(HitNormal));
                        UseDefaultImpactFX = false;
                    }
                }
                else
                    Target.SendMessage("ApplyDamageNoVFX", Damage, SendMessageOptions.DontRequireReceiver);

                
  
            }
            else if (HitTag == "AIPlayer" && Target.GetComponent<PrEnemyAI>().team != team)
            {
                
                // Debug.Log("Bullet team = " + team + " Target Team = " + Target.GetComponent<PrEnemyAI>().team);
                Target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("BulletPos", transform.position, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                if (generatesBloodDamage)
                {
                    Target.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    if (Target.GetComponent<PrEnemyAI>().damageVFX != null)
                    {
                        Instantiate(Target.GetComponent<PrEnemyAI>().damageVFX, HitPos, Quaternion.LookRotation(HitNormal));
                        UseDefaultImpactFX = false;
                    }
                }
                    
                else
                    Target.SendMessage("ApplyDamageNoVFX", Damage, SendMessageOptions.DontRequireReceiver);

                
                
            }
            else if (HitTag == "Enemy" && Target.GetComponent<PrEnemyAI>().team != team)
            {
                // Debug.Log("Bullet team = " + team + " Target Team = " + Target.GetComponent<PrEnemyAI>().team);
                Target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("BulletPos", transform.position, SendMessageOptions.DontRequireReceiver);
                Target.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                if (generatesBloodDamage)
                {
                    Target.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    if (Target.GetComponent<PrEnemyAI>().damageVFX != null)
                    {
                        Instantiate(Target.GetComponent<PrEnemyAI>().damageVFX, HitPos, Quaternion.LookRotation(HitNormal));
                        UseDefaultImpactFX = false;
                    }
                }
                else
                    Target.SendMessage("ApplyDamageNoVFX", Damage, SendMessageOptions.DontRequireReceiver);

                
            }
        }
        else
        {
            if (Target.GetComponent<Rigidbody>())
            {
                Target.GetComponent<Rigidbody>().AddForceAtPosition(HitNormal * -HitForce, HitPos);
            }
 
            //Object Pooling Mode
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;

            Vector3 explosivePos = transform.position;
            Collider[] colls = Physics.OverlapSphere(explosivePos, DamageRadius);
            foreach (Collider col in colls)
            {

                if (col.CompareTag("Destroyable") && col.GetComponent<PrDestroyableActor>().team != team)
                {
                    if (col.GetComponent<Rigidbody>())
                    {
                        col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                    }

                    col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);

                }
                else if (col.CompareTag("Player") && col.GetComponent<PrTopDownCharInventory>().team != team)
                {
                    if (col.GetComponent<Rigidbody>())
                    {
                        col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                    }
                    col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                    
                }
                else if (col.CompareTag("AIPlayer") && col.GetComponent<PrEnemyAI>().team != team)
                {
                    if (col.GetComponent<Rigidbody>())
                    {
                        col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                    }
                    col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                }
                else if (col.CompareTag("Enemy") && col.GetComponent<PrEnemyAI>().team != team)
                {
                    if (col.GetComponent<Rigidbody>())
                    {
                        col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RadialForce, explosivePos, DamageRadius * 3);
                    }
                    col.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("BulletPos", explosivePos, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyTempMod", temperatureMod, SendMessageOptions.DontRequireReceiver);
                    col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);

                }
            }

            if (playerCamera)
                playerCamera.ExplosionShake(cameraShakeFactor, cameraShakeDuration);
        }
		

		if (DefaultImpactFX && UseDefaultImpactFX)
			Instantiate(DefaultImpactFX, HitPos  , Quaternion.LookRotation( HitNormal) );

		if (DefaultImpactDecal && HitTag != "Enemy" && HitTag != "Player" && HitTag != "AIPlayer")
		{
			GameObject BulletDecal = Instantiate(DefaultImpactDecal, HitPos  , Quaternion.LookRotation( HitNormal) ) as GameObject;
			BulletDecal.transform.localPosition += BulletDecal.transform.forward * 0.01f;
			BulletDecal.transform.parent = Target.transform;
		}

        if (DetachOnDie.Length > 0)
        {
            foreach (GameObject GO in DetachOnDie)
            {
                GO.transform.parent = this.transform.parent;

                //Object Pooling System
                GO.GetComponent<PrDestroyTimer>().enabled = true;
                GO.GetComponent<PrDestroyTimer>().DestroyTime = 10f;
            }
        }

        if (usePooling)
            //Object Pooling Mode
            this.gameObject.SetActive(false);
        else
            Destroy(this.gameObject); 
    }
    
	
	void OnCollisionEnter(Collision collision)
	{
        
		if (UsePhysicsCollisions)
        {
            if (!alreadyDestroyed)
                DestroyBullet(collision.contacts[0].normal, collision.contacts[0].point, collision.transform, collision.gameObject, collision.transform.tag);
        }
			
 
    }


}
