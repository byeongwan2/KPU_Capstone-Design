using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrTopDownMutiplayerCam : PrTopDownCamera {

    [Header("Multiplayer Settings")]
    Camera Mcamera; 
    public bool multiplayerCam = false;
    public GameObject multiplayerCamTarget;
    public GameObject[] playersGO;
    public Vector2 targetHeightVariation = new Vector2(10, 40);
    public float targetHeightDistanceFactor = 1.0f;
    public float targetHeightCorrection = -3.0f;
    public float targetHeightLimit = 5f;
    public Transform[] walls;
    public Vector3[] cameraBounds;
    public Vector4 camaraLimitBounds;
    private float activateTimer = 0.0f;
    private bool wallActivated = false;
    public bool useCameraColisions = false;
    private float lastHeight = 0.0f;

    public void MultiplayerCam(GameObject[] playerForCamera,int actualPlayerCount)
    {
        multiplayerCam = true;
        if (multiplayerCamTarget == null)
            multiplayerCamTarget = new GameObject("MultiPlayerCamTarget") as GameObject;
        TargetToFollow = multiplayerCamTarget.transform;

        cameraBounds = new Vector3[4];

        //playersGO = new GameObject[0];

        playersGO = new GameObject[playerForCamera.Length];
        for (int i = 0; i < playerForCamera.Length; i++)
        {
            playersGO[i] = playerForCamera[i];
        }

        SetMultiplayerCamHeight();

    }
    
    // Use this for initialization
    public virtual void Start () {

        Mcamera = GetComponentInChildren<Camera>();

        SetMultiplayerCamHeight();

        ActualHeight = TargetHeight;

        ActivateWalls(false);
    }

    public virtual void ActivateWalls(bool active)
    {
        
        foreach (Transform w in walls)
        {
            w.gameObject.SetActive(active);
        }

        wallActivated = active;
    }
    
    public void ResetWalls()
    {
        ActivateWalls(false);
        wallActivated = false;
        activateTimer = 0.0f;
    }
    public virtual void FixedUpdate()
    {
        if (multiplayerCam)
        {

            SetMultiplayerCamHeight();

            if (activateTimer <= 3.0f)
                activateTimer += Time.deltaTime;
            else if (activateTimer > 2.0f && useCameraColisions && !wallActivated)
            {
                ActivateWalls(true);
            }
            
            // playersGO = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject o in playersGO)
            {
               // Debug.Log("Player " + o.name);
            }

            cameraBounds[0] = Mcamera.ViewportToWorldPoint(new Vector3(1 * camaraLimitBounds[0], 0.5f, 10));
            cameraBounds[1] = Mcamera.ViewportToWorldPoint(new Vector3(1 + ( camaraLimitBounds[1] * -1), 0.5f, 10));
            cameraBounds[2] = Mcamera.ViewportToWorldPoint(new Vector3(0.5f, 1 * camaraLimitBounds[2], 10));
            cameraBounds[3] = Mcamera.ViewportToWorldPoint(new Vector3(0.5f, 1 + (camaraLimitBounds[3] * -1), 10));

            if (walls.Length > 0)
            {
                for (int i = 0; i < walls.Length; i++ )
                {
                    RaycastHit hit;
                    Vector3 dir = cameraBounds[i] - Mcamera.transform.position;
                    if (Physics.Raycast(Mcamera.transform.position, dir, out hit, 999f, LayerMask.NameToLayer("Floor")))
                    {
                        walls[i].position = new Vector3(hit.point.x, 0.0f, hit.point.z);
                                            
                    }
                   
                }
            }
        }

    }

    public void SetMultiplayerCamHeight()
    {
        if (playersGO.Length > 0)
        {
            if (playersGO.Length == 1)
            {
                multiplayerCamTarget.transform.position = playersGO[0].transform.position;
                targetHeightOffset = -2;
            }

            else if (playersGO.Length == 2)
            {
                multiplayerCamTarget.transform.position = (playersGO[0].transform.position + playersGO[1].transform.position) / 2;
                multiplayerCamTarget.transform.position -= Vector3.one * targetHeightCorrection;

                if (!useCameraColisions)
                {
                    if (MostDistantPlayer() > targetHeightVariation.x && MostDistantPlayer() < targetHeightVariation.y)
                    {
                        lastHeight = targetHeightOffset;
                        targetHeightOffset = MostDistantPlayer() * targetHeightDistanceFactor;
                    }
                    else
                    {
                        targetHeightOffset = lastHeight;
                    }
                }
                

            }

            else if (playersGO.Length == 3)
            {
                multiplayerCamTarget.transform.position = (playersGO[0].transform.position + playersGO[1].transform.position + playersGO[2].transform.position) / 3;
                multiplayerCamTarget.transform.position -= Vector3.one * targetHeightCorrection;

                if (MostDistantPlayer() > (targetHeightVariation.x * 2) && MostDistantPlayer() < targetHeightVariation.y)
                {
                    targetHeightOffset = MostDistantPlayer() * targetHeightDistanceFactor;
                }

            }

            else if (playersGO.Length == 4)
            {
                multiplayerCamTarget.transform.position = (playersGO[0].transform.position + playersGO[1].transform.position + playersGO[2].transform.position + playersGO[3].transform.position) / 4;
                multiplayerCamTarget.transform.position -= Vector3.one * targetHeightCorrection;

                if (MostDistantPlayer() > (targetHeightVariation.x * 2.5f) && MostDistantPlayer() < targetHeightVariation.y)
                {
                    targetHeightOffset = MostDistantPlayer() * targetHeightDistanceFactor;
                }
            }
        }
        
        
    }

    public float MostDistantPlayer()
    {
        float initialDistance = 0f;

        foreach (GameObject p in  playersGO)
        {
            for (int i= 0; i < playersGO.Length; i++)
            {
                if (Vector3.Distance(p.transform.position, playersGO[i].transform.position) > initialDistance)
                {
                    initialDistance = Vector3.Distance(p.transform.position, playersGO[i].transform.position);
                }
            }
        }
        return initialDistance;
    }
    

}
