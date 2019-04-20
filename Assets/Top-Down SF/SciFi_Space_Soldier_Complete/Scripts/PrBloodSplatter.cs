using UnityEngine;
using System.Collections;

public class PrBloodSplatter : MonoBehaviour {

    public Transform rayDir;
    public Transform rayTarget;
    public float directionRandom = 5.0f;
    private Vector3 finalOrientation;

    public int splatNumber = 5;
    private ParticleSystem particles;
	// Use this for initialization
	void Start () {
        particles = GetComponentInChildren<ParticleSystem>();
        
        //////Fix for 2017.2 BUG //////
        particles.Emit(1);
        particles.Clear();
        ////// End of Fix //////

        rayDir = new GameObject("rayDir").transform;
        rayDir.position = transform.position + (transform.up * 1.5f);
        rayDir.position -= transform.forward * 0.5f;
        rayDir.rotation = transform.rotation;
        rayDir.parent = transform;

        rayTarget = new GameObject("rayTarget").transform;
        rayTarget.position = rayDir.position + rayDir.forward * -2;
        rayTarget.position -= transform.up * 0.5f;
        rayTarget.rotation = transform.rotation;
        rayTarget.parent = rayDir;

    }
	
	// Update is called once per frame
	void Update () {
       
    }

    void RandomRotation()
    {

        float randomRot = Random.Range(directionRandom, -directionRandom);
        float randomRot2 = Random.Range(0, -directionRandom * 4);
        float randomRot3 = Random.Range(directionRandom, -directionRandom);

        finalOrientation = rayTarget.position + new Vector3(randomRot, randomRot2, randomRot3);
        finalOrientation = finalOrientation - rayDir.position;
        finalOrientation = finalOrientation.normalized * 10;
    }

    public void Splat()
    {
        for (int i = 0; i < splatNumber; i++)
        {
            RandomRotation();

            RaycastHit hit;
           
            int layerMask = 1 << 0;
            int LayerMask2 = 1 << 8;
            layerMask = layerMask | LayerMask2;

            if (Physics.Raycast(rayDir.position, finalOrientation, out hit, 10.0f , layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag != "Player" && hit.collider.tag != "Enemy")
                {
                    //Debug.Log("Collider =" + hit.collider + " " + hit.normal);
                    particles.transform.rotation = Quaternion.LookRotation(hit.normal);
                    Vector3 rot = particles.transform.rotation.eulerAngles;

                    particles.transform.position = hit.point + (particles.transform.forward * 0.025f);

                    var emitParams = new ParticleSystem.EmitParams();
                    emitParams.rotation3D = rot;
                    emitParams.position = hit.point + (particles.transform.forward * 0.025f) + (Vector3.up * Random.Range(0.03f, 0.01f));
                   
                    particles.Emit(emitParams, 1);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (rayDir && rayTarget)
        {
            Gizmos.DrawLine(rayDir.position, finalOrientation);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(rayDir.position, finalOrientation);
            Gizmos.DrawSphere(rayTarget.position, 0.25f);
        }
        
    }
}
