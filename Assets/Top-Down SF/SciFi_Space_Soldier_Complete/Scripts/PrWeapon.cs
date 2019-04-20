using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrWeapon : MonoBehaviour {


    public string WeaponName = "Rifle";

    public enum WT
    {
        Pistol = 0, Rifle = 1, Minigun = 2, RocketLauncher = 3, Melee = 4, Laser = 5
    }

    public WT Type = WT.Rifle;
    public bool useIK = true;

    [Header("Melee Weapon")]
    public float MeleeRadius = 1.0f;
    public int meleeDamage = 1;
    private List<GameObject> meleeFinalTarget;

    [Header("Stats")]

    public int BulletsPerShoot = 1;
    public int BulletDamage = 20;
    public float tempModFactor = 0.0f;
    public float BulletSize = 1.0f;

    public float BulletSpeed = 1.0f;
    public float BulletAccel = 0.0f;

    public int Bullets = 10;
    [HideInInspector]
    public int ActualBullets = 0;

    public int Clips = 3;
    [HideInInspector]
    public int ActualClips = 0;

    public float ReloadTime = 1.0f;
    public bool playReloadAnim = true;
    private float ActualReloadTime = 0.0f;

    public float bulletTimeToLive = 3.0f;

    [HideInInspector]
    public bool Reloading = false;

    public float FireRate = 0.1f;
    public float AccDiv = 0.0f;

    public float radialAngleDirection = 0.0f;

    public float shootingNoise = 25f;

    [Header("Quick Reload")]
    public bool useQuickReload = true;
    public Vector2 HUDquickReloadTimes = new Vector2(0.5f, 0.7f);
    private bool quickReloadActive = false;

    [Header("References & VFX")]
    public float shootShakeFactor = 2.0f;
    public Transform ShootFXPos;
    public GameObject BulletPrefab;
    public GameObject ShootFXFLash;
    public Light ShootFXLight;
    public Renderer LaserSight;
    private PrTopDownCamera playerCamera;

    [Header("Laser Weapon Settings")]
    public GameObject laserBeamPrefab;
    private GameObject[] actualBeams;
    public float laserWidthFactor = 1.0f;
    public float laserLiveTime = 1.0f;
    public float warmingTime = 0.2f;
    public bool generatesBloodDamage = true;
    public GameObject warmingVFX;
    private GameObject actualWarmingVFX;
    public GameObject laserHitVFX;
    private GameObject[] actualLaserHits;

    [HideInInspector]
    public Transform ShootTarget;
    [HideInInspector]
    public GameObject Player;

    [Header("Sound FX")]
    public AudioClip[] ShootSFX;
    public AudioClip ReloadSFX;
    public AudioClip ShootEmptySFX;
    [HideInInspector]
    public AudioSource Audio;

    [Header("Autoaim")]
    public float AutoAimAngle = 7.5f;
    public float AutoAimDistance = 10.0f;

    private Vector3 EnemyTargetAuto = Vector3.zero;
    private Vector3 FinalTarget = Vector3.zero;

    //HUD
    [Header("HUD")]
    [HideInInspector]
    public bool updateHUD = true;

    public Sprite WeaponPicture;
    [HideInInspector]
    public GameObject HUDWeaponPicture;
    [HideInInspector]
    //public GameObject HUDWeaponName;
    //[HideInInspector]
    public GameObject HUDWeaponBullets;
    [HideInInspector]
    public GameObject HUDWeaponBulletsBar;
    [HideInInspector]
    public GameObject HUDWeaponClips;
    [HideInInspector]
    public GameObject HUDquickRelaodMarker;
    [HideInInspector]
    public GameObject HUDquickRelaodZone;


    //Object Pooling Manager
    public bool usePooling = true;
    private GameObject[] GameBullets;
    private GameObject BulletsParent;
    private int ActualGameBullet = 0;
    private GameObject Muzzle;

    [HideInInspector]
    public bool AIWeapon = false;
    [HideInInspector]
    public Transform AIEnemyTarget;

    [HideInInspector]
    public bool turretWeapon = false;

    [HideInInspector]
    public int team = 0;

    private void Awake()
    {
        ActualBullets = Bullets;
        ActualClips = Clips;

    }

    // Use this for initialization
    void Start()
    {
        Audio = transform.parent.GetComponent<AudioSource>();

       
        if (!AIWeapon)
        {
            HUDWeaponBullets.GetComponent<Text>().text = (ActualBullets / BulletsPerShoot).ToString();
            HUDWeaponClips.GetComponent<Text>().text = ActualClips.ToString();
            HUDWeaponBulletsBar.GetComponent<Image>().fillAmount = (1.0f / Bullets) * ActualBullets;
            HUDWeaponBulletsBar.GetComponent<RectTransform>().localScale = Vector3.one;

            team = Player.GetComponent<PrTopDownCharInventory>().team;
        }

        //Basic Object Pooling Initialization ONLY FOR RANGED WEAPONS
        if (Type == WT.Rifle || Type == WT.Pistol || Type == WT.Minigun || Type == WT.RocketLauncher)
        {
            if (usePooling)
            {
                GameBullets = new GameObject[Bullets * BulletsPerShoot];
                BulletsParent = new GameObject(WeaponName + "_Bullets");

                for (int i = 0; i < (Bullets * BulletsPerShoot); i++)
                {
                    GameBullets[i] = Instantiate(BulletPrefab, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                    GameBullets[i].SetActive(false);
                    GameBullets[i].name = WeaponName + "_Bullet_" + i.ToString();
                    GameBullets[i].transform.parent = BulletsParent.transform;

                    GameBullets[i].GetComponent<PrBullet>().team = team;
                    GameBullets[i].GetComponent<PrBullet>().usePooling = true;
                    GameBullets[i].GetComponent<PrBullet>().InitializePooling();
                }
            }
            
        }
        else if (Type == WT.Laser)
        {

            actualBeams = new GameObject[BulletsPerShoot];
            actualLaserHits = new GameObject[BulletsPerShoot];
            GameObject BulletsParent = new GameObject(WeaponName + "_Beams");

            //Laser Weapon Initialization
            for (int i = 0; i < BulletsPerShoot; i++)
            {
                actualBeams[i] = Instantiate(laserBeamPrefab, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                actualBeams[i].SetActive(false);
                actualBeams[i].name = WeaponName + "_Beam_" + i.ToString();
                actualBeams[i].transform.parent = BulletsParent.transform;
                actualBeams[i].GetComponent<PrWeaponLaserBeam>().InitializeLine(laserWidthFactor, ShootFXPos);

                actualLaserHits[i] = Instantiate(laserHitVFX, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
                actualLaserHits[i].SetActive(false);
                actualLaserHits[i].name = WeaponName + "_Beam_Hit_" + i.ToString();
                actualLaserHits[i].transform.parent = BulletsParent.transform;
            }

            if (turretWeapon)
            {
                ShootTarget = new GameObject("ShootTarget").transform;
                ShootTarget.SetParent(transform);
            }


        }
        else if (Type == WT.Melee)
        {
            //Melee Weapon Initialization
            /*
            HUDWeaponBullets.GetComponent<Text>().text = "";
            HUDWeaponClips.GetComponent<Text>().text = "";
            HUDWeaponBulletsBar.GetComponent<RectTransform>().localScale = Vector3.zero;*/
        }

        if (ShootFXFLash)
        {
            Muzzle = Instantiate(ShootFXFLash, ShootFXPos.position, ShootFXPos.rotation) as GameObject;
            Muzzle.transform.parent = ShootFXPos.transform;
            Muzzle.SetActive(false);
        }

        if (GameObject.Find("PlayerCamera") != null)
        {
            playerCamera = GameObject.Find("PlayerCamera").GetComponent<PrTopDownCamera>();

        }
        else if (GameObject.Find("MutiplayerCamera") != null)
        {
            playerCamera = GameObject.Find("MutiplayerCamera").GetComponent<PrTopDownMutiplayerCam>();
        }

        if (useQuickReload)
        {
            if (HUDquickReloadTimes[0] < 0.0f)
                HUDquickReloadTimes[0] = 0.0f;
            else if (HUDquickReloadTimes[0] >= 1.0f)
                HUDquickReloadTimes[0] = 0.98f;

            if (HUDquickReloadTimes[1] < 0.0f)
                HUDquickReloadTimes[1] = 0.1f;
            else if (HUDquickReloadTimes[1] >= 1.0f)
                HUDquickReloadTimes[1] = 0.99f;
        }
    }

    // Update is called once per frame
    void Update() {

        if (Reloading)
        {
            ActualReloadTime += Time.deltaTime;

            if (!AIWeapon && !turretWeapon && useQuickReload)
            {
                HUDquickRelaodMarker.GetComponent<RectTransform>().localPosition = new Vector3(ActualReloadTime * (46.0f / ReloadTime), 0, 0);

                if (ActualReloadTime >= (HUDquickReloadTimes[0] * ReloadTime) && ActualReloadTime <= (HUDquickReloadTimes[1] * ReloadTime))
                    quickReloadActive = true;
                else
                    quickReloadActive = false;
            }

            if (ActualReloadTime >= ReloadTime)
            {
                PositiveReload();
            }
        }


    }

    private void OnDestroy()
    {
        if (BulletsParent)
            Destroy(BulletsParent);
    }

    void PositiveReload()
    {
        Reloading = false;
        ActualReloadTime = 0.0f;
        SendMessageUpwards("EndReload", SendMessageOptions.DontRequireReceiver);

        WeaponEndReload();
    }

    public void SetupQuickReload()
    {
        HUDquickRelaodZone.GetComponent<RectTransform>().localPosition = new Vector3(HUDquickReloadTimes[0] * 46.0f, 0, 0);
        HUDquickRelaodZone.GetComponent<RectTransform>().localScale = new Vector3(HUDquickReloadTimes[1] - HUDquickReloadTimes[0], 1, 1);
    }

    public void TryQuickReload()
    {
        if (quickReloadActive)
        {
            PositiveReload();
            quickReloadActive = false;
        }
            
    }

    public void TurnOffLaser()
    {
        LaserSight.enabled = false;
    }

    void LateUpdate()
    {
        if (!AIWeapon)
        {
            LaserSight.transform.position = ShootFXPos.position;
            LaserSight.transform.LookAt(ShootTarget.position, Vector3.up);
        }
    }

    void WeaponEndReload()
    {
        ActualBullets = Bullets;
        UpdateWeaponGUI();
        
    }

    void UpdateWeaponGUI()
    {
        if (!AIWeapon && Type != WT.Melee && updateHUD)
        {
            HUDWeaponBullets.GetComponent<Text>().text = (ActualBullets / BulletsPerShoot).ToString();
            HUDWeaponClips.GetComponent<Text>().text = ActualClips.ToString();
            HUDWeaponBulletsBar.GetComponent<Image>().fillAmount =(1.0f / Bullets) * ActualBullets;
            //Debug.Log("Bullets = " + Bullets);
            //HUDWeaponBulletsBar.GetComponent<RectTransform>().localScale = new Vector3((1.0f / Bullets) * ActualBullets, 1.0f, 1.0f);

        }
    }

    public void UpdateWeaponGUI(GameObject weapPic)
    {
        if (!AIWeapon && Type != WT.Melee)
        {
            HUDWeaponBullets.GetComponent<Text>().text = (ActualBullets / BulletsPerShoot).ToString();
            HUDWeaponClips.GetComponent<Text>().text = ActualClips.ToString();
            HUDWeaponBulletsBar.GetComponent<Image>().fillAmount = (1.0f / Bullets) * ActualBullets;
            //HUDWeaponBulletsBar.GetComponent<RectTransform>().localScale = new Vector3((1.0f / Bullets) * ActualBullets, 1.0f, 1.0f);
            HUDWeaponPicture = weapPic;
            if (HUDWeaponPicture.GetComponentInChildren<Text>())
                HUDWeaponPicture.GetComponentInChildren<Text>().text = WeaponName;
        }
    }

    public void CancelReload()
    {
        Reloading = false;
        if (playReloadAnim)
            Player.GetComponent<Animator>().SetBool("Reloading", false);
        SendMessageUpwards("EndReload", SendMessageOptions.DontRequireReceiver);
        ActualReloadTime = 0.0f;
    }

	public void Reload()
	{
		if (ActualClips > 0 || Clips == -1)
		{
            
            if (!AIWeapon || !turretWeapon)
            {
                if (useQuickReload)
                    SendMessageUpwards("QuickReloadActive", true, SendMessageOptions.DontRequireReceiver);
                ActualClips -= 1;
            }
            
            if (playReloadAnim && !turretWeapon && !AIWeapon)
                Player.GetComponent<Animator>().SetBool("Reloading", true);
            Reloading = true;
            Audio.PlayOneShot(ReloadSFX);
            ActualReloadTime = 0.0f;
           
        }
	}

    public void AIReload()
    {
        SendMessageUpwards("StartReload", SendMessageOptions.DontRequireReceiver);
        Reloading = true;
        Audio.PlayOneShot(ReloadSFX);
        ActualReloadTime = 0.0f;
    }

    void AutoAim()
    {
        //Autoaim////////////////////////

        GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemys != null)
        {
            float BestDistance = 100.0f;

            foreach (GameObject Enemy in Enemys)
            {
                Vector3 EnemyPos = Enemy.transform.position;
                Vector3 EnemyDirection = EnemyPos - Player.transform.position;
                float EnemyDistance = EnemyDirection.magnitude;

                if (Vector3.Angle(Player.transform.forward, EnemyDirection) <= AutoAimAngle && EnemyDistance < AutoAimDistance)
                {
                    //
                    if (Enemy.GetComponent<PrEnemyAI>().actualState != PrEnemyAI.AIState.Dead)
                    {
                        if (EnemyDistance < BestDistance)
                        {
                            BestDistance = EnemyDistance;
                            EnemyTargetAuto = EnemyPos + new Vector3(0, 1, 0);
                        }
                    }
                   

                }
            }
        }

        if (EnemyTargetAuto != Vector3.zero)
        {
            FinalTarget = EnemyTargetAuto;
            ShootFXPos.transform.LookAt(FinalTarget);
        }
        else
        {
            ShootFXPos.transform.LookAt(ShootTarget.position);
            FinalTarget = ShootTarget.position;
        }

        //End of AutoAim
        /////////////////////////////////

    }

    void AIAutoAim()
    {
        //Autoaim////////////////////////

        Vector3 PlayerPos = AIEnemyTarget.position + new Vector3(0, 1.5f, 0);
        FinalTarget = PlayerPos;
        
      
    }

    public void PlayShootAudio()
    {
        if (ShootSFX.Length > 0)
        {
            int FootStepAudio = 0;

            if (ShootSFX.Length > 1)
            {
                FootStepAudio = Random.Range(0, ShootSFX.Length);
            }

            float RandomVolume = Random.Range(0.6f, 1.0f);

            Audio.PlayOneShot(ShootSFX[FootStepAudio], RandomVolume);

            if (!AIWeapon)
                Player.SendMessage("MakeNoise", shootingNoise);
           
        }
    }

    public void Shoot()
	{
        if (AIWeapon || turretWeapon)
        {
            AIAutoAim();
        }
        else
        {
            AutoAim();
        }

        if (ActualBullets > 0)
            PlayShootAudio();
        //else
        //    Audio.PlayOneShot(ShootEmptySFX);
        float angleStep = radialAngleDirection / BulletsPerShoot;
        float finalAngle = 0.0f; 

        for (int i = 0; i < BulletsPerShoot; i++)
		{
            
            float FinalAccuracyModX = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
            FinalAccuracyModX /= 100;

            float FinalAccuracyModY = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
            FinalAccuracyModY /= 100;

            float FinalAccuracyModZ = Random.Range(AccDiv, -AccDiv) * Vector3.Distance(Player.transform.position, FinalTarget);
            FinalAccuracyModZ /= 100;
          
            Vector3 FinalOrientation = FinalTarget + new Vector3(FinalAccuracyModX, FinalAccuracyModY, FinalAccuracyModZ);

			ShootFXPos.transform.LookAt(FinalOrientation);

            if (BulletsPerShoot > 1 && radialAngleDirection > 0.0f)
            {
                Quaternion aimLocalRot = Quaternion.Euler(0, finalAngle - (radialAngleDirection / 2) + (angleStep * 0.5f), 0);
                ShootFXPos.transform.rotation = ShootFXPos.transform.rotation * aimLocalRot;

                finalAngle += angleStep;
            }

            if (Type != WT.Laser && BulletPrefab && ShootFXPos && !Reloading)
            {
                if (ActualBullets > 0)
                {
                    GameObject Bullet;
                    if (usePooling)
                    {
                        //Object Pooling Method 
                        Bullet = GameBullets[ActualGameBullet];
                        Bullet.transform.position = ShootFXPos.position;
                        Bullet.transform.rotation = ShootFXPos.rotation;
                        Bullet.GetComponent<Rigidbody>().isKinematic = false;
                        Bullet.GetComponent<Collider>().enabled = true;
                        Bullet.GetComponent<PrBullet>().timeToLive = bulletTimeToLive;
                        Bullet.GetComponent<PrBullet>().ResetPooling();
                        Bullet.SetActive(true);
                        ActualGameBullet += 1;
                        if (ActualGameBullet >= GameBullets.Length)
                            ActualGameBullet = 0;
                    }
                    else
                    {
                        Bullet = Instantiate(BulletPrefab, ShootFXPos.position, ShootFXPos.rotation);
                        Bullet.GetComponent<PrBullet>().usePooling = false;
                        Bullet.SetActive(true);
                        Bullet.GetComponent<Rigidbody>().isKinematic = false;
                        Bullet.GetComponent<Collider>().enabled = true;
                        Bullet.GetComponent<PrBullet>().timeToLive = bulletTimeToLive;
                    }
                        

                    //Object Pooling VFX
                    Muzzle.transform.rotation = transform.rotation;
                    EmitParticles(Muzzle);

                    //Generic 
                    Bullet.GetComponent<PrBullet>().Damage = BulletDamage;
                    Bullet.GetComponent<PrBullet>().temperatureMod = tempModFactor;
                    Bullet.GetComponent<PrBullet>().BulletSpeed = BulletSpeed;
                    Bullet.GetComponent<PrBullet>().BulletAccel = BulletAccel;
                    if (usePooling)
                        Bullet.transform.localScale = Bullet.GetComponent<PrBullet>().OriginalScale * BulletSize;

                    ShootFXLight.GetComponent<PrLightAnimator>().AnimateLight(true);
                    ActualBullets -= 1;

                    if (playerCamera)
                    {
                        if (!AIWeapon)
                            playerCamera.Shake(shootShakeFactor, 0.2f);
                        else
                            playerCamera.Shake(shootShakeFactor * 0.5f, 0.2f);
                    }

                    if (ActualBullets == 0)
                        Reload();

                }

            }
            // Laser Shoot
            else if (Type == WT.Laser && actualBeams.Length != 0 && ShootFXPos && !Reloading)
            {
                bool useDefaultImpactFX = true;
                
                Vector3 HitPos = ShootTarget.position + new Vector3(0, 1.2f, 0);

                Vector3 hitNormal = ShootTarget.forward;


                if (ActualBullets > 0)
                {
                    
                    //Object Pooling Method 
                    GameObject Beam = actualBeams[ActualGameBullet];
                    Beam.transform.position = ShootFXPos.position;
                    Beam.transform.rotation = ShootFXPos.rotation;
                    Beam.SetActive(true);
                    
                    Beam.GetComponent<PrWeaponLaserBeam>().Activate(laserLiveTime);
                    //Shoot Beam
                    RaycastHit hit;

                    if (Physics.Raycast(ShootFXPos.position, ShootFXPos.forward, out hit))
                    {
                        GameObject target = hit.collider.gameObject;
                         HitPos = hit.point;
                        hitNormal = hit.normal;
                        Beam.GetComponent<PrWeaponLaserBeam>().SetPositions(ShootFXPos.position, HitPos);

                        if (hit.collider.tag == "Player" && target.GetComponent<PrTopDownCharInventory>().team != team)
                        {
                            target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("BulletPos", hit.point, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("ApplyTempMod", tempModFactor, SendMessageOptions.DontRequireReceiver);
                            
                            if (generatesBloodDamage)
                            {
                                target.SendMessage("ApplyDamage", BulletDamage, SendMessageOptions.DontRequireReceiver);
                                if (target.GetComponent<PrTopDownCharInventory>().DamageFX != null)
                                {
                                    Instantiate(target.GetComponent<PrTopDownCharInventory>().DamageFX, HitPos, Quaternion.LookRotation(hitNormal));
                                    useDefaultImpactFX = false;
                                }
                            }
                            else
                            {
                                target.SendMessage("ApplyDamageNoVFX", BulletDamage, SendMessageOptions.DontRequireReceiver);
                            }

                        }
                        else if (hit.collider.tag == "Enemy" && target.GetComponent<PrEnemyAI>().team != team)
                        {
                            target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("BulletPos", hit.point, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("ApplyTempMod", tempModFactor, SendMessageOptions.DontRequireReceiver);
                            
                            if (generatesBloodDamage)
                            {
                                target.SendMessage("ApplyDamage", BulletDamage, SendMessageOptions.DontRequireReceiver);
                                if (target.GetComponent<PrEnemyAI>().damageVFX != null)
                                {
                                    Instantiate(target.GetComponent<PrEnemyAI>().damageVFX, HitPos, Quaternion.LookRotation(hitNormal));
                                    useDefaultImpactFX = false;
                                }
                            }
                            else
                            {
                                target.SendMessage("ApplyDamageNoVFX", BulletDamage, SendMessageOptions.DontRequireReceiver);
                            }
                        }

                        else if (hit.collider.tag == "AIPlayer" && target.GetComponent<PrEnemyAI>().team != team)
                        {
                            target.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("BulletPos", hit.point, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("ApplyTempMod", tempModFactor, SendMessageOptions.DontRequireReceiver);
                            
                            if (generatesBloodDamage)
                            {
                                target.SendMessage("ApplyDamage", BulletDamage, SendMessageOptions.DontRequireReceiver);

                                if (target.GetComponent<PrEnemyAI>().damageVFX != null)
                                {
                                    Instantiate(target.GetComponent<PrEnemyAI>().damageVFX, HitPos, Quaternion.LookRotation(hitNormal));
                                    useDefaultImpactFX = false;
                                }
                            }
                            else
                            {
                                target.SendMessage("ApplyDamageNoVFX", BulletDamage, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        else if (hit.collider.tag == "Destroyable" && target.GetComponent<PrDestroyableActor>().team != team)
                        {
                            //Debug.Log("Bullet team = " + team + " Target Team = " + Target.GetComponent<PrDestroyableActor>().team);
                            target.SendMessage("BulletPos", hit.point, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("ApplyTempMod", tempModFactor, SendMessageOptions.DontRequireReceiver);
                            target.SendMessage("ApplyDamage", BulletDamage, SendMessageOptions.DontRequireReceiver);
                            if (target.GetComponent<Rigidbody>())
                            {
                                target.GetComponent<Rigidbody>().AddForceAtPosition(hitNormal * Random.Range(-200.0f,-400.0f), HitPos);
                            }
                        }
                    }

                    else
                    {
                        Beam.GetComponent<PrWeaponLaserBeam>().SetPositions(ShootFXPos.position, ShootTarget.position + new Vector3(0,1.2f,0));
                    }

                    //default Hit VFX
                    if (useDefaultImpactFX)
                    {
                        actualLaserHits[ActualGameBullet].SetActive(true);
                        actualLaserHits[ActualGameBullet].transform.position = HitPos;
                        actualLaserHits[ActualGameBullet].transform.rotation = Quaternion.LookRotation(hitNormal);
                        actualLaserHits[ActualGameBullet].GetComponent<ParticleSystem>().Play();
                    }

                    ActualGameBullet += 1;
                    //Object Pooling VFX
                    Muzzle.transform.rotation = transform.rotation;
                    EmitParticles(Muzzle);

                    if (ActualGameBullet >= actualBeams.Length)
                        ActualGameBullet = 0;

                    ShootFXLight.GetComponent<PrLightAnimator>().AnimateLight(true);
                    ActualBullets -= 1;

                    if (playerCamera)
                    {
                        if (!AIWeapon)
                            playerCamera.Shake(shootShakeFactor, 0.2f);
                        else
                            playerCamera.Shake(shootShakeFactor * 0.5f, 0.2f);
                    }

                    if (ActualBullets == 0)
                        Reload();

                }
            }

            UpdateWeaponGUI();

            EnemyTargetAuto = Vector3.zero;

            
        }
	}

    void EmitParticles(GameObject VFXEmiiter)
    {
        VFXEmiiter.SetActive(true);
        VFXEmiiter.GetComponent<ParticleSystem>().Play();
    }


    public void AIAttackMelee(Vector3 playerPos, GameObject targetGO)
    {
        PlayShootAudio();

        //Object Pooling VFX
        if (Muzzle)
        {
            EmitParticles(Muzzle);
        }
        if (ShootFXLight)
            ShootFXLight.GetComponent<PrLightAnimator>().AnimateLight(true);

        if (Vector3.Distance(playerPos + Vector3.up, ShootFXPos.position) <= MeleeRadius)
        {
            //Debug.Log("Hit Player Sucessfully");
            targetGO.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
            targetGO.SendMessage("BulletPos", ShootFXPos.position, SendMessageOptions.DontRequireReceiver);
            targetGO.SendMessage("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AttackMelee()
    {
        PlayShootAudio();

        //Object Pooling VFX
        if (Muzzle)
        {
            EmitParticles(Muzzle);
        }
        //Use Light
        if (ShootFXLight)
            ShootFXLight.GetComponent<PrLightAnimator>().AnimateLight(true);

        //Start Finding Enemy Target
        meleeFinalTarget = new List<GameObject>();

        GameObject[] EnemysTemp = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] PlayersTemp = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] Enemys = new GameObject[EnemysTemp.Length + PlayersTemp.Length];
        int t = 0;
        foreach (GameObject E in EnemysTemp)
        {
            Enemys[t] = E;
            t += 1;
        }
        foreach (GameObject E in PlayersTemp)
        {
            Enemys[t] = E;
            t += 1;
        }


        if (Enemys != null)
        {
            float BestDistance = 100.0f;

            foreach (GameObject Enemy in Enemys)
            {
                Vector3 EnemyPos = Enemy.transform.position;
                Vector3 EnemyDirection = EnemyPos - Player.transform.position;
                float EnemyDistance = EnemyDirection.magnitude;

                if (Vector3.Angle(Player.transform.forward, EnemyDirection) <= 90 && EnemyDistance < MeleeRadius)
                {
                    //
                    if (Enemy.GetComponent<PrEnemyAI>())
                    {
                        if (Enemy.GetComponent<PrEnemyAI>().actualState != PrEnemyAI.AIState.Dead && Enemy.GetComponent<PrEnemyAI>().team != team)
                        {
                            if (EnemyDistance < BestDistance)
                            {
                                BestDistance = EnemyDistance;
                                meleeFinalTarget.Add(Enemy);// = Enemy;
                            }

                        }
                    }
                    else if (Enemy.GetComponent<PrTopDownCharInventory>())
                    {
                        if (Enemy.GetComponent<PrTopDownCharInventory>().isDead != true && Enemy.GetComponent<PrTopDownCharInventory>().team != team)
                        {
                            if (EnemyDistance < BestDistance)
                            {
                                BestDistance = EnemyDistance;
                                meleeFinalTarget.Add(Enemy);// = Enemy;
                            }

                        }
                    }
                   
                }
            }
        }

        GameObject[] destroyables = GameObject.FindGameObjectsWithTag("Destroyable");

        if (destroyables != null)
        {
            float BestDistance = 100.0f;

            foreach (GameObject destroyable in destroyables)
            {
                Vector3 destroyablePos = destroyable.transform.position;
                Vector3 destrDirection = destroyablePos - Player.transform.position;
                float EnemyDistance = destrDirection.magnitude;

                if (Vector3.Angle(Player.transform.forward, destrDirection) <= 90 && EnemyDistance < MeleeRadius)
                {
                    if (EnemyDistance < BestDistance)
                    {
                        BestDistance = EnemyDistance;
                        meleeFinalTarget.Add(destroyable);// = Enemy;
                    }
                 }
            }
        }

        foreach (GameObject meleeTarget in meleeFinalTarget)
        {
            //Debug.Log("Hit Enemy Sucessfully");
            meleeTarget.SendMessage("PlayerTeam", team, SendMessageOptions.DontRequireReceiver);
            meleeTarget.SendMessage("BulletPos", ShootFXPos.position, SendMessageOptions.DontRequireReceiver);
            meleeTarget.SendMessage("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
        }
            
       
    }

    public void LoadAmmo(int LoadType)
    {
        ActualBullets = Bullets;
        ActualClips = Clips / LoadType;
        WeaponEndReload();
    }

    void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FinalTarget, 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ShootFXPos.position, 0.2f);*/

    }
}
