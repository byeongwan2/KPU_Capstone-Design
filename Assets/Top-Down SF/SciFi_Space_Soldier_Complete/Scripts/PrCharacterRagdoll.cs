using UnityEngine;
using System.Collections;

public class PrCharacterRagdoll : MonoBehaviour {

    public GameObject[] ragdollBones;
    public bool DeactivateAtStart = true;
    private Transform weaponObject;

    private ParticleSystem[] VFXParticles;
    private bool active = false;

    private float ragdollTimer = 5.0f;
    private float activeRagdollTimer = 0.0f;
    // Use this for initialization
    void Start () {
        InitializeRagdoll();
        if (GetComponentInChildren<ParticleSystem>())
            VFXParticles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
	void Update () {
	    if (active)
        {
            if (activeRagdollTimer < ragdollTimer)
            {
                activeRagdollTimer += Time.deltaTime;
            }
            else
            {
                DeactivateRagdoll();
            }
            
        }
	}

    public void InitializeRagdoll()
    {
        if (transform.GetComponentInChildren<PrWeapon>())
        {
            weaponObject = transform.GetComponentInChildren<PrWeapon>().transform;
            weaponObject.gameObject.layer = LayerMask.NameToLayer("PlayerCharacter");
        }
       
        Rigidbody[] temp = transform.GetComponentsInChildren<Rigidbody>();
        ragdollBones = new GameObject[temp.Length];
        int t = 0;
        foreach (Rigidbody r in temp)
        {
            if (r.gameObject.name != gameObject.name)
            {
                r.gameObject.layer = LayerMask.NameToLayer("DeadCharacter");
                ragdollBones.SetValue(r.gameObject, t);
                t += 1;
            }
        }

        if (DeactivateAtStart)
        {
            foreach (GameObject GO in ragdollBones)
            {
                if (GO != null)
                {
                    GO.GetComponent<Collider>().enabled = false;
                    GO.GetComponent<Rigidbody>().isKinematic = true;
                    if (GO.GetComponent<CharacterJoint>())
                    {
                        GO.GetComponent<CharacterJoint>().enableProjection = true;

                        //GO.AddComponent<PrRagdollStretchFix>();
                    }
                    
                }
            }

            if (weaponObject)
            {
                weaponObject.gameObject.GetComponent<Collider>().enabled = false;
                //weaponObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                weaponObject.gameObject.layer = LayerMask.NameToLayer("Weapon");
            }
        }
    }

    public void DeactivateRagdoll()
    {
        foreach (GameObject GO in ragdollBones)
        {
            if (GO != null)
            {
                GO.GetComponent<Collider>().enabled = false;
                GO.GetComponent<Rigidbody>().isKinematic = true;

            }
        }
        active = false;
        activeRagdollTimer = 0.0f;
    }

    public void ActivateRagdoll()
    {
        active = true;
        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = false;
        foreach (GameObject GO in ragdollBones)
        {
            if (GO != null)
            {
                GO.GetComponent<Collider>().enabled = true;
                GO.GetComponent<Rigidbody>().isKinematic = false;
                
            }
        }
        //if (transform && weaponObject)
        //    weaponObject.SetParent(transform);
        if (weaponObject)
        {
            weaponObject.SetParent(null);
            weaponObject.gameObject.AddComponent<Rigidbody>();
            weaponObject.gameObject.GetComponent<Collider>().enabled = true;
        }
          
    }
    public void SetForceToRagdoll(Vector3 position, Vector3 force, Transform target)
    {
        if (!active)
            ActivateRagdoll();

        GameObject targetBone = ragdollBones[0];
        if (target != null)
        {
            targetBone = target.gameObject;
        }
        else
        {
             //getClosestBone
            float dist = 200f;
            foreach (GameObject go in ragdollBones)
            {
                if (go != null)
                {
                    float tempDist = Vector3.Distance(go.transform.position, position);
                    if (dist > tempDist)
                    {
                        dist = tempDist;
                        targetBone = go;
                    }
                }
            }
        }
       
        //clamp force
        //Debug.Log("Force Applied =" + force);

        targetBone.GetComponent<Rigidbody>().AddForceAtPosition(force, position,ForceMode.Impulse);

        if (weaponObject)
        {    //weaponObject.GetComponent<Rigidbody>().AddExplosionForce(1.0f, position, 1.0f);
            weaponObject.GetComponent<Rigidbody>().AddForceAtPosition(force * 0.1f, position, ForceMode.Impulse);
            weaponObject.gameObject.AddComponent<PrDestroyTimer>();
        }
    }

    public void SetExplosiveForce(Vector3 position)
    {
        //Debug.Log("Explosion Acitvated");
        InitializeRagdoll();
        //ActivateRagdoll();

        foreach (GameObject go in ragdollBones)
        {
            if (go != null)
            {
                //go.GetComponent<Rigidbody>().AddForceAtPosition(position * 25f, position, ForceMode.Impulse);
                go.GetComponent<Rigidbody>().AddExplosionForce(20.0f, position, 2.0f, 0.25f, ForceMode.Impulse);
                //Debug.Log(go.name);
            }
        }

        if (VFXParticles != null && VFXParticles.Length > 0)
        {
            foreach (ParticleSystem p in VFXParticles)
            {
                p.Play();
            }
        }
    }
}
