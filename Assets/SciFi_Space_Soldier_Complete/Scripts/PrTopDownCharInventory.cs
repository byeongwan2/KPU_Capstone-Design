using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrTopDownCharInventory : MonoBehaviour {

    [Header("Stats")]

    public int Health = 100;
    [HideInInspector] public int ActualHealth = 100;
        
    public float Stamina = 1.0f;
    public float StaminaRecoverSpeed = 0.5f;
    [HideInInspector] public float ActualStamina = 1.0f;
    public float StaminaRecoverLimit = 0.5f;
    private float ActualStaminaRecover = 0.5f;

    [HideInInspector] public bool UsingStamina = false;
    [HideInInspector] public bool isDead = false;
    public bool DestroyOnDead = false;

    private bool Damaged = false;
    private float DamagedTimer = 0.0f;
    
    private SphereCollider NoiseTrigger;
    [HideInInspector]
    public float actualNoise = 0.0f;
    private float noiseDecaySpeed = 10.0f;

    private int enemyTeam = 1;

    [Header("Temperature Settings")]
    public bool useTemperature = true;
    [HideInInspector]
    public float temperature = 1.0f;
    public float temperatureThreshold = 2.0f;
    public int temperatureDamage = 5;
    public float onFireSpeedFactor = 0.5f;
    private float tempTimer = 0;

    [Header("Weapons")]

    public bool alwaysAim = false;

    public int playerWeaponLimit = 2;

    public PrWeapon[] InitialWeapons;

    private float lastWeaponChange = 0.0f;
    [HideInInspector]
    public bool Armed = true;
    [HideInInspector]
    public GameObject[] Weapon;
    [HideInInspector]
    public int ActiveWeapon = 0;
    private bool CanShoot = true;
    public PrWeaponList WeaponListObject;
    private GameObject[] WeaponList;
    public Transform WeaponR;
    public Transform WeaponL;

    public bool aimingIK = false;
    public bool useArmIK = true;
    //[HideInInspector]
    public int team = 0;

    //Grenade Vars
    [Header("Grenades Vars")]
    public float ThrowGrenadeMaxForce = 100f;
    public GameObject grenadesPrefab;
    public int maxGrenades = 10;
    public int grenadesCount = 5;
    private bool isThrowing = false;

    [HideInInspector] public bool Aiming = false;

    private float FireRateTimer = 0.0f;
	private float LastFireTimer = 0.0f;

    private Transform AimTarget;

    [Header("VFX")]
    public GameObject DamageFX;
    private Vector3 LastHitPos = Vector3.zero;
    public Renderer[] MeshRenderers;
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

    [Header("Sound FX")]
    
    public float FootStepsRate = 0.4f;
    public float GeneralFootStepsVolume = 1.0f;
    public AudioClip[] Footsteps;
    private float LastFootStepTime = 0.0f;
    private AudioSource Audio;

    //USe Vars
    [Header("Use Vars")]
    public float UseAngle = 75.0f;
    [HideInInspector] public GameObject UsableObject;
    [HideInInspector] public bool UsingObject = false;
            
    //Pickup Vars
    private GameObject PickupObj;
    [HideInInspector]
    public int BlueKeys = 0;
    [HideInInspector]
    public int RedKeys = 0;
    [HideInInspector]
    public int YellowKeys = 0;
    [HideInInspector]
    public int FullKeys = 0;

    //HuD Vars
    [Header("HUD")]
    public GameObject Compass;
    public TextMesh CompassDistance;
    private bool CompassActive = false;
    private Transform CompassTarget;

    public GameObject HUDHealthBar;
    public GameObject HUDStaminaBar;
    public GameObject HUDDamageFullScreen;

    [HideInInspector]
    public float HUDDamage;

    public GameObject HUDWeaponPicture;
    public GameObject HUDWeaponBullets;
    public GameObject HUDWeaponBulletsBar;
    public GameObject HUDWeaponClips;
    public GameObject HUDUseHelper;
    public GameObject HUDColorBar;

    public GameObject HUDGrenadesIcon;
    public GameObject HUDGrenadesCount;

    [Header("Quick Reload HUD")]

    public GameObject quickReloadPanel;
    public GameObject quickReloadMarker;
    public GameObject quickReloadZone;
    [HideInInspector]
    public bool useQuickReload = true;

    [Header("Multiplayer HUD")]
    public float multiplayerHUDOffset = 70.0f;
    private bool splitScreen = false;
    private int totalPlayers = 1;
    private Vector2 splitOff = new Vector2(200,112);
    private Vector2 splitMargins = new Vector2(10, 0);
    private float splitScaleFactor = 0.9f;

    //Private References
    [HideInInspector]
    public PrTopDownCharController charController;
    [HideInInspector]
    public Animator charAnimator;

    private GameObject[] Canvases;

    //ArmIK
    private Transform ArmIKTarget = null;

    private PrCharacterIK CharacterIKController;


    // Use this for initialization
    void Start() {
        //Creates weapon array
        Weapon = new GameObject[playerWeaponLimit];

        //Load Weapon List from Scriptable Object
        WeaponList = WeaponListObject.weapons;

        ActualHealth = Health;
        ActualStamina = Stamina;
        ActualStaminaRecover = StaminaRecoverLimit;

        Audio = GetComponent<AudioSource>() as AudioSource;
        charAnimator = GetComponent<Animator>();

        DeactivateCompass();
       
        charController = GetComponent<PrTopDownCharController>();
        AimTarget = charController.AimFinalPos;
        HUDHealthBar.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        CreateNoiseTrigger();

        //Weapon Instantiate and initialization
        if (InitialWeapons.Length > 0)
        {
            InstantiateWeapons();
            
            Armed = true;
        }
        else
        {
            Armed = false;
        }

        Canvases = GameObject.FindGameObjectsWithTag("Canvas");
        if (Canvases.Length > 0)
        {
            foreach (GameObject C in Canvases)
                UnparentTransforms(C.transform);
        }

        if (alwaysAim)
        {
            Aiming = true;
            charAnimator.SetBool("Aiming", true);
        }

        InitializeHUD();

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
        if (useQuickReload && quickReloadPanel && quickReloadMarker && quickReloadZone)
        {
            QuickReloadActive(false);
        }

        //Update players on Enemies
        AIUpdatePlayerCount();

        //Update grenades HUD
        if (HUDGrenadesCount)
            HUDGrenadesCount.GetComponent<Text>().text = grenadesCount.ToString();
    }

    public void AIUpdatePlayerCount()
    {
        //Send Message to all AI
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            foreach (GameObject e in enemies)
            {
                e.SendMessage("FindPlayers", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void UnparentTransforms(Transform Target)
    {
        Target.SetParent(null);
    }

    void CreateNoiseTrigger()
    {
        GameObject NoiseGO = new GameObject();
        NoiseGO.name = "Player Noise Trigger";
        NoiseGO.AddComponent<SphereCollider>();
        NoiseTrigger = NoiseGO.GetComponent<SphereCollider>();
        NoiseTrigger.GetComponent<SphereCollider>().isTrigger = true;
        NoiseTrigger.transform.parent = this.transform;
        NoiseTrigger.transform.position = transform.position + new Vector3(0,1,0);
        NoiseTrigger.gameObject.tag = "Noise";
    }

    void InstantiateWeapons()
    {
       
        foreach (PrWeapon Weap in InitialWeapons)
        {
            int weapInt = 0;
            //Debug.Log("Weapon to instance = " + Weap);

            foreach (GameObject weap in WeaponList)
            {
                
                if (Weap.gameObject.name == weap.name)
                {
                    //Debug.Log("Weapon to pickup = " + weap + " " + weapInt);
                    PickupWeapon(weapInt);
                }
                    
                else
                    weapInt += 1;
            }

        }

    }

    public void FootStep()
    {
        if (Footsteps.Length > 0 && Time.time >= (LastFootStepTime + FootStepsRate))
        {
            int FootStepAudio = 0;

            if (Footsteps.Length > 1)
            {
                FootStepAudio = Random.Range(0, Footsteps.Length);
            }

            float FootStepVolume = charAnimator.GetFloat("Speed") * GeneralFootStepsVolume;
            if (Aiming)
                FootStepVolume *= 0.5f;

            Audio.PlayOneShot(Footsteps[FootStepAudio], FootStepVolume);

            MakeNoise(FootStepVolume * 10f);
            LastFootStepTime = Time.time;
        }
    }
    
    public void RollSound(AudioClip SFX)
    {
        if (SFX != null)
            Audio.PlayOneShot(SFX);
    }

    //Test Function to Assign a Target to the Compass
    public void TestAssignTarget()
    {
        ActivateCompass(GameObject.Find("PickupExample_RocketLauncher"));

    }

    public void ActivateCompass(GameObject Target)
    {
        Compass.SetActive(true);
        CompassActive = true;
        CompassTarget = Target.transform;

    }

    public void DeactivateCompass()
    {
        CompassActive = false;
        Compass.SetActive(false);

    }

    public void MakeNoise(float volume)
    {
        actualNoise = volume;
    }

    public void LoadGrenades(int quantity)
    {
        grenadesCount += quantity;
        if (grenadesCount > maxGrenades)
        {
            grenadesCount = maxGrenades;
        }
        if (HUDGrenadesCount)
            HUDGrenadesCount.GetComponent<Text>().text = grenadesCount.ToString();
    }

    void ThrowG()
    {
        GameObject Grenade = Instantiate(grenadesPrefab, WeaponL.position, Quaternion.LookRotation(transform.forward)) as GameObject;
        Grenade.GetComponent<PrBullet>().team = team;
        Vector3 grenadeForce = this.transform.forward * Grenade.GetComponent<PrBullet>().BulletSpeed * 10 + Vector3.up * 2000;
        float targetDistance = Vector3.Distance(AimTarget.transform.position, transform.position);
        Vector3 finalGrenadeForce = grenadeForce * (targetDistance / 20.0f);
        float MaxForce = ThrowGrenadeMaxForce * 17;
        finalGrenadeForce.x = Mathf.Clamp(finalGrenadeForce.x, -MaxForce, MaxForce);
        finalGrenadeForce.y = Mathf.Clamp(finalGrenadeForce.y, -MaxForce, MaxForce);
        finalGrenadeForce.z = Mathf.Clamp(finalGrenadeForce.z, -MaxForce, MaxForce);
        //Debug.Log(finalGrenadeForce);
        Grenade.GetComponent<Rigidbody>().AddForce(finalGrenadeForce);
        Grenade.GetComponent<Rigidbody>().AddRelativeTorque(Grenade.transform.forward * 50f, ForceMode.Impulse);

        grenadesCount -= 1;
        if (HUDGrenadesCount)
            HUDGrenadesCount.GetComponent<Text>().text = grenadesCount.ToString();
    }

    void EndThrow()
    {
        isThrowing = false;
        EnableArmIK(true);
    }


    void EnableArmIK(bool active)
    {
        if (CharacterIKController && useArmIK )
            if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().useIK)
                CharacterIKController.ikActive = active;
            else
                CharacterIKController.ikActive = false;
    }

    
    void WeaponReload()
    {
        if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().ActualBullets < Weapon[ActiveWeapon].GetComponent<PrWeapon>().Bullets && Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reloading == false &&
            Weapon[ActiveWeapon].GetComponent<PrWeapon>().ActualClips > 0)
        {
            Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = false;
            Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reload();
            //charAnimator.SetBool("Reloading", true);
            CanShoot = false;
            //Disable Arm IK
            EnableArmIK(false);

            if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().useQuickReload)
            {
                QuickReloadActive(true);
            }
        }
    }

    void QuickReloadActive(bool state)
    {
        //Debug.Log("QuickReloading " + state );
        quickReloadPanel.SetActive(state);
    }


    public void SetPlayerColors(int mode, int team, PrPlayerSettings playerSettings)
    {
        if (mode == 0)
        {
            //Singleplayer Colors
            if (playerSettings.useSinglePlayerColor)
            {
                foreach (Renderer rend in MeshRenderers)
                {
                    rend.material.SetColor("_MaskedColorA", playerSettings.singlePlayerColor);
                }
            }

        }
        else if (mode == 1)
        {
            //Deathmatch Colors
            foreach (Renderer rend in MeshRenderers)
            {
                rend.material.SetColor("_MaskedColorA", playerSettings.playerColor[team]);
            }
        }
        else if (mode == 2)
        {
            //Coop Colors
            if (playerSettings.useCoopPlayerColor)
            {
                foreach (Renderer rend in MeshRenderers)
                {
                    rend.material.SetColor("_MaskedColorA", playerSettings.coopPlayerColor[team]);
                }
            }
        }
        else if (mode == 3)
        {
            //Team DeathMatch Colors
            foreach (Renderer rend in MeshRenderers)
            {
                Debug.Log(team);
                rend.material.SetColor("_MaskedColorA", playerSettings.teamColor[team]);

            }

        }
    }

    void EndMelee()
    {
        charController.useRootMotion = false;
    }

    void MeleeEvent()
    {
        //this event comes from animation, the exact moment of HIT
        Weapon[ActiveWeapon].GetComponent<PrWeapon>().AttackMelee();
       
    }
    // Update is called once per frame
    void Update () {

        if (Damaged && MeshRenderers.Length > 0)
        {
            DamagedTimer = Mathf.Lerp(DamagedTimer, 0.0f, Time.deltaTime * 10);

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

           if (HUDDamageFullScreen)
                HUDDamageFullScreen.GetComponent<UnityEngine.UI.Image>().color = new Vector4(1, 1, 1, DamagedTimer * 0.5f);
            
        }

        if (!isDead)
		{
            //Calculates Temperature and damage
            ApplyTemperature();

            // Equip Weapon
            if (Input.GetButtonUp(charController.playerCtrlMap[6]) && Aiming == false && charController.Sprinting == false && charController.Rolling == false && isThrowing == false)
            {
                Weapon[ActiveWeapon].GetComponent<PrWeapon>().CancelReload();

                if (Armed)
                    Armed = false;
                else
                    Armed = true;
                EquipWeapon(Armed);
            }
            // Throw grenades
            if (Input.GetButton(charController.playerCtrlMap[14]))
            {
                if (grenadesCount > 0 && isThrowing == false && charController.Rolling == false && charController.Sprinting == false && Armed)
                {
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().CancelReload();
                    isThrowing = true;
                    charAnimator.SetTrigger("ThrowG");
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = false;
                    EnableArmIK(false);

                }
            }
            // Shoot Weapons
            if ( Input.GetAxis(charController.playerCtrlMap[4]) >= 0.5f || Input.GetButton(charController.playerCtrlMap[15]) )
            {
                if (charController.Rolling == false && charController.Sprinting == false && isThrowing == false)
                {
                    if (CanShoot && Weapon[ActiveWeapon] != null && Time.time >= (LastFireTimer + FireRateTimer))
                    {
                        //Melee Weapon
                        if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Type == global::PrWeapon.WT.Melee) 
                        {
                            LastFireTimer = Time.time;
                            charAnimator.SetTrigger("AttackMelee");
                            charAnimator.SetInteger("MeleeType", Random.Range(0, 2));
                            charController.useRootMotion = true;
                        }
                        //Ranged Weapon
                        else 
                        {
                            if (Aiming)
                            {
                                LastFireTimer = Time.time;
                                Weapon[ActiveWeapon].GetComponent<PrWeapon>().Shoot();
                                if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reloading == false)
                                {
                                    if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().ActualBullets > 0)
                                        charAnimator.SetTrigger("Shoot");
                                    else
                                        WeaponReload();
                                }
                            }
                        }
                    }
                }
			}
			// Reload Weapon
			if (Input.GetButtonDown(charController.playerCtrlMap[5]))
			{
                if (charController.Rolling == false && charController.Sprinting == false && isThrowing == false)
                {
                   WeaponReload();
                }
                if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reloading == true)
                {
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().TryQuickReload();
                }
			}
            // Aim
			if (Input.GetButton(charController.playerCtrlMap[8]) || Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[2])) > 0.3f || Mathf.Abs( Input.GetAxis(charController.playerCtrlMap[3])) > 0.3f)
			{
                if (charController.Rolling == false && charController.Sprinting == false && !UsingObject && Armed && isThrowing == false)
                {
                    if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Type != global::PrWeapon.WT.Melee)
                    {
                        if (!alwaysAim)
                        {
                            Aiming = true;
                            if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reloading == true || isThrowing == true)
                                Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = false;
                            else
                                Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = true;
                            charAnimator.SetBool("RunStop", false);
                            charAnimator.SetBool("Aiming", true);
                        }
                       
                    }
                }
                
            }
            //Stop Aiming
			else if (Input.GetButtonUp(charController.playerCtrlMap[8]) || Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[2])) < 0.3f || Mathf.Abs(Input.GetAxis(charController.playerCtrlMap[3])) < 0.3f)
			{
                if (!alwaysAim)
                {
                    Aiming = false;
                    charAnimator.SetBool("Aiming", false);
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = false;
                }
            }
            //USE
			if (Input.GetButtonDown(charController.playerCtrlMap[11]) && UsableObject && !UsingObject && isThrowing == false)
			{
                if (UsableObject.GetComponent<PrUsableDevice>().IsEnabled)
                {
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().CancelReload();
                    StartUsingGeneric("Use");

                    UsableObject.GetComponent<PrUsableDevice>().User = this.gameObject;
                    UsableObject.GetComponent<PrUsableDevice>().Use();
                }

            }
            //Pickup
            else if (Input.GetButtonDown(charController.playerCtrlMap[11]) && !UsableObject && !UsingObject && PickupObj && isThrowing == false)
            {
                Weapon[ActiveWeapon].GetComponent<PrWeapon>().CancelReload();

                StartUsingGeneric("Pickup");

                PickupItem();


            }
            // Change Weapon
            if (Input.GetButtonDown(charController.playerCtrlMap[13]) || Input.GetAxis(charController.playerCtrlMap[9]) >= 0.5f || Input.GetAxis(charController.playerCtrlMap[16]) != 0f )
            {
                
                if (isThrowing == false && Time.time >= lastWeaponChange + 0.25f && Armed)
                {
                    Weapon[ActiveWeapon].GetComponent<PrWeapon>().CancelReload();
                    ChangeWeapon();
                }
                    
            }

            if (alwaysAim)
            {
                if (!UsingObject && !charController.Sprinting)
                    Aiming = true;
                else
                    Aiming = false;
            }

            if (HUDUseHelper && UsableObject)
            {
                HUDUseHelper.transform.rotation = Quaternion.identity;
            }

            

            if (ActualStaminaRecover >= StaminaRecoverLimit)
            {
                if (UsingStamina)
                {
                    if (ActualStamina > 0.05f)
                        ActualStamina -= Time.deltaTime;
                    else if (ActualStamina > 0.0f)
                    {
                        ActualStamina = 0.0f;
                        ActualStaminaRecover = 0.0f;
                    }
                      

                }
                else if (!UsingStamina)
                {
                    if (ActualStamina < Stamina)
                        ActualStamina += Time.deltaTime * StaminaRecoverSpeed;
                    else
                        ActualStamina = Stamina;
                }
            }
            else if (ActualStaminaRecover < StaminaRecoverLimit)
            {
                ActualStaminaRecover += Time.deltaTime;
            }
            

            HUDStaminaBar.GetComponent<RectTransform>().localScale = new Vector3((1.0f / Stamina) *  ActualStamina, 1.0f, 1.0f);

            

            if (UsingObject && UsableObject)
            {
                Quaternion EndRotation = Quaternion.LookRotation(UsableObject.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, EndRotation, Time.deltaTime * 5);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            }

            //Noise Manager
            if (actualNoise > 0.0f)
            {
                actualNoise -= Time.deltaTime * noiseDecaySpeed;
                NoiseTrigger.radius = actualNoise;
            }

        }
        
    }

    void LateUpdate()
    {
        if (CompassActive)
        {
            if (CompassTarget)
            {
                Compass.transform.LookAt(CompassTarget.position);
                CompassDistance.text = "" + (Mathf.RoundToInt(Vector3.Distance(CompassTarget.position, transform.position))) + " Mts";

                if (Vector3.Distance(CompassTarget.position, transform.position) <= 2.0f)
                {
                    CompassTarget = null;
                    DeactivateCompass();
                }
            }
            else
            {
                DeactivateCompass();
            }

        }
        
        if (aimingIK)
        {
            if (Aiming && !Weapon[ActiveWeapon].GetComponent<PrWeapon>().Reloading && !isThrowing && !charController.Rolling && !charController.Jumping && !charController.Sprinting)
            {
                WeaponR.parent.transform.LookAt(AimTarget.position, Vector3.up) ;
            }
        }
    }

    void EquipWeapon(bool bArmed)
    {
        charAnimator.SetBool("Armed", bArmed);
        Weapon[ActiveWeapon].SetActive(bArmed);
        Weapon[ActiveWeapon].GetComponent<PrWeapon>().UpdateWeaponGUI(HUDWeaponPicture);

        if (!bArmed)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 0.0f);
            int PistoActlLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistoActlLayer, 0.0f);
            int RifleActlLayer = charAnimator.GetLayerIndex("RifleActions");
            charAnimator.SetLayerWeight(RifleActlLayer, 0.0f);
            int PartialActions = charAnimator.GetLayerIndex("PartialActions");
            charAnimator.SetLayerWeight(PartialActions, 0.0f);

            if (CharacterIKController)
                CharacterIKController.enabled = false;
        }
        else
        {
            if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Type == global::PrWeapon.WT.Pistol)
            {
                int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
                charAnimator.SetLayerWeight(PistolLayer, 1.0f);
                int PistoActlLayer = charAnimator.GetLayerIndex("PistolActions");
                charAnimator.SetLayerWeight(PistoActlLayer, 1.0f);
                int RifleActlLayer = charAnimator.GetLayerIndex("RifleActions");
                charAnimator.SetLayerWeight(RifleActlLayer, 0.0f);
            }
            else if (Weapon[ActiveWeapon].GetComponent<PrWeapon>().Type == global::PrWeapon.WT.Rifle)
            {
                int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
                charAnimator.SetLayerWeight(PistolLayer, 0.0f);
                int PistoActlLayer = charAnimator.GetLayerIndex("PistolActions");
                charAnimator.SetLayerWeight(PistoActlLayer, 0.0f);
                int RifleActlLayer = charAnimator.GetLayerIndex("RifleActions");
                charAnimator.SetLayerWeight(RifleActlLayer, 1.0f);
            }

            int PartAct = charAnimator.GetLayerIndex("PartialActions");
            charAnimator.SetLayerWeight(PartAct, 1.0f);

            if (CharacterIKController)
                CharacterIKController.enabled = true;
        }

        EnableArmIK(bArmed);

    }

    void StartUsingGeneric(string Type)
    {
        Aiming = false;
        UsingObject = true;

        charController.m_CanMove = false;
        charAnimator.SetTrigger(Type);

        Weapon[ActiveWeapon].GetComponent<PrWeapon>().LaserSight.enabled = false;

        EnableArmIK(false);
    }

    void PickupItem()
    {
        transform.rotation = Quaternion.LookRotation(PickupObj.transform.position - transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            
        PickupObj.SendMessage("PickupObjectNow", ActiveWeapon);
    }

    public void SpawnTeleportFX()
    {
        Damaged = true;
        DamagedTimer = 1.0f;
    }

    public void PickupWeapon(int WeaponType)
    {
        GameObject NewWeapon = Instantiate(WeaponList[WeaponType], WeaponR.position, WeaponR.rotation) as GameObject;
        NewWeapon.transform.parent = WeaponR.transform;
        NewWeapon.transform.localRotation = Quaternion.Euler(90, 0, 0);
        NewWeapon.name = "Player_" + NewWeapon.GetComponent<PrWeapon>().WeaponName;

        //New multi weapon system
        bool replaceWeapon = true;

        for (int i = 0; i < playerWeaponLimit ; i++)
        {
            if (Weapon[i] == null)
            {
                //Debug.Log(i + " " + NewWeapon.name);

                Weapon[i] = NewWeapon;
                replaceWeapon = false;
                
                if (ActiveWeapon != i)
                {
                    // ChangeWeapon();
                    ChangeToWeapon(i);
                }
                break;
            }

        }
        if (replaceWeapon)
        {
            //Debug.Log("Replacing weapon" + Weapon[ActiveWeapon].name + " using " + NewWeapon.name);
            DestroyImmediate(Weapon[ActiveWeapon]);
            Weapon[ActiveWeapon] = NewWeapon;

        }

        InitializeWeapons();
    }

    public void PickupKey(int KeyType)
    {
        if (KeyType == 0)
            BlueKeys += 1;
        else if (KeyType == 1)
            YellowKeys += 1;
        else if (KeyType == 2)
            RedKeys += 1;
        else if (KeyType == 3)
            FullKeys += 1;
    }

    void InitializeWeapons()
    {
        PrWeapon ActualW = Weapon[ActiveWeapon].GetComponent<PrWeapon>();
        Weapon[ActiveWeapon].SetActive(true);
        HUDWeaponPicture.GetComponent<UnityEngine.UI.Image>().sprite = ActualW.WeaponPicture;
        ActualW.ShootTarget = AimTarget;
        ActualW.Player = this.gameObject;
        FireRateTimer = ActualW.FireRate;

        ActualW.HUDWeaponBullets = HUDWeaponBullets;
        ActualW.HUDWeaponBulletsBar = HUDWeaponBulletsBar;
        ActualW.HUDWeaponClips = HUDWeaponClips;

        useQuickReload = ActualW.useQuickReload;

        ActualW.HUDquickRelaodMarker = quickReloadMarker;
        ActualW.HUDquickRelaodZone = quickReloadZone;
        ActualW.SetupQuickReload();
        
        //ArmIK
        if (useArmIK)
        {
            if (ActualW.gameObject.transform.Find("ArmIK"))
            {
                ArmIKTarget = ActualW.gameObject.transform.Find("ArmIK");
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
                    CharacterIKController.ikActive = true;
                }

            }
            else
            {
                if (CharacterIKController != null)
                    CharacterIKController.ikActive = false;
            }
        }
        

        ActualW.Audio = WeaponR.GetComponent<AudioSource>();

        if (ActualW.Type == global::PrWeapon.WT.Pistol)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 1.0f);
            int PistolActLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistolActLayer, 1.0f);
            charAnimator.SetBool("Armed", true);
        }
        else if (ActualW.Type == global::PrWeapon.WT.Rifle)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 0.0f);
            int PistolActLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistolActLayer, 0.0f);
            charAnimator.SetBool("Armed", true);
        }
        else if (ActualW.Type == global::PrWeapon.WT.Melee)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 0.0f);
            int PistolActLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistolActLayer, 0.0f);
            charAnimator.SetBool("Armed", false);
        }
        else if (ActualW.Type == global::PrWeapon.WT.Laser)
        {
            int PistolLayer = charAnimator.GetLayerIndex("PistolLyr");
            charAnimator.SetLayerWeight(PistolLayer, 0.0f);
            int PistolActLayer = charAnimator.GetLayerIndex("PistolActions");
            charAnimator.SetLayerWeight(PistolActLayer, 0.0f);
            charAnimator.SetBool("Armed", true);
        }
    }

    void InitializeHUD()
    {
        if (HUDDamageFullScreen)
            HUDDamageFullScreen.GetComponent<UnityEngine.UI.Image>().color = new Vector4(1, 1, 1, 0);

        if (charController.playerNmb > 0)
        {
            RectTransform HUDHealtRect = HUDHealthBar.transform.parent.parent.GetComponent<RectTransform>();
            RectTransform HUDWeaponRect = HUDWeaponBulletsBar.transform.parent.GetComponent<RectTransform>();
            RectTransform HUDQuickReloadPanel = quickReloadPanel.transform.GetComponent<RectTransform>();

            //FOR MULTIPLAYER PURPOSES
            float XPos = HUDHealtRect.localPosition.x;
            float YPos = HUDHealtRect.localPosition.y;

            float XWeapPos = HUDWeaponRect.localPosition.x;
            float YWeapPos = HUDWeaponRect.localPosition.y;

            float XQRPos = HUDQuickReloadPanel.localPosition.x;
            float YQRPos = HUDQuickReloadPanel.localPosition.y;

            //Debug.Log(XPos);

            if (splitScreen)
            {

                //Scale HUDS
                HUDHealtRect.localScale *= splitScaleFactor;
                HUDWeaponRect.localScale *= splitScaleFactor;

                //Apply Split Screen Margins
                XPos = XPos - splitMargins.x;
                YPos = YPos - splitMargins.y;
                XWeapPos = XWeapPos - splitMargins.x;
                YWeapPos = YWeapPos - splitMargins.y;
                XQRPos = XQRPos - splitMargins.x;
                YQRPos = YQRPos - splitMargins.y;

                //Damage Rect
                Vector3 damageScale = HUDDamageFullScreen.GetComponent<RectTransform>().localScale * 1.01f;
                Vector3 damagePos = HUDDamageFullScreen.GetComponent<RectTransform>().localPosition;

                if (totalPlayers == 2)
                {
                    //Set Damage HUD size
                    HUDDamageFullScreen.GetComponent<RectTransform>().localScale = 
                        new Vector3(damageScale.x * 0.5f, damageScale.y, damageScale.z);

                    if (charController.playerNmb == 1)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos , YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos, YQRPos, 0);

                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos - new Vector3((splitOff.x * 0.5f), 0, 0);
                    }
                    else if (charController.playerNmb == 2)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos + splitOff.x, YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos + splitOff.x, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos + splitOff.x, YQRPos, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos + new Vector3((splitOff.x * 0.5f), 0, 0);
                    }
                }
                else if (totalPlayers == 3)
                {
                    //Set Damage HUD size
                    HUDDamageFullScreen.GetComponent<RectTransform>().localScale =
                        new Vector3(damageScale.x * 0.5f, damageScale.y * 0.5f, damageScale.z);

                    if (charController.playerNmb == 1)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos + (splitOff.x * 0.5f), YPos + splitOff.y, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos + (splitOff.x * 0.5f), YWeapPos + splitOff.y, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos + (splitOff.x * 0.5f), YQRPos + splitOff.y, 0);

                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos + new Vector3(0, splitOff.y * 0.5f, 0);
                    }
                    else if (charController.playerNmb == 2)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos, YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos, YQRPos, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos - new Vector3((splitOff.x * 0.5f), splitOff.y * 0.5f, 0);
                    }
                    else if (charController.playerNmb == 3)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos + splitOff.x, YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos + splitOff.x, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos + splitOff.x, YQRPos, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos + new Vector3((splitOff.x * 0.5f), -splitOff.y * 0.5f, 0);
                    }
                }
                else if (totalPlayers == 4)
                {
                    //Set Damage HUD size
                    HUDDamageFullScreen.GetComponent<RectTransform>().localScale =
                        new Vector3(damageScale.x * 0.5f, damageScale.y * 0.5f, damageScale.z);

                    if (charController.playerNmb == 1)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos, YPos + splitOff.y, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos, YWeapPos + splitOff.y, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos, YQRPos + splitOff.y, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos - new Vector3((splitOff.x * 0.5f), splitOff.y * 0.5f, 0);
                    }

                    else if (charController.playerNmb == 2)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos + splitOff.x, YPos + splitOff.y, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos + splitOff.x, YWeapPos + splitOff.y, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos + splitOff.x, YQRPos + splitOff.y, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos + new Vector3((splitOff.x * 0.5f), splitOff.y * 0.5f, 0);
                    }

                    else if (charController.playerNmb == 3)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos, YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos, YQRPos, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos - new Vector3((splitOff.x * 0.5f), splitOff.y * 0.5f, 0);
                    }

                    else if (charController.playerNmb == 4)
                    {
                        HUDHealtRect.localPosition = new Vector3(XPos + splitOff.x, YPos, 0);
                        HUDWeaponRect.localPosition = new Vector3(XWeapPos + splitOff.x, YWeapPos, 0);
                        HUDQuickReloadPanel.localPosition = new Vector3(XQRPos + splitOff.x, YQRPos, 0);
                        //Damage Position
                        HUDDamageFullScreen.GetComponent<RectTransform>().localPosition = damagePos + new Vector3((splitOff.x * 0.5f), -splitOff.y * 0.5f, 0);
                    }

                }
            }
            else
            {
                HUDHealthBar.transform.parent.parent.GetComponent<RectTransform>().localPosition = new Vector3(XPos + (multiplayerHUDOffset * (charController.playerNmb - 1)), YPos, 0);

                YPos = HUDWeaponBulletsBar.transform.parent.GetComponent<RectTransform>().localPosition.y;
                XPos = HUDWeaponBulletsBar.transform.parent.GetComponent<RectTransform>().localPosition.x;

                HUDWeaponBulletsBar.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(XPos + (multiplayerHUDOffset * (charController.playerNmb - 1)), YPos, 0);

                YPos = HUDQuickReloadPanel.transform.GetComponent<RectTransform>().localPosition.y;
                XPos = HUDQuickReloadPanel.transform.GetComponent<RectTransform>().localPosition.x;

                HUDQuickReloadPanel.transform.GetComponent<RectTransform>().localPosition = new Vector3(XPos + (multiplayerHUDOffset * (charController.playerNmb - 1)), YPos, 0);
            }

            //SET HUD COLOR ACCORDING TO PLAYER COLOR
            if (HUDColorBar)
                HUDColorBar.GetComponent<UnityEngine.UI.Image>().color = charController.playerSettings.playerColor[charController.playerNmb - 1] * 0.625f;
        }
    }

    public void SetSplitScreen(bool active, int tPlayers)
    {
        splitScreen = active;
        totalPlayers = tPlayers;
    }

    void ChangeToWeapon(int weaponInt)
    {
        lastWeaponChange = Time.time;

        int nextWeapon = weaponInt;

        //Debug.Log("Changing Weapon " + Weapon[ActiveWeapon]);

        //New Multiple weapon system
        if (Weapon[nextWeapon] != null)
        {
            //Debug.Log("Testing");
            foreach (GameObject i in Weapon)
            {
                if (i != null)
                {
                    i.GetComponent<PrWeapon>().LaserSight.enabled = false;
                    i.SetActive(false);
                }
                //Debug.Log("Deactivating Weapon " + Weapon[ActiveWeapon]);
            }
            //Debug.Log(ActiveWeapon + " " + nextWeapon);
 
            ActiveWeapon = nextWeapon;
            Weapon[ActiveWeapon].SetActive(true);

            InitializeWeapons();
            Weapon[ActiveWeapon].GetComponent<PrWeapon>().UpdateWeaponGUI(HUDWeaponPicture);


        }
    }

    void ChangeWeapon()
    {
        lastWeaponChange = Time.time;

        int nextWeapon = ActiveWeapon + 1;
        if (nextWeapon >= playerWeaponLimit)
            nextWeapon = 0;

        //New Multiple weapon system
        if (Weapon[nextWeapon] != null)
        {
            //Debug.Log(ActiveWeapon + " " + nextWeapon);
            foreach (GameObject i in Weapon)
            {
                if (i != null)
                {
                    i.GetComponent<PrWeapon>().LaserSight.enabled = false;
                    i.SetActive(false);
                }
                //Debug.Log("Deactivating Weapon " + Weapon[ActiveWeapon]);
            }
                        
            ActiveWeapon = nextWeapon;

            Weapon[ActiveWeapon].SetActive(true);

            InitializeWeapons();
            Weapon[ActiveWeapon].GetComponent<PrWeapon>().UpdateWeaponGUI(HUDWeaponPicture);


        }
        else
        {
            for (int i = nextWeapon; i < playerWeaponLimit; i++)
            {
                if (Weapon[i] != null)
                {
                    ActiveWeapon = i - 1;
                    ChangeWeapon();
                    break;
                }
            }
            
            ActiveWeapon = playerWeaponLimit - 1;
            //Debug.Log(playerWeaponLimit);
            ChangeWeapon();
            
        }
    }

	public void StopUse()
	{
		charController.m_CanMove = true;
		charAnimator.SetTrigger("StopUse");
        UsingObject = false;

        EnableArmIK(true);
       
    }
    public void EndPickup()
    {
        charController.m_CanMove = true;
        UsingObject = false;

        EnableArmIK(true);
    }

    public void PlayerTeam(int enTeam)
    {
        enemyTeam = enTeam;
        if (enemyTeam == team)
        {
            enemyTeam += 1;
            enemyTeam *= -1;
        }
        //Debug.Log("EnemyTeam " + enemyTeam);
    }

    public void BulletPos(Vector3 BulletPosition)
    {
        LastHitPos = BulletPosition;
        LastHitPos.y = 0;
    }

    public void SetNewSpeed(float speedFactor)
    {
        charController.m_MoveSpeedSpecialModifier = speedFactor;
    }

    public void SetHealth(int HealthInt)
    {
        ActualHealth = HealthInt;
        //Debug.Log(ActualHealth);
        
        if (ActualHealth > 1)
        {
            HUDHealthBar.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Clamp((1.0f / Health) * ActualHealth,0.1f,1.0f) , 1.0f, 1.0f);
        }
        else
        {
            HUDHealthBar.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 1.0f, 1.0f);
        }
     
    }

    void ApplyTemperature()
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

    void applyTemperatureDamage()
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

    void ApplyDamagePassive(int damage)
    {
        if (!isDead)
        {
            SetHealth(ActualHealth - damage);

            if (ActualHealth <= 0)
            {
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

    public void ApplyDamage(int Damage)
	{
        if (ActualHealth > 0)
		{
            //Here you can put some Damage Behaviour if you want
            SetHealth(ActualHealth - Damage);

            Damaged = true;
            DamagedTimer = 1.0f;

            if (actualSplatVFX)
            {
                actualSplatVFX.transform.LookAt(LastHitPos);
                actualSplatVFX.Splat();
            }

            if (ActualHealth <= 0)
            {
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;
                if (Damage >= damageThreshold)
                    explosiveDeath = true;
                Die(false);
            }
                
        }
		
	}

    public void ApplyDamageNoVFX(int Damage)
    {
        if (ActualHealth > 0)
        {
            //Here you can put some Damage Behaviour if you want
            SetHealth(ActualHealth - Damage);

            Damaged = true;
            DamagedTimer = 1.0f;

            if (ActualHealth <= 0)
            {
                if (actualSplatVFX)
                    actualSplatVFX.transform.parent = null;
                if (Damage >= damageThreshold)
                    explosiveDeath = true;
                Die(true);
            }

        }

    }


    public void Die(bool temperatureDeath)
	{
        EnableArmIK(false);
		isDead = true;
		charAnimator.SetBool("Dead", true);

        charController.m_isDead = true;
        Weapon[ActiveWeapon].GetComponent<PrWeapon>().TurnOffLaser();

        //Set invisible to Bullets
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        
        this.tag = "Untagged";
        charController.playerSelection.enabled = false;

        DestroyHUD();

        if (useRagdollDeath)
        {
            Vector3 ragdollDirection = transform.position - LastHitPos;
            ragdollDirection = ragdollDirection.normalized;
            if (!temperatureDeath)
                GetComponent<PrCharacterRagdoll>().SetForceToRagdoll(LastHitPos + new Vector3(0,1.5f,0), ragdollDirection * (ragdollForceFactor * Random.Range(0.8f,1.2f)));
        }

        //Send Message to Game script to notify Dead
        SendMessageUpwards("PlayerDied", charController.playerNmb, SendMessageOptions.DontRequireReceiver);
        SendMessageUpwards("NewFrag", enemyTeam, SendMessageOptions.DontRequireReceiver);

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
                    actualDeathVFX.transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);

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

        AIUpdatePlayerCount();

        Destroy(charController);
        Destroy(GetComponent<Collider>());
        //Destroy(this);
        

    }

    public void DestroyHUD()
    {
        //Destroy GUI
        Weapon[ActiveWeapon].GetComponent<PrWeapon>().updateHUD = false;
        if (HUDDamageFullScreen != null)
        {
            if (HUDDamageFullScreen.transform.parent.gameObject != null)
                Destroy(HUDDamageFullScreen.transform.parent.gameObject);
        }
        if (HUDWeaponPicture != null)
        {
            if (HUDWeaponPicture.transform.parent.parent.gameObject != null)
                Destroy(HUDWeaponPicture.transform.parent.parent.gameObject);
        }
    }

	public void EndReload()
	{
		CanShoot = true;
        charAnimator.SetBool("Reloading", false);
        if (useQuickReload && quickReloadPanel && quickReloadMarker && quickReloadZone)
        {
            QuickReloadActive(false);
        }
        EnableArmIK(true);
    }

	void OnTriggerStay(Collider other) {
        
        if (other.CompareTag("Usable") && UsableObject == null)
        {
                if (other.GetComponent<PrUsableDevice>().IsEnabled)
                    UsableObject = other.gameObject;
        }
        else if (other.CompareTag("Pickup") && PickupObj == null )
        {
            PickupObj = other.gameObject;
              
        }
        

    }
	
    

	void OnTriggerExit(Collider other)
	{
        
        if (other.CompareTag("Usable") && UsableObject != null)
        {
            UsableObject = null;
            HUDUseHelper.SetActive(false);
        }
        if (other.CompareTag("Pickup") && PickupObj != null)
        {
           
            PickupObj = null;
               
        }
        
	}
        
}
