using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(SphereCollider))]

public class PrTurret : MonoBehaviour
{

    public enum turretState
    {
        Idle,
        ChasingTarget,
        Shooting,
        Dead
    }
    

    [Header("Basic AI Settings")]
    private bool isActive = true;
    [HideInInspector]
    public turretState actualState = turretState.Idle;

    [Header("Turret Settings")]
    public float awarnessDistance = 10.0f;
    public float rotationSpeed = 1.0f;
    [Range(0f, 360f)]
    public float rotationLimit = 60.0f;
    public float timeLimit = 0.0f;
    private float actualTimeLimit = 0.0f;
    private Quaternion initialRot; 

    [Header("Turret Weapon Settings")]
    public bool attackPlayers = false;
    public bool attackEnemies = true;
    public Transform weaponSlot;
    public GameObject weapon;
    public Transform Target;
    private PrWeapon weaponComp;
    private GameObject weaponGO;
    public bool overrideWeaponStats = true;
    public int bulletDamage = 1;
    public float bulletSpeed = 1.0f;
    public float bulletAccel = 0.0f;
    public float fireRate = 0.5f;
    public int clipSize = 10;
    public float reloadTime = 1.0f;

    [Header("Turret Animation Settings")]
    public bool useAnimation = true;
    private bool animate = false;
    public AnimationCurve shootingMovement;
    public float movementFactor = 1.0f;
    public float animationSpeed = 1.0f;
    private float animationTimer = 0.0f;

    private float shootTimer = 1.0f;
    private float actualTimer = 0.0f;

    [Header("Turret Visuals")]
    public bool showArea = true;
    public Renderer areaMesh;
    public bool applyColors = true;
    public Color attackEverythingColor = Color.white;
    public Color attackEnemiesColor = Color.white;
    public Color attackPlayerColor = Color.white;

    //Targeting variables
    private List<GameObject> playerGO;
    //private List<GameObject> enemiesGO;
    private bool targetsInRange = false;
    private Vector3 actualTargetPos = Vector3.zero;
    [HideInInspector]
    public bool enemyDead = false;

    [Header("Debug")]
    //public bool doNotAttackPlayer = false;
    public bool DebugOn = false;
    //public TextMesh DebugText;
    public Mesh AreaMesh;
    //public Mesh TargetArrow;

    // Use this for initialization
    void Start()
    {
        playerGO = new List<GameObject>();
        //enemiesGO = new List<GameObject>();

        if (weaponSlot && weapon)
        {
            weaponGO = Instantiate(weapon, weaponSlot.position, weaponSlot.rotation) as GameObject;
            weaponGO.transform.parent = weaponSlot.transform;
            weaponComp = weaponGO.GetComponent<PrWeapon>() as PrWeapon;
            weaponComp.Player = this.gameObject;
            weaponComp.AIEnemyTarget = Target;
            weaponComp.AIWeapon = true;
            weaponComp.turretWeapon = true;
            if (attackPlayers && !attackEnemies)
                weaponComp.team = 1;
            else if (attackEnemies && !attackPlayers)
                weaponComp.team = 0;
            else if (attackPlayers && attackEnemies)
                weaponComp.team = 3;
            initialRot = weaponSlot.rotation;
            GetComponent<SphereCollider>().radius = awarnessDistance;
            if (overrideWeaponStats)
                OverrideWeaponParameters();
            actualTimer = weaponComp.FireRate;
            shootTimer = weaponComp.FireRate;
        }

        if (showArea && areaMesh)
        {
            SetAreaVisuals(showArea);
        }
        else if (areaMesh)
        {
            SetAreaVisuals(showArea);
        }
    }

    void SetAreaVisuals(bool active)
    {
        areaMesh.GetComponent<Transform>().localScale = new Vector3(awarnessDistance, awarnessDistance, awarnessDistance) * 2;
        areaMesh.material.SetFloat("_Angle", rotationLimit * 0.5f);
        areaMesh.enabled = active;

        if (applyColors)
        {
            if (attackPlayers && !attackEnemies)
                areaMesh.material.SetColor("_BaseColor", attackPlayerColor);
            else if (attackEnemies && !attackPlayers)
                areaMesh.material.SetColor("_BaseColor", attackEnemiesColor);
            else if (attackEnemies && attackPlayers)
                areaMesh.material.SetColor("_BaseColor", attackEverythingColor);
        }

    }

    void OverrideWeaponParameters()
    {
        weaponComp.FireRate = fireRate;
        weaponComp.BulletDamage = bulletDamage;
        weaponComp.BulletSpeed = bulletSpeed;
        weaponComp.BulletAccel= bulletAccel;
        weaponComp.Bullets = clipSize;
        weaponComp.ReloadTime = reloadTime;
    }

    void ChangeState()
    {
        
        if (actualState == turretState.ChasingTarget)
        {
            actualState = turretState.Idle;
        }
    }

    void LateUpdate()
    {
        Vector3 slotPos = weaponGO.transform.localPosition;

        if (animate && useAnimation)
        {
            slotPos.z = shootingMovement.Evaluate(animationTimer) * movementFactor;
            weaponGO.transform.localPosition = slotPos;

            // Increase the timer by the time since last frame
            animationTimer += Time.deltaTime * animationSpeed;
            if (animationTimer > 2.0f)
                animate = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if (timeLimit > 0.0f)
        {
            actualTimeLimit += Time.deltaTime;
            if (actualTimeLimit >= timeLimit)
            {
                isActive = false;
            }
        }
        if (weaponSlot && isActive)
        {
            if (targetsInRange)
            {
                float closestDistance = awarnessDistance;

                foreach (GameObject p in playerGO)
                {
                    
                    if (p.GetComponent<PrTopDownCharInventory>())
                        enemyDead = p.GetComponent<PrTopDownCharInventory>().isDead;
                    else if (p.GetComponent<PrEnemyAI>())
                        enemyDead = p.GetComponent<PrEnemyAI>().dead;

                    if (!enemyDead)
                    {
                        Vector3 targetDir = (p.transform.position + new Vector3(0f, 1.6f, 0f)) - (weaponSlot.position);

                        float angle = Vector3.Angle(targetDir, transform.forward);
                        if (angle < rotationLimit * 0.54f)
                        {
                            float enemyDistance = Vector3.Distance(weaponSlot.position, p.transform.position + new Vector3(0.0f, 1.6f, 0.0f));

                            if (closestDistance > enemyDistance)
                            {
                                //Check if the enemy is visible to add it to the list of possible enemies and look for the closest target
                                RaycastHit hit;

                                if (Physics.Raycast(weaponSlot.position, targetDir, out hit))
                                {
                                    if (hit.collider.CompareTag("Player") && attackPlayers)
                                    {
                                        closestDistance = enemyDistance;
                                        actualTargetPos = p.transform.position + new Vector3(0f, 1.6f, 0f);
                                        Target.position = p.transform.position;
                                        actualState = turretState.ChasingTarget;
                                    }
                                    else if (hit.collider.CompareTag("Enemy") && attackEnemies)
                                    {
                                        closestDistance = enemyDistance;
                                        actualTargetPos = p.transform.position + new Vector3(0f, 1.6f, 0f);
                                        Target.position = p.transform.position;
                                        actualState = turretState.ChasingTarget;
                                    }
                                    else
                                    {
                                        if (actualState == turretState.ChasingTarget)
                                            ChangeState();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (actualState == turretState.ChasingTarget)
                                ChangeState();
                        }
                    }
                    else
                    {
                        playerGO.Remove(p);
                        ChangeState();
                        break;
                    }
                }
            }
            

            switch (actualState)
            {
               
                case turretState.Idle:
                    weaponSlot.rotation = Quaternion.Lerp(weaponSlot.rotation, initialRot, 4 * Time.deltaTime);

                    // Debug.Log("Aiming Player");
                    break;
                case turretState.ChasingTarget:
 
                        Quaternion actualRot = weaponSlot.rotation;
                        weaponSlot.LookAt(actualTargetPos);
                        Quaternion newRot = weaponSlot.rotation;
                        weaponSlot.rotation = Quaternion.Lerp(actualRot, newRot, rotationSpeed * Time.deltaTime);

                        if (actualTimer >= 0.0f)
                            actualTimer -= Time.deltaTime;
                        else
                        {
                            actualTimer = shootTimer;
                            weaponComp.Shoot();
                            if (!weaponComp.Reloading)
                            {
                                animate = true;
                                animationTimer = 0.0f;
                            }
                            else
                            {
                                animate = false;
                            }                           

                    }
                        weaponSlot.transform.eulerAngles = new Vector3(0.0f, weaponSlot.transform.eulerAngles.y, 0.0f);
                  //  }
                    
                    // Debug.Log("Attacking");
                    break;
                /*case turretState.Shooting:
                    if (actualTimer >= 0.0f)
                        actualTimer -= Time.deltaTime;
                    else
                    {
                        actualTimer = shootTimer;
                        weaponComp.Shoot();
                        animate = true;
                        animationTimer = 0.0f;

                    }
                    // Debug.Log("Attacking");
                    break;*/
                case turretState.Dead:
                    //StopMovement();
                    //Debug.Log("Dead");
                    break;
                default:
                    // Debug.Log("NOTHING");
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player" && attackPlayers)
            {
                playerGO.Add(other.gameObject);
                targetsInRange = true;

            }
            if (other.gameObject.tag == "Enemy" && attackEnemies)
            {
                playerGO.Add(other.gameObject);
                targetsInRange = true;
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player" && attackPlayers)
            {
                playerGO.Remove(other.gameObject);
                

            }
            if (other.gameObject.tag == "Enemy" && attackEnemies)
            {
                playerGO.Remove(other.gameObject);

            }

            if (playerGO.Count <= 0 /*&& enemiesGO.Count <= 0*/)
            {
                targetsInRange = false;
                ChangeState();
            }
                
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        /*if (player && playerIsVisible)
        {
            if (eyesAndEarTransform)
                Gizmos.DrawLine(playerTransform.position + new Vector3(0, eyesAndEarTransform.position.y, 0), eyesAndEarTransform.position);
            else
                Gizmos.DrawLine(playerTransform.position + new Vector3(0f, 1.6f, 0f), transform.position + new Vector3(0f, 1.6f, 0f));
        }*/

        Quaternion lRayRot = Quaternion.AngleAxis(-rotationLimit * 0.5f, Vector3.up);
        Quaternion rRayRot = Quaternion.AngleAxis(rotationLimit * 0.5f, Vector3.up);
        Vector3 lRayDir = lRayRot * transform.forward;
        Vector3 rRayDir = rRayRot * transform.forward;
        if (weaponSlot)
        {
            Gizmos.DrawRay(weaponSlot.position, lRayDir * awarnessDistance);
            Gizmos.DrawRay(weaponSlot.position, rRayDir * awarnessDistance);
        }
        else
        {
            Gizmos.DrawRay(transform.position + new Vector3(0f, 1.6f, 0f), lRayDir * awarnessDistance);
            Gizmos.DrawRay(transform.position + new Vector3(0f, 1.6f, 0f), rRayDir * awarnessDistance);
        }

        Gizmos.DrawMesh(AreaMesh, transform.position, Quaternion.identity, Vector3.one * awarnessDistance);

    }
}
