using UnityEngine;
using System.Collections;

public class PrWaypointsRoute : MonoBehaviour {

    [Header("Waypoints Settings")]

    public Transform[] waypoints;
    public float timeToWait = 3.0f;
    public Mesh waypointMesh;
    public bool oneWayPath = false;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        int wayp = 0; 
        if (waypoints.Length > 0)
        {
            foreach (Transform waypoint in waypoints)
            { 
                if (waypoint != null )
                {
                    Gizmos.DrawSphere(waypoint.position, 0.4f);

                    if (wayp < waypoints.Length - 1)
                    {
                        Gizmos.color = Color.green * 0.75f;
                        if (waypoints[wayp + 1] != null)
                        {
                            Gizmos.DrawLine(waypoint.position, waypoints[wayp + 1].position);
                            Gizmos.color = Color.green * 2;
                            Gizmos.DrawMesh(waypointMesh, waypoint.position, Quaternion.LookRotation(waypoints[wayp + 1].position - waypoint.position, Vector3.up), Vector3.one);
                        }
                           
                    }
                    else if (!oneWayPath)
                    {
                        Gizmos.color = Color.green * 0.75f;
                        Gizmos.DrawLine(waypoint.position, waypoints[0].position);
                        Gizmos.color = Color.green * 2;
                        Gizmos.DrawMesh(waypointMesh, waypoint.position, Quaternion.LookRotation(waypoints[0].position - waypoint.position, Vector3.up), Vector3.one);
                    }
                }
               
                    
                wayp = wayp + 1;
            }
        }
    }
}
