using UnityEngine;
using System.Collections;

public class PrDestroyableActor : MonoBehaviour {

    [Header("Stats")]
    public int Health = 100;
    public bool Destroyable = true;
    private int ActualHealth = 0;
    private bool Destroyed = false;

    [Header("Temperature Settings")]
    public bool useTemperature = true;
    [HideInInspector]
    public float temperature = 1.0f;
    public float temperatureThreshold = 2.0f;
    public int temperatureDamage = 5;
    public float onFireSpeedFactor = 0.5f;
    private float tempTimer = 0;

    [Header("VFX")]
    public float shakeFactor = 3.0f;
    public float shakeDuration = 1.0f;
    public GameObject DestroyFX;
	public Vector3 DestroyFXOffset = new Vector3(0,0,0);
    private PrTopDownCamera playerCamera;

    [Space]
    public GameObject frozenVFX;
    private GameObject actualFrozenVFX;
    public GameObject burningVFX;
    private GameObject actualBurningVFX;

    [Header("Damage")]
    public bool RadialDamage = true;
    public float DamageRadius = 2;
    public int Damage = 50;
    
    private bool Damaged = false;
    private float DamagedTimer = 0.0f;
    public int team = 3;

    [Header("HUD")]
    public bool ShowHealthBar = false;
    public GameObject HealthBar;
    public Color HealthBarColor = Color.white;
    private GameObject HealthBarParent;

    [Header("Debug")]
    public Mesh AreaRing;

    // Use this for initialization
    void Start () {
		ActualHealth = Health;
        if (GameObject.Find("PlayerCamera"))
            playerCamera = GameObject.Find("PlayerCamera").GetComponent<PrTopDownCamera>();
        if (HealthBar)
        {
            HealthBar.SetActive(ShowHealthBar);
        }

        if (useTemperature)
        {
            if (frozenVFX)
            {
                actualFrozenVFX = Instantiate(frozenVFX, transform.position, transform.rotation) as GameObject;
                actualFrozenVFX.transform.position = transform.position;
                actualFrozenVFX.transform.parent = transform;
            }
            if (burningVFX)
            {
                actualBurningVFX = Instantiate(burningVFX, transform.position, transform.rotation) as GameObject;
                actualBurningVFX.transform.position = transform.position;
                actualBurningVFX.transform.parent = transform;
            }
        }
        
    }

    void ActivateHealthBar(bool active)
    {
        HealthBar.GetComponent<UnityEngine.UI.Image>().color = HealthBarColor;
        HealthBarParent = HealthBar.transform.parent.gameObject;
        HealthBarParent.SetActive(active);
    }
	
	// Update is called once per frame
	void Update () {
        if ( Damaged )
        {
            DamagedTimer = Mathf.Lerp(DamagedTimer, 0.0f, Time.deltaTime * 10);
           
            if (Mathf.Approximately(DamagedTimer, 0.0f))
            {
                DamagedTimer = 0.0f;
                Damaged = false;

            }

            GetComponent<MeshRenderer>().material.SetFloat("_DamageFX", DamagedTimer);
        }
        if (useTemperature)
            ApplyTemperature();

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

        if (GetComponent<MeshRenderer>().material.HasProperty("_FrozenMix"))
            GetComponent<MeshRenderer>().material.SetFloat("_FrozenMix", Mathf.Clamp(1.0f - temperature, 0.0f, 1.0f));
        if (GetComponent<MeshRenderer>().material.HasProperty("_BurningMix"))
            GetComponent<MeshRenderer>().material.SetFloat("_BurningMix", Mathf.Clamp(temperature - 1.0f, 0.0f, 1.0f));

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

    void ApplyDamagePassive(int damage)
    {
        if (!Destroyed)
        {
            Health -= damage;

            if (Health <= 0)
            {
                DestroyActor();
            }
        }
    }

    void ApplyTempMod(float temperatureMod)
    {
        temperature += temperatureMod;
        temperature = Mathf.Clamp(temperature, 0.0f, temperatureThreshold);
    }

    void ApplyDamage(int Damage)
	{
        if (Destroyable)
        {
            ActualHealth -= Damage;

            Damaged = true;
            DamagedTimer = 1.0f;

            if (HealthBar)
                HealthBar.GetComponent<UnityEngine.UI.Image>().transform.localScale = new Vector3((1.0f / Health) * ActualHealth, 0.6f, 1.0f);

            if (ActualHealth <= 0 && !Destroyed)
            {
                DestroyActor();
            }
        }
		
	}

	void DestroyActor()
	{
        Destroyed = true;

        if (DestroyFX)
			Instantiate (DestroyFX, transform.position + DestroyFXOffset, Quaternion.identity);

        if (playerCamera)
        {
            playerCamera.ExplosionShake(shakeFactor, shakeDuration);
        }
	        			
		Destroy (this.gameObject);

        if (RadialDamage)
        {
            DestroyImmediate(GetComponent<Rigidbody>());
            DestroyImmediate(GetComponent<Collider>());
            Vector3 explosivePos = transform.position + DestroyFXOffset;
            Collider[] colls = Physics.OverlapSphere(explosivePos, DamageRadius);
            foreach (Collider col in colls)
            {

                if (col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Destroyable"))
                {
                    col.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                }
            }

        }


    }

    void OnDrawGizmos()
    {
        if (RadialDamage)
          Gizmos.DrawMesh(AreaRing, transform.position + DestroyFXOffset, Quaternion.identity,Vector3.one * DamageRadius);

    }
}
