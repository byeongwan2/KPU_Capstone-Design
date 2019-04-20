using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrEnemyAI : MonoBehaviour
{

    public enum enemyType
    {
        Soldier,
        Pod
    }

    public enum AIState
    {
        Patrol,
        Wander,
        ChasingPlayer,
        AimingPlayer,
        Attacking,
        CheckingSound,
        Dead,
        TowerDefensePath,
        FriendlyFollow,
    }

    [Header("Health and Stats")]
    public enemyType type = enemyType.Soldier;
    public int Health = 100;
    [HideInInspector]
    public bool dead = false;

    [Header("Temperature Settings")]
    public bool useTemperature = true;
    [HideInInspector]
    public float temperature = 1.0f;
    public float temperatureThreshold = 2.0f;
    public int temperatureDamage = 5;
    public float onFireSpeedFactor = 0.5f;
    private float tempTimer = 0;
    
    [Header("Basic AI Settings")]
    public AIState actualState = AIState.Patrol;
    private bool stopPlaying = false;

    public float chasingSpeed = 1.0f;
    public float normalSpeed = 0.75f;
    public float aimingSpeed = 0.3f;
    public bool stopIfGetHit = true;
    public float rotationSpeed = 0.15f;
    public float randomWaypointAccuracy = 1.0f;
    
    public bool useRootmotion = true;
    public bool standInPlace = false;

    public bool lockRotation = false;
    public Vector3 lockedRotDir = Vector3.zero;

    private bool canAttack = true;

    //[HideInInspector]
    public int team = 1;

    public bool friendlyAI = false;
    public List<GameObject> friends;
    private GameObject closestFriend;
    private float closestFriendDistance = 99999.0f;

    [Header("AI Sensor Settings")]

    public float awarnessDistance = 20;
    public float aimingDistance = 15;
    public float attackDistance = 8;
    private float playerActualDistance = 99999;

    [Range(10f, 360f)]
    public float lookAngle = 90f;
    public float hearingDistance = 20;

    public Transform eyesAndEarTransform;
    private Transform actualSensorTrans;
    private Vector3 actualSensorPos;

    private bool aiming = false;
    [HideInInspector]
    public Animator enemyAnimator;
    private bool playerIsVisible = false;
    private float playerLastTimeSeen = 0.0f;

    public float forgetPlayerTimer = 5.0f;
    private float actualforgetPlayerTimer = 0.0f;
    private Vector3 lastNoisePos;
    private float alarmedTimer = 10.0f;
    private float actualAlarmedTimer = 0.0f;
    private float newtAlarm = 0.0f;

    [HideInInspector]
    public bool lookForPlayer = false;

    [Header("Waypoints Settings")]
    public PrWaypointsRoute waypointRoute;
    private int actualWaypoint = 0;
    [HideInInspector]
    public Transform[] waypoints;
    public bool waypointPingPong = false;
    private bool inverseDirection = false; 
    private bool waiting = false;
    private float timeToWait = 3.0f;
    private float actualTimeToWait = 0.0f;
    private float waitTimer = 0.0f;
    public Vector3 finalGoal = Vector3.zero;

    [Header("Tower Defense Settings")]
    public bool towerDefenseAI = false;
    public Transform towerDefenseTarget;
    public int towerDefenseStage = 0;
    private bool pathEnded = false;
    private Vector3 attackPos = Vector3.zero;


    [Header("Weapon Settings")]
    public bool useArmIK = true;
    public Transform WeaponGrip;
    public GameObject AssignedWeapon;
    private PrWeapon weapon;
    private float LastFireTimer = 0.0f;
    public float FireRate = 1.0f;
    public float attackAngle = 5f;
    public int meleeAttacksOptions = 1;
    private int actualMeleeAttack = 0;
    public bool chooseRandomMeleeAttack = true;
        
    public List<GameObject> play;
    public Transform playerTransform;
 
    [Header("VFX")]
     
    public int hitAnimsMaxTypes = 1;
    //private int lastHitAnim = -1;
    private int randomHitAnim = -1;
    public GameObject spawnFX;
    public GameObject damageVFX;
    public GameObject explosionFX;
    public bool destroyOnDead = false;
    public Renderer[] MeshRenderers;
    private Vector3 LastHitPos = Vector3.zero;
    private bool Damaged = false;
    private float DamagedTimer = 0.0f;
    public bool destroyDeadBody = false;
    public float destroyDeadBodyTimer = 5.0f;
    [Space]
    public Transform BurnAndFrozenVFXParent;
    public GameObject frozenVFX;
    private GameObject actualFrozenVFX;
    public GameObject burningVFX;
    private GameObject actualBurningVFX;
    [Space]
    public GameObject damageSplatVFX;
    private PrBloodSplatter actualSplatVFX;
    [Space]
    public GameObject deathVFX;
    public float deathVFXHeightOffset = -0.1f;
    private GameObject actualDeathVFX;

    [Space]
    //Explosive Death VFX
    public bool useExplosiveDeath = true;
    private bool explosiveDeath = false;
    public int damageThreshold = 50;
    public GameObject explosiveDeathVFX;
    private GameObject actualExplosiveDeathVFX;

    //Ragdoll Vars
    [Header("Ragdoll setup")]
    public bool useRagdollDeath = false;
    public float ragdollForceFactor = 1.0f;
    private Vector3 ragdollForce;

    [Header("Sound FX")]

    public float FootStepsRate = 0.4f;
    public float generalFootStepsVolume = 1.0f;
    public AudioClip[] Footsteps;
    private float LastFootStepTime = 0.0f;
    private AudioSource Audio;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent agent;

    [Header("Debug")]
    public bool doNotAttackPlayer = false;
    public bool DebugOn = false;
    public TextMesh DebugText;

    public Mesh AreaMesh;
    public Mesh TargetArrow;

    private PrCharacterIK CharacterIKController;
    private Transform ArmIKTarget;

    // Use this for initialization
    public virtual void Start()
    {
        //Debug
        if (DebugText && !DebugOn)
            DebugText.GetComponent<Renderer>().enabled = false;

        //Create Waypoints Array
        SetWaypoints();

        //Ass
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        //Spawn FX
        if (spawnFX)
            Instantiate(spawnFX, transform.position, Quaternion.identity);

        //Rigidbody
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (eyesAndEarTransform)
            actualSensorTrans = eyesAndEarTransform;
        else
            actualSensorTrans = this.transform;

        actualSensorPos = actualSensorTrans.position;
        actualforgetPlayerTimer = forgetPlayerTimer;

        SetTimeToWait();
        Audio = GetComponent<AudioSource>();

        //Friendly AI setup
        if (friendlyAI)
        {
            gameObject.tag = "AIPlayer";
            team = 0;
        }

        //Find Players
        FindPlayers();

        if (GetComponent<Animator>())
            enemyAnimator = GetComponent<Animator>();

        //Initialize Waypoints
        if (waypoints.Length > 0)
        {
            finalGoal = waypoints[0].transform.position;
        }

        //Initialize Weapon
        InstantiateWeapon();
        if (lookForPlayer && !towerDefenseAI)
            CheckPlayerVisibility(360f);

        if (useRootmotion)
            enemyAnimator.applyRootMotion = false;

        //ragdoll Initialization
        gameObject.AddComponent<PrCharacterRagdoll>();

        if (useExplosiveDeath && explosiveDeathVFX)
        {
            actualExplosiveDeathVFX = Instantiate(explosiveDeathVFX, transform.position, transform.rotation) as GameObject;
            actualExplosiveDeathVFX.SetActive(false);

            if (GameObject.Find("VFXBloodParent"))
                actualExplosiveDeathVFX.transform.parent = GameObject.Find("VFXBloodParent").transform;
            else
            {
                GameObject VFXParent = new GameObject("VFXBloodParent") as GameObject;
                actualExplosiveDeathVFX.transform.parent = VFXParent.transform;
            }
        }

        if (damageSplatVFX)
        {
            GameObject GOactualSplatVFX = Instantiate(damageSplatVFX, transform.position, transform.rotation) as GameObject;
            GOactualSplatVFX.transform.position = transform.position;
            GOactualSplatVFX.transform.parent = transform;
            actualSplatVFX = GOactualSplatVFX.GetComponent<PrBloodSplatter>();
        }

        if (frozenVFX)
        {
            actualFrozenVFX = Instantiate(frozenVFX, transform.position, transform.rotation) as GameObject;
            actualFrozenVFX.transform.position = transform.position;
            actualFrozenVFX.transform.parent = transform;
            if (BurnAndFrozenVFXParent)
                actualFrozenVFX.transform.parent = BurnAndFrozenVFXParent;

        }
        if (burningVFX)
        {
            actualBurningVFX = Instantiate(burningVFX, transform.position, transform.rotation) as GameObject;
            actualBurningVFX.transform.position = transform.position;
            actualBurningVFX.transform.parent = transform;
            if (BurnAndFrozenVFXParent)
                actualBurningVFX.transform.parent = BurnAndFrozenVFXParent;
        }

        if (deathVFX)
        {
            actualDeathVFX = Instantiate(deathVFX, transform.position, transform.rotation) as GameObject;
            actualDeathVFX.SetActive(false);

            if (GameObject.Find("VFXBloodParent"))
                actualDeathVFX.transform.parent = GameObject.Find("VFXBloodParent").transform;
            else
            {
                GameObject VFXParent = new GameObject("VFXBloodParent") as GameObject;
                actualDeathVFX.transform.parent = VFXParent.transform;
            }
        }



        agent.enabled = true;
          

        if (!towerDefenseAI)
            GetCLoserWaypoint();

        GameObject[] AIs = GameObject.FindGameObjectsWithTag("AIPlayer");
        foreach (GameObject AI in AIs)
        {
            AI.GetComponent<PrEnemyAI>().FindPlayers();
        }

    }

    public void SetWaypoints()
    {
        if (waypointRoute)
        {
            waypoints = new Transform[waypointRoute.waypoints.Length];
            timeToWait = waypointRoute.timeToWait;

            for (int i = 0; i < (waypoints.Length); i++)
            {
                waypoints[i] = waypointRoute.waypoints[i];

            }
        }
    }

    public void StopAllActivities()
    {
        Debug.Log("Stop Moving");
        stopPlaying = true;
        actualState = AIState.Wander;

        enemyAnimator.SetFloat("Speed", 0.0f);
        enemyAnimator.SetBool("Aiming", false);
        play = new List<GameObject>();
    }

    public void FindPlayers()
    {
        //NEW
        //USUAL ENEMY 

        if (!friendlyAI)
        {
            //Debug.Log("Trying to find players");
            play = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

            List<GameObject> AIplayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("AIPlayer"));
            if (AIplayers.Count != 0)
            {
                foreach (GameObject AI in AIplayers)
                {
                    AI.GetComponent<PrEnemyAI>().GetClosestPlayer();
                }
                play.AddRange(AIplayers);
            }
        }
        //FRIENDLY AI
        else if (friendlyAI)
        {
            friends = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
            GetClosestFriends();
            //Debug.Log("Friends =" + friends.Count);

            play = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            
        }
        

        GetClosestPlayer();
    }
   
    public void GetClosestFriends()
    {
      
        //Debug.Log("GETTING CLOSEST Friends " + gameObject.name);
        
        foreach (GameObject f in friends)
        {
            if (f != null)
            {
                if (closestFriendDistance > Vector3.Distance(transform.position, f.transform.position))
                {
                    closestFriend = f;
                    
                    //Debug.Log("FRIEND TRANSFORM =" + closestFriend.name);

                    closestFriendDistance = Vector3.Distance(transform.position, f.transform.position);
                }
                
            }


        }
        if (friends.Count == 0)
        {
           // Debug.Log("NO FRIENDS FOUND " + gameObject.name);
        }
        
    }


    public void GetClosestPlayer()
    {
        /*
        if (friendlyAI)
        {
            //Debug.Log("GETTING CLOSEST ENEMY " + gameObject.name);
        }*/
        if (play.Count != 0)
        {
           /* if (friendlyAI)
            {
                //Debug.Log("ENEMY AI COUNT =" + play.Count);
            }*/
            foreach (GameObject p in play)
            {
                if (p != null)
                {
                    if (playerActualDistance > Vector3.Distance(actualSensorPos, p.transform.position + new Vector3(0.0f, 1.6f, 0.0f)))
                    {
                        playerTransform = p.transform;
                        /*if (friendlyAI)
                        {
                            //Debug.Log("ENEMY TRANSFORM =" + playerTransform.name);
                        }*/
                        playerActualDistance = Vector3.Distance(actualSensorPos, p.transform.position + new Vector3(0.0f, 1.6f, 0.0f));
                    }
                }
            }
        }
        else 
        {
            playerActualDistance = 9999.0f;
            playerTransform = null;
            actualState = AIState.Patrol;
            //Debug.Log("NO ENEMIES FOUND " + gameObject.name);
        }
    }

    void OnAnimatorMove()
    {
        if (agent != null && useRootmotion)
            agent.velocity = enemyAnimator.deltaPosition / Time.deltaTime;
    }

    void BulletPos(Vector3 BulletPosition)
    {
        LastHitPos = BulletPosition;
        LastHitPos.y = 0;
    }

    void RandomHitAnim()
    {
        if (randomHitAnim < hitAnimsMaxTypes)
        {
            randomHitAnim += 1;
        }
        else
        {
            randomHitAnim = 0;
        }
        //randomHitAnim = Random.Range(0, hitAnimsMaxTypes);
        //if (randomHitAnim == lastHitAnim)
        //    RandomHitAnim();
    }

    void ApplyDamagePassive(int damage)
    {
        if (actualState != AIState.Dead)
        {
            Health -= damage;
  
            if (Health <= 0)
            {
                if (damage >= damageThreshold)
                    explosiveDeath = true;
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;
                
                Die(true);
            }
        }
    }

    void ApplyTempMod(float temperatureMod)
    {
        temperature += temperatureMod;
        temperature = Mathf.Clamp(temperature, 0.0f, temperatureThreshold);
    }

    void ApplyDamage(int damage)
    {

        if ( actualState != AIState.Dead)
        {
            EnableArmIK(false);

            if (weapon.Type == PrWeapon.WT.Melee)
                SetCanAttack(false);
            
            //Get Damage Direction
            Vector3 hitDir = new Vector3(LastHitPos.x,0, LastHitPos.z) - transform.position;
            Vector3 front = transform.forward;

            if (type == enemyType.Pod)
            {
                if (Vector3.Dot(front, hitDir) > 0)
                {
                    enemyAnimator.SetInteger("Side", 1);
                }
                else
                {
                    enemyAnimator.SetInteger("Side", 0);
                }
            }
            enemyAnimator.SetTrigger("Hit");
            //RandomHitAnim();
            //lastHitAnim = randomHitAnim;
            enemyAnimator.SetInteger("Type", Random.Range(0, hitAnimsMaxTypes));
            
            Damaged = true;
            DamagedTimer = 1.0f;

            if (playerTransform != null && !towerDefenseAI)
            {
                agent.ResetPath();
                CheckPlayerNoise(playerTransform.position);
                actualState = AIState.ChasingPlayer;
            }
                
            Health -= damage;

            if (actualSplatVFX)
            {
                actualSplatVFX.transform.LookAt(LastHitPos);
                actualSplatVFX.Splat();
            }

            if (stopIfGetHit)
                agent.velocity = Vector3.zero;

            if (Health <= 0)
            {
                if (damage >= damageThreshold)
                    explosiveDeath = true;
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;
                Die(false);
            }
        }
    }

    void ApplyDamageNoVFX(int damage)
    {

        if (actualState != AIState.Dead)
        {
            Health -= damage;

            if (Health <= 0)
            {
                if (damage >= damageThreshold)
                    explosiveDeath = true;
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;
                Die(true);
            }
        }
    }

    void PodDestruction(Vector3 hitDir)
    {
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<CharacterController>());
        Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
        GetComponent<Animator>().enabled = false;

        if (transform.Find("Root").GetComponent<SphereCollider>())
            transform.Find("Root").GetComponent<SphereCollider>().enabled = true;
        if (transform.Find("Root").GetComponent<Rigidbody>())
        {
            transform.Find("Root").GetComponent<Rigidbody>().isKinematic = false;
            transform.Find("Root").GetComponent<Rigidbody>().AddForce(hitDir * -10, ForceMode.Impulse);

        }

        if (destroyOnDead)
        {
            PrDestroyTimer DestroyScript = GetComponent<PrDestroyTimer>();
            DestroyScript.enabled = true;
        }

        gameObject.name = gameObject.name + "_DEAD";
        SendMessageUpwards("EnemyDead", SendMessageOptions.DontRequireReceiver);
    }

    void SoldierDestruction(bool temperatureDeath)
    {
        actualState = AIState.Dead;
        enemyAnimator.SetBool("Dead", true);
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
        if (useRagdollDeath)
        {
            GetComponent<PrCharacterRagdoll>().ActivateRagdoll();
            ragdollForce = ((transform.position + new Vector3(0, 1.5f, 0)) - (LastHitPos + new Vector3(0f, 1.6f, 0f))) * (ragdollForceFactor * Random.Range(0.8f, 1.2f));
            
            if (!temperatureDeath)
                GetComponent<PrCharacterRagdoll>().SetForceToRagdoll(LastHitPos + new Vector3(0f, 1.6f, 0f), ragdollForce, BurnAndFrozenVFXParent);
        }

        if (explosiveDeath && actualExplosiveDeathVFX)
        {
            actualExplosiveDeathVFX.transform.position = transform.position;
            actualExplosiveDeathVFX.transform.rotation = transform.rotation;
            actualExplosiveDeathVFX.SetActive(true);
            actualExplosiveDeathVFX.SendMessage("SetExplosiveForce", LastHitPos + new Vector3(0, 1.5f, 0), SendMessageOptions.DontRequireReceiver);
          
            Destroy(this.gameObject);
            
        }
        else
        {
            if (deathVFX && actualDeathVFX)
            {
                if (temperatureDeath)
                {
                    //Freezing of Burning Death VFX...
                }
                else
                {
                    actualDeathVFX.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    actualDeathVFX.transform.LookAt(LastHitPos);
                    actualDeathVFX.transform.position = new Vector3(transform.position.x, deathVFXHeightOffset, transform.position.z);

                    actualDeathVFX.SetActive(true);

                    ParticleSystem[] particles = actualDeathVFX.GetComponentsInChildren<ParticleSystem>();

                    if (particles.Length > 0)
                    {
                        foreach (ParticleSystem p in particles)
                        {
                            p.Play();
                        }
                    }
                }
                
            }
        }
    }

    void Die(bool temperatureDeath)
    {
        //Send Message to Spawners
        EnableArmIK(false);
        SendMessageUpwards("EnemyDead", SendMessageOptions.DontRequireReceiver);

        gameObject.tag = "Untagged";
        Vector3 hitDir = LastHitPos - transform.position;

        if (explosionFX)
        {
            GameObject DieFXInstance = Instantiate(explosionFX, transform.Find("Root").transform.position, Quaternion.identity) as GameObject;
            DieFXInstance.transform.parent = transform.Find("Root").transform;
        }

        if (type == enemyType.Pod)
        {
            PodDestruction(hitDir);
        }
        else if (type == enemyType.Soldier)
        {
            SoldierDestruction(temperatureDeath);
        }

        dead = true;
    }

    void EnableArmIK(bool active)
    {
        if (CharacterIKController && useArmIK)
            CharacterIKController.ikActive = active;

        //Debug.Log("AimIK ON = " + active);
    }

    void InstantiateWeapon()
    {
        if (AssignedWeapon && WeaponGrip)
        {
            GameObject InstWeapon = Instantiate(AssignedWeapon, WeaponGrip.position, WeaponGrip.rotation) as GameObject;
            InstWeapon.transform.parent = WeaponGrip;
            InstWeapon.transform.localRotation = Quaternion.Euler(90, 0, 0);

            weapon = InstWeapon.GetComponent<PrWeapon>();
            weapon.Player = this.gameObject;
            weapon.team = team;
            weapon.AIWeapon = true;
            weapon.LaserSight.enabled = false;
            if (weapon.Type == PrWeapon.WT.Melee)
            {
                weapon.MeleeRadius = attackDistance;
                aimingDistance = attackDistance;
            }

            FireRate = weapon.FireRate;

            if (playerTransform)
                weapon.AIEnemyTarget = playerTransform;

            if (useArmIK)
            {
                if (weapon.gameObject.transform.Find("ArmIK"))
                {
                    ArmIKTarget = weapon.gameObject.transform.Find("ArmIK");
                    if (GetComponent<PrCharacterIK>() == null)
                    {
                        gameObject.AddComponent<PrCharacterIK>();
                        CharacterIKController = GetComponent<PrCharacterIK>();
                    }
                    else if (GetComponent<PrCharacterIK>())
                    {
                        CharacterIKController = GetComponent<PrCharacterIK>();
                    }

                    if (CharacterIKController)
                    {
                        CharacterIKController.leftHandTarget = ArmIKTarget;
                        EnableArmIK(true);
                    }

                }
                else
                {
                    if (CharacterIKController != null)
                        EnableArmIK(false);
                }
            }
        }
    }

    void SetRandomPosVar(Vector3 goal)
    {
        finalGoal = goal + new Vector3(Random.Range(-randomWaypointAccuracy, randomWaypointAccuracy), 0, Random.Range(-randomWaypointAccuracy, randomWaypointAccuracy));
    }

    void SetTimeToWait()
    {
        actualTimeToWait = Random.Range(timeToWait * 0.75f, -timeToWait * 0.75f) + timeToWait;
    }

    public void SwitchDebug()
    {
        if (DebugOn)
        {
            DebugOn = false;
        }
        else
        {
            DebugOn = true;
        }

        if (DebugText)
            DebugText.GetComponent<Renderer>().enabled = DebugOn;

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!stopPlaying)
        {
            UpdateRenderers();
            UpdateInput();
            SetState();
            ApplyState();
            ApplyRotation();
            ApplyTemperature();
            SetDebug();
        }
        
    }

    public virtual void ApplyTemperature()
    {
        if (useTemperature)
        {
            if (temperature > 1.0f || temperature < 1.0f)
            {
                if (tempTimer < 1.0f)
                    tempTimer += Time.deltaTime;
                else
                {
                    tempTimer = 0.0f;
                    applyTemperatureDamage();
                }
            }

            foreach (Renderer Mesh in MeshRenderers)
            {
                if (Mesh.material.HasProperty("_FrozenMix"))
                    Mesh.material.SetFloat("_FrozenMix", Mathf.Clamp(1.0f - temperature, 0.0f, 1.0f));
                if (Mesh.material.HasProperty("_BurningMix"))
                    Mesh.material.SetFloat("_BurningMix", Mathf.Clamp(temperature - 1.0f, 0.0f, 1.0f));
            }

            foreach (SkinnedMeshRenderer SkinnedMesh in MeshRenderers)
            {
                if (SkinnedMesh.material.HasProperty("_FrozenMix"))
                    SkinnedMesh.material.SetFloat("_FrozenMix", Mathf.Clamp(1.0f - temperature, 0.0f, 1.0f));
                if (SkinnedMesh.material.HasProperty("_BurningMix"))
                    SkinnedMesh.material.SetFloat("_BurningMix", Mathf.Clamp(temperature - 1.0f, 0.0f, 1.0f));
            }

            if (actualFrozenVFX)
            {
                if (temperature < 1.0f)
                    actualFrozenVFX.SetActive(true);
                else
                {
                    actualFrozenVFX.SetActive(false);
                }
            }
            if (actualBurningVFX)
            {
                if (temperature > 1.0f)
                    actualBurningVFX.SetActive(true);
                else
                {
                    actualBurningVFX.SetActive(false);
                }
            }
        }
        

    }

    public virtual void applyTemperatureDamage()
    {
        if (temperature < 1.0f)
        {
            ApplyDamagePassive(temperatureDamage);
        }
        else if (temperature > 1.0f)
        {
            ApplyDamagePassive(temperatureDamage);
        }
    }

    public virtual void UpdateRenderers()
    {
        if (Damaged && MeshRenderers.Length > 0)
        {
            DamagedTimer = Mathf.Lerp(DamagedTimer, 0.0f, Time.deltaTime * 15);

            if (Mathf.Approximately(DamagedTimer, 0.0f))
            {
                DamagedTimer = 0.0f;
                Damaged = false;
            }
            
            foreach (Renderer Mesh in MeshRenderers)
            {
                if (Mesh.material.HasProperty("_DamageFX"))
                    Mesh.material.SetFloat("_DamageFX", DamagedTimer);
            }

            foreach (SkinnedMeshRenderer SkinnedMesh in MeshRenderers)
            {
                if (SkinnedMesh.material.HasProperty("_DamageFX"))
                    SkinnedMesh.material.SetFloat("_DamageFX", DamagedTimer);
            }
        }
    }
    public virtual void UpdateInput()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            SwitchDebug();
        }
    }

    public virtual void SetState()
    {
        if (Health <= 0 || dead)
        {
            actualState = AIState.Dead;
            if (destroyDeadBody)
            {
                destroyDeadBodyTimer -= Time.deltaTime;
                if (destroyDeadBodyTimer <= 0.0f)
                {
                    //Debug.Log("BodyDestroyed");
                    Destroy(gameObject);
                    
                }
            }
        }
        else
        {
            //Set variables
            if (eyesAndEarTransform)
                actualSensorPos = actualSensorTrans.position;
            else
                actualSensorPos = transform.position + new Vector3(0.0f, 1.6f, 0.0f);
            if (!towerDefenseAI) //Generic AI against Player
            {
                if (friendlyAI)
                {
                    GetClosestPlayer();
                    GetClosestFriends();
                    if (closestFriend != null)
                        closestFriendDistance = Vector3.Distance(transform.position, closestFriend.transform.position);
                    if (playerTransform != null)
                    {
                        //Debug.Log(playerTransform.name + " " + playerActualDistance);
                        CheckPlayerVisibility(lookAngle);
                        if (!playerIsVisible)
                        {
                            playerTransform = null;
                            playerActualDistance = 999.0f;
                        }
                    }
                    else
                        actualState = AIState.FriendlyFollow;
                }
                else
                {
                    if (actualAlarmedTimer > 0.0)
                    {
                        actualAlarmedTimer -= Time.deltaTime;
                    }

                    if (play.Count != 0 && Health > 0 && !doNotAttackPlayer)
                    {
                        GetClosestPlayer();
                        if (playerTransform != null)
                            playerActualDistance = Vector3.Distance(actualSensorPos, playerTransform.position + new Vector3(0.0f, 1.6f, 0.0f));

                        if (actualAlarmedTimer > 0.0)
                        {
                            actualState = AIState.CheckingSound;
                        }
                        else if (actualAlarmedTimer <= 0.0 && playerActualDistance <= awarnessDistance)
                        {
                            CheckPlayerVisibility(lookAngle);
                            if (!playerIsVisible)
                            {
                                playerTransform = null;
                                playerActualDistance = 999.0f;
                            }
                        }
                        else if (actualAlarmedTimer <= 0.0f || playerActualDistance > awarnessDistance)
                        {
                            actualState = AIState.Patrol;
                        }
                        else if (Health > 0)
                        {
                            actualState = AIState.Patrol;
                        }
                    }
                }
                
            }
            else //TOWER DEFENSE AI DECISIONS
            {
                if (towerDefenseAI)
                {
                    actualState = AIState.TowerDefensePath;
                    if (towerDefenseTarget)
                        playerActualDistance = Vector3.Distance(actualSensorPos, towerDefenseTarget.position + new Vector3(0.0f, 1.6f, 0.0f));
                    else
                        playerActualDistance = 0.0f;
                }
            }
            
        }
    }
    public virtual void ApplyState()
    {
        switch (actualState)
        {
            case AIState.Patrol:
                if (standInPlace)
                {
                    StopMovement();
                }
                else if (waypoints.Length > 0 && !standInPlace)
                {
                    if (agent.remainingDistance >= 1.0f && !waiting)
                    {
                        if (agent.remainingDistance >= 2.0f)
                            MoveForward(normalSpeed, finalGoal);
                        else
                            MoveForward(aimingSpeed, finalGoal);
                    }
                    else if (!waiting && waitTimer < actualTimeToWait)
                    {
                        waiting = true;

                    }
                    if (waiting)
                    {
                        StopMovement();
                        if (waitTimer < actualTimeToWait)
                            waitTimer += Time.deltaTime;
                        else
                            ChangeWaytpoint();
                    }
                }
                //Debug.Log("patrolling");
                break;
            case AIState.Wander:
                StopMovement();
                break;
            case AIState.ChasingPlayer:
                if (playerTransform)
                {
                    if (standInPlace)
                    {
                        StopMovement();
                    }
                    else
                    {
                        MoveForward(chasingSpeed, playerTransform.position);
                    }
                }
                   

                //Debug.Log("chasing");
                break;
            case AIState.AimingPlayer:

                if (playerTransform)
                {
                    if (standInPlace)
                    {
                        StopMovement();
                    }
                    else
                    {
                        MoveForward(aimingSpeed, playerTransform.position);
                    }
                    LookToTarget(playerTransform.position);
                    AttackPlayer();
                }
                // Debug.Log("Aiming Player");
                break;
            case AIState.Attacking:
                if (playerTransform)
                {
                    LookToTarget(playerTransform.position);
                    AttackPlayer();
                    StopMovement();
                    
                }
                else
                {
                    playerActualDistance = 999.0f;
                    GetClosestPlayer();
                }
                // Debug.Log("Attacking");
                break;
            case AIState.Dead:
                StopMovement();
                //Debug.Log("Dead");
                break;
            case AIState.CheckingSound:
                if (standInPlace)
                {
                    StopMovement();
                }
                else if (Vector3.Distance(transform.position, lastNoisePos) >= 2.0f)
                {
                    MoveForward(normalSpeed, lastNoisePos);
                }
                else
                {
                    StopMovement();
                }
                CheckPlayerVisibility(lookAngle);
                //  Debug.Log("Checking noise position!!");
                break;
            case AIState.TowerDefensePath:
                if (waypoints.Length > 0 && !pathEnded)
                {
                    towerDefenseStage = 0;
                    if (agent.remainingDistance >= 1.5f)
                        MoveForward(normalSpeed, finalGoal);
                    else
                        ChangeWaytpoint();
                   //Debug.Log("Tower Defense Waypoints");
                }
                else if (actualWaypoint == waypoints.Length -1 && pathEnded)
                {
                    if (playerActualDistance > (attackDistance * 0.8f))
                    {
                        towerDefenseStage = 1;
                        attackPos = Vector3.zero;
                        if (towerDefenseTarget)
                            MoveForward(normalSpeed, towerDefenseTarget.position);
                        
                        //Debug.Log("Tower Defense Chasing Target");
                    }
                    else
                    {
                        towerDefenseStage = 2;
                        StopMovement();
                        if (towerDefenseTarget)
                        {
                            LookToTarget(towerDefenseTarget.position);
                            AttackTower();
                        }
                        //Debug.Log("Tower Defense Attacking!!!!");
                    }
                }
                
                //Debug.Log("Tower Defense Attack!!!");
                break;
            case AIState.FriendlyFollow:
                if (standInPlace)
                {
                    StopMovement();
                }
                else if (closestFriendDistance >= 4.0f && closestFriend != null)
                {
                    MoveForward(normalSpeed, closestFriend.transform.position);
                }
                
                else
                {
                    StopMovement();
                }
                CheckPlayerVisibility(lookAngle);
                //Debug.Log("Dead");
                break;
            
            default:
                // Debug.Log("NOTHING");
                break;
        }
    }
    public virtual void ApplyRotation()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y , 0);
        if (agent != null)
        {
            if (useTemperature && temperature >= 0.05f)
                agent.updateRotation = true;
            else if (useTemperature && temperature < 0.05f)
                agent.updateRotation = false;
            else if (!useTemperature)
                agent.updateRotation = true;
        }
    }
    public virtual void SetDebug()
    { 
        if (DebugText && DebugOn)
        {
            DebugText.text = actualState.ToString() + "\n" + "Alarmed= " + Mathf.Round(actualAlarmedTimer) + "\n" + "ForgetPlayer= " + Mathf.Round(actualforgetPlayerTimer);
            if (actualState == AIState.Patrol)
                DebugText.color = Color.white;
            else if (actualState == AIState.ChasingPlayer)
                DebugText.color = Color.green * 3;
            else if (actualState == AIState.AimingPlayer)
                DebugText.color = Color.yellow * 2;
            else if (actualState == AIState.Attacking)
                DebugText.color = Color.red * 2;
            else if (actualState == AIState.CheckingSound)
                DebugText.color = Color.cyan;
            else if (actualState == AIState.Dead)
                DebugText.color = Color.gray;
            else if (actualState == AIState.FriendlyFollow)
                DebugText.color = Color.white;
            if (friendlyAI)
            {
                if (playerTransform)
                {
                    float distance = Vector3.Distance(playerTransform.position, transform.position);
                    DebugText.text = DebugText.text + "\n" + "EnemyDistance= " + Mathf.Round(distance) + "\n" + "actualEnemy= " + playerTransform.name + "\n" + "CanAttack= " + canAttack;
                }
                   
                else
                    DebugText.text = DebugText.text + "\n" + "FriendDistance= " + closestFriendDistance;
            }
        }

    }

    void AttackPlayer()
    {
        Vector3 targetDir = (playerTransform.position + new Vector3(0f, 1.6f, 0f)) - (actualSensorPos);

        float angle = Vector3.Angle(targetDir, transform.forward);
        if (angle < attackAngle)
        {
            enemyAnimator.ResetTrigger("Alert");
            enemyAnimator.SetTrigger("CancelAlert");

            if (weapon && !doNotAttackPlayer)
            {

                if (Time.time >= (LastFireTimer + FireRate))
                {
                    LastFireTimer = Time.time;
                    //Attack Melee
                    if (weapon.Type == PrWeapon.WT.Melee)
                    {
                        UseMeleeWeapon();
                    }
                    //Attack Ranged 
                    else
                    {
                        ShootWeapon();
                    }
                }
            }
        }

    }


    void AttackTower()
    {
        if (towerDefenseTarget)
        {
            Vector3 targetDir = (towerDefenseTarget.position + new Vector3(0f, 1.6f, 0f)) - (actualSensorPos);

            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < attackAngle)
            {
                if (weapon)
                {
                    if (Time.time >= (LastFireTimer + FireRate))
                    {
                        LastFireTimer = Time.time;
                        //Attack Melee
                        if (weapon.Type == PrWeapon.WT.Melee)
                        {
                            //Debug.LogWarning("Using Weapon " + gameObject.name);
                            UseMeleeWeapon();
                        }
                        //Attack Ranged 
                        else
                        {
                            ShootWeapon();
                        }
                    }
                }
            }
        }
        
    }

    void ShootWeapon()
    {
        if (canAttack)
        {
            if (playerTransform)
                weapon.AIEnemyTarget = playerTransform;

            weapon.Shoot();
            if (weapon.Reloading == false)
            {
                if (weapon.ActualBullets > 0)
                    enemyAnimator.SetTrigger("Shoot");
                else
                {
                    weapon.Reload();
                }
                    
            }
        }
       
    }
    
    /*
    public void EndReload()
    {
        //canAttack = true;
        enemyAnimator.SetBool("Reloading", false);
    }*/

    void UseMeleeWeapon()
    {
        if (canAttack)
        {
            if (playerTransform)
                weapon.AIEnemyTarget = playerTransform;

            enemyAnimator.SetTrigger("MeleeAttack");

            if (chooseRandomMeleeAttack)
                enemyAnimator.SetInteger("MeleeType", Random.Range(0, meleeAttacksOptions));
            else
            {
                enemyAnimator.SetInteger("MeleeType", actualMeleeAttack);
                if (actualMeleeAttack < meleeAttacksOptions - 1)
                    actualMeleeAttack += 1;
                else
                    actualMeleeAttack = 0;
            }
        }

    }

    void MeleeEvent()
    {
        if (!towerDefenseAI)
        {
            weapon.AIAttackMelee(playerTransform.position, playerTransform.gameObject);
        }
        else
        {
            if (towerDefenseTarget)
                weapon.AIAttackMelee(towerDefenseTarget.position, towerDefenseTarget.gameObject);
        }
    }

    void SetCanAttack(bool set)
    {
        canAttack = set;
       
    }
    void CanAttack()
    {
        SetCanAttack(true);
        EnableArmIK(true);
    }

    void CheckPlayerNoise(Vector3 noisePos)
    {
        if (!doNotAttackPlayer && !dead && !towerDefenseAI && !friendlyAI)
        {
            Vector3 currentGoal = agent.destination;
            SetWaypoint(noisePos);
            UnityEngine.AI.NavMeshPath NoisePath = agent.path;
            

            if (agent.remainingDistance != 0 && agent.CalculatePath(noisePos, NoisePath))
            {
                if (newtAlarm == 0.0f || Time.time >= newtAlarm + 15f)
                {
                    if (actualState == AIState.Patrol)
                    {
                        if (enemyAnimator)
                            enemyAnimator.SetTrigger("Alert");
                        lastNoisePos = noisePos;
                        newtAlarm = Time.time;

                        actualAlarmedTimer = alarmedTimer;

                        agent.SetDestination(noisePos);

                        //Debug.Log(gameObject.name + " New Noise Position assigned. Position: " + lastNoisePos);
                        
                    }
                }
                else
                {
                    lastNoisePos = noisePos;
                    newtAlarm = Time.time;

                    actualAlarmedTimer = alarmedTimer;

                    agent.SetDestination(noisePos);

                    //Debug.Log(gameObject.name + " New Noise Position assigned");
                }
               
            }
           
            else
            {
                agent.SetDestination(currentGoal);
                //Debug.Log(gameObject.name + " Can´t Reach Noise");
                //Debug.Log("Can´t Reach Noise ");
            }
        }

    }

    void PlayerVisibilityRay(Vector3 targetDir)
    {
        RaycastHit hit;
        int AIInt = 9;
        int objectInt = 8;
        int playerInt = 14;
        int playerLayer = 1 << playerInt;
        int AILayer = 1 << AIInt;
        int objectLayer = 1 << objectInt;
        int finalMask = playerLayer | objectLayer | AILayer;

        if (!friendlyAI)
        {
            if (Physics.Raycast(actualSensorPos, targetDir, out hit, 999.0f, finalMask))
            //if (Physics.Raycast(actualSensorPos, targetDir, out hit))
            {
                if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("AIPlayer"))
                {
                    //Debug.Log("Can´t see Player");
                    playerIsVisible = false;
                    if (actualforgetPlayerTimer <= 0.1f)
                    {
                        actualState = AIState.Patrol;
                    }
                }
                else if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("AIPlayer"))
                {
                    //Debug.Log("Seeing Player " + player.transform.position);
                    //Debug.DrawRay(actualSensorPos, targetDir, Color.magenta);

                    playerIsVisible = true;
                    actualAlarmedTimer = 0.0f;
                    newtAlarm = 0.0f;
                }
            }
        }
        else if (friendlyAI)
        {
            if (Physics.Raycast(actualSensorPos, targetDir, out hit, 999.0f, finalMask))
            {
                if (!hit.collider.CompareTag("Enemy"))
                {
                    //Debug.Log("Can´t see Player");

                    playerIsVisible = false;
                    if (actualforgetPlayerTimer <= 0.1f)
                    {
                        actualState = AIState.FriendlyFollow;
                    }
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    //Debug.Log("Seeing Player " + player.transform.position);
                    //Debug.DrawRay(actualSensorPos, targetDir, Color.green);

                    playerIsVisible = true;
                    actualAlarmedTimer = 0.0f;
                    newtAlarm = 0.0f;
                }
            }
        }
       
    }

    public void CheckPlayerVisibility(float actualLookAngle)
    {
        if (playerTransform != null)
        {
            Vector3 targetDir = (playerTransform.position + new Vector3(0f, 1.6f, 0f)) - (actualSensorPos);

            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < actualLookAngle && !doNotAttackPlayer && !dead)
            {
                if (Time.time >= playerLastTimeSeen)
                {
                    PlayerVisibilityRay(targetDir);
                    playerLastTimeSeen = Time.time + 0.1f;
                }

                if (playerIsVisible)
                {
                    playerActualDistance = Vector3.Distance(actualSensorPos, playerTransform.position + new Vector3(0.0f, 1.6f, 0.0f));

                    if (playerActualDistance > aimingDistance /*&& agent.remainingDistance != 0*/)
                    {
                        actualState = AIState.ChasingPlayer;
                        enemyAnimator.SetBool("Aiming", false);
                    }
                    else if (playerActualDistance <= aimingDistance /*&& agent.remainingDistance != 0*/)
                    {
                        if (playerActualDistance <= attackDistance)
                        {
                            if (!friendlyAI)
                            {
                                if (playerTransform && playerTransform.GetComponent<PrTopDownCharInventory>())
                                {
                                    if (playerTransform.GetComponent<PrTopDownCharInventory>().isDead == false)
                                        actualState = AIState.Attacking;
                                    else
                                    {
                                        FindPlayers();
                                        actualState = AIState.Patrol;
                                    } 
                                       
                                }
                                else if (playerTransform && playerTransform.GetComponent<PrEnemyAI>())
                                {
                                    if (playerTransform.GetComponent<PrEnemyAI>().dead == false)
                                        actualState = AIState.Attacking;
                                    else
                                    {
                                        FindPlayers();
                                        actualState = AIState.Patrol;
                                    }
                                       
                                }
                            }
                            else
                            {
                                if (playerTransform && playerTransform.GetComponent<PrEnemyAI>().dead == false)
                                    actualState = AIState.Attacking;
                                else
                                {
                                    FindPlayers();
                                    actualState = AIState.FriendlyFollow;
                                }
                            }
                        }
                        else
                        {
                            if (!friendlyAI)
                            {
                                if (playerTransform && playerTransform.GetComponent<PrTopDownCharInventory>())
                                {
                                    if (playerTransform.GetComponent<PrTopDownCharInventory>().isDead == false)
                                        actualState = AIState.AimingPlayer;
                                    else
                                    {
                                        FindPlayers();
                                        actualState = AIState.Patrol;
                                    }
                                }
                                else if (playerTransform && playerTransform.GetComponent<PrEnemyAI>())
                                {
                                    if (playerTransform.GetComponent<PrEnemyAI>().dead == false)
                                        actualState = AIState.AimingPlayer;
                                    else
                                    {
                                        FindPlayers();
                                        actualState = AIState.Patrol;
                                    }
                                }
                                else
                                    actualState = AIState.Patrol;
                            }
                            else
                            {
                                if (playerTransform && playerTransform.GetComponent<PrEnemyAI>().dead == false)
                                    actualState = AIState.AimingPlayer;
                                else
                                {
                                    FindPlayers();
                                    actualState = AIState.FriendlyFollow;
                                }

                            }

                        }
                        enemyAnimator.SetBool("Aiming", true);
                    }
                    //}

                }
            }
            else if (actualAlarmedTimer > 0.0f)
            {
                actualState = AIState.CheckingSound;
                enemyAnimator.SetBool("Aiming", false);
            }
            else
            {
                actualState = AIState.Patrol;
                enemyAnimator.SetBool("Aiming", false);
            }
        }
        

    }
    void FootStep()
    {
        if (Footsteps.Length > 0 && Time.time >= (LastFootStepTime + FootStepsRate))
        {
            int FootStepAudio = 0;

            if (Footsteps.Length > 1)
            {
                FootStepAudio = Random.Range(0, Footsteps.Length);
            }

            float FootStepVolume = enemyAnimator.GetFloat("Speed") * generalFootStepsVolume;
            if (aiming)
                FootStepVolume *= 0.5f;

            Audio.PlayOneShot(Footsteps[FootStepAudio], FootStepVolume);

            LastFootStepTime = Time.time;
        }
    }

    void StopMovement()
    {
        if (agent != null)
        {
            if (!towerDefenseAI)
            {
                agent.velocity = Vector3.zero;
               
                enemyAnimator.SetFloat("Speed", 0.0f, 0.5f, Time.deltaTime);
            }
            else
            {
                if (attackPos == Vector3.zero)
                    attackPos = transform.position;
                transform.position = attackPos;

                agent.velocity = Vector3.zero;
                enemyAnimator.SetFloat("Speed", 0.0f);
            }
        }
           
    }

    void ChangeWaytpoint()
    {
        waiting = false;
        if (!waypointPingPong && !towerDefenseAI) //Unidirectional Waypoint
        {
            if (actualWaypoint < waypoints.Length - 1)
                actualWaypoint = actualWaypoint + 1;
            else
                actualWaypoint = 0;
        }
        else if (waypointPingPong && !towerDefenseAI)//Ping pong waypoints
        {
            if (!inverseDirection)
            {
                if (actualWaypoint < waypoints.Length - 1)
                    actualWaypoint = actualWaypoint + 1;
                else
                {
                    inverseDirection = true;
                    actualWaypoint = actualWaypoint - 1;
                }
            }
            else
            {
                if (actualWaypoint > 0)
                    actualWaypoint = actualWaypoint - 1;
                else
                {
                    inverseDirection = false;
                    actualWaypoint = 1;
                }
            }
        }
        else if (towerDefenseAI)
        {
            if (actualWaypoint < waypoints.Length - 1)
            {
                actualWaypoint = actualWaypoint + 1;
                //Debug.Log("ActualWaypoint =" + actualWaypoint);
            }
            else
                pathEnded = true;
        }

        waitTimer = 0.0f;
        SetTimeToWait();
        SetWaypoint(waypoints[actualWaypoint].transform.position);

    }

    public void SetWaypoint(Vector3 Pos)
    {
        SetRandomPosVar(Pos);
        agent.SetDestination(finalGoal);
    }

    public void MoveForward(float speed, Vector3 goal)
    {
        if (useRootmotion)
        {
            agent.destination = goal;
            enemyAnimator.SetFloat("Speed", speed, 0.4f, Time.deltaTime);
            if (useTemperature)
            {
                if (temperature > 1.0f)
                    enemyAnimator.SetFloat("Temperature", 2 - Mathf.Clamp(temperature, 0.0f, onFireSpeedFactor));
                else if (temperature <= 1.0f)
                    enemyAnimator.SetFloat("Temperature", Mathf.Clamp(temperature, 0.0f, onFireSpeedFactor));

            }
            
        }
        else
        {
            //Debug.Log("Moving Forward");
            agent.destination = goal;
            if (useTemperature)
            {
                if (temperature <= 1.0f)
                {
                    agent.speed = (speed + 0.8f) * Mathf.Clamp(temperature, 0.0f, 1.0f);
                    enemyAnimator.SetFloat("Speed", speed * Mathf.Clamp(temperature, 0.5f, 1.0f), 0.25f, Time.deltaTime);
                }
                else if (temperature > 1.0f)
                {
                    float tempExtra = Mathf.Clamp(temperature - 1.0f,1.0f,2.0f) * onFireSpeedFactor;
                    Debug.Log(tempExtra);
                    agent.speed = (speed + 0.8f) * tempExtra;
                    enemyAnimator.SetFloat("Speed", speed * tempExtra, 0.25f, Time.deltaTime);
                    
                }
                enemyAnimator.SetFloat("Temperature", Mathf.Clamp(temperature, 0.0f, 1.0f));
            }
            else
            {
                agent.speed = (speed + 0.8f);
                enemyAnimator.SetFloat("Speed", speed, 0.25f, Time.deltaTime);
            }
           

        }

    }

    void LookToTarget(Vector3 target)
    {

        Quaternion targetRot = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Mathf.Clamp(temperature, 0.0f,1.0f));

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Noise")
        {
            CheckPlayerNoise(other.transform.position);
        }


    }

    public void GetCLoserWaypoint()
    {
        
        int selected = 0;
        float selDist = 999f;
        float dist = 0.0f;
        bool changeWayp = false;
        if (waypoints.Length > 0)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                dist = agent.remainingDistance;

                if (dist <= selDist)
                {
                    selDist = dist;

                    actualWaypoint = selected;

                    changeWayp = true;
                }
                selected += 1;
            }
            if (changeWayp)
            {
                ChangeWaytpoint();
            }
            else
            {
                SetWaypoint(waypoints[0].position);

            }
        }
    }

    void LateUpdate()
    {
        if (lockRotation)
        {
            transform.rotation = Quaternion.Euler(lockedRotDir);
        }
    }

    void EndMelee()
    {
    }


    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (playerTransform && playerIsVisible)
        {
            if (eyesAndEarTransform)
                Gizmos.DrawLine(playerTransform.position + new Vector3(0, eyesAndEarTransform.position.y, 0), eyesAndEarTransform.position);
            else
                Gizmos.DrawLine(playerTransform.position + new Vector3(0f, 1.6f, 0f), transform.position + new Vector3(0f, 1.6f, 0f));
        }

        Quaternion lRayRot = Quaternion.AngleAxis(-lookAngle * 0.5f, Vector3.up);
        Quaternion rRayRot = Quaternion.AngleAxis(lookAngle * 0.5f, Vector3.up);
        Vector3 lRayDir = lRayRot * transform.forward;
        Vector3 rRayDir = rRayRot * transform.forward;
        if (eyesAndEarTransform)
        {
            Gizmos.DrawRay(eyesAndEarTransform.position, lRayDir * awarnessDistance);
            Gizmos.DrawRay(eyesAndEarTransform.position, rRayDir * awarnessDistance);
        }
        else
        {
            Gizmos.DrawRay(transform.position + new Vector3(0f, 1.6f, 0f), lRayDir * awarnessDistance);
            Gizmos.DrawRay(transform.position + new Vector3(0f, 1.6f, 0f), rRayDir * awarnessDistance);
        }

        Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * awarnessDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * aimingDistance);
        Gizmos.DrawSphere(lastNoisePos, 1.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * attackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * hearingDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawMesh(TargetArrow, finalGoal, Quaternion.identity, Vector3.one);

        if (useRagdollDeath && dead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(LastHitPos + new Vector3(0f, 1.6f, 0f), 0.1f);
            Gizmos.DrawRay(LastHitPos + new Vector3(0f, 1.6f, 0f),  ragdollForce);
            
        }

        
    }

}
