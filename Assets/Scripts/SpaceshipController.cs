using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SpaceShip CONTROLLER SCRIPT */
/* Description : SpaceShip object acting as a moving platform between waypoints */

public class SpaceshipController : MonoBehaviour
{
    [HideInInspector] public GameObject player;     // Player object

    public GameObject[] ssWaypoints;                // List of SpaceShip (ss) waypoints              
    int ssWaypointIndex;                            // Waypoint index
    public Vector3 nextWaypointPosition;            // Next available index

    public float xAngle, yAngle, zAngle;            // SpaceShip (angle) rotation
    public float ssSpeed;                           // SpaceShip speed
    public Vector3 ssPosition;                     // SpaceShip position


    public void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");    // Setting the player object

        ssWaypointIndex = 0;                                   // Instantiating the waypoint index
        yAngle = 180;                                           // Setting the y angle rotation
        ssSpeed = 1.5f;                                        // Setting the cat speed
    }

    public void Update()
    {

        // GET THE POSITION OF THE CAT OBJECT
        ssPosition = transform.position;

        // SET THE NEXT WAYPOINT POSITION TO GO TO
        nextWaypointPosition = ssWaypoints[ssWaypointIndex].transform.position;

        // CHECK IF OBJECT HAS REACHED THE WAYPOINT
        if (ssPosition == nextWaypointPosition)
        {

            // INCREMENT THE WAYPOINT INDEX
            ssWaypointIndex++;

            // ROTATE THE OBJECT TO FACE NEW WAYPOINT
            transform.Rotate(xAngle, yAngle, zAngle);

            // CHECK IF OBJECT HAS REACHED LAST WAYPOINT
            if (ssWaypointIndex >= ssWaypoints.Length)
            {

                // RESET THE WAYPOINT INDEX
                ssWaypointIndex = 0;
            }
        }

        // SET THE OBJECT TO MOVE TOWARDS TO THE NEXT WAYPOINT
        transform.position = Vector3.MoveTowards(transform.position, nextWaypointPosition, Time.deltaTime * ssSpeed);
    }
}
