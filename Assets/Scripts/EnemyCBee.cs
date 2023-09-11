using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* ENEMY CONTROLLER SCRIPT */
/* Description : Enemy of type GUARD that 1) Patrols area in accordance with waypoints set,
                                          2) Alerts enemies in proximity of type NORMAL when player is in sight (LOS)
                                          3) Chases player upon sight (KILL)
*/

public class EnemyCBee : MonoBehaviour
{
    // ENEMY AI STATES
    private enum EnemyAIstate
    {
        PATROLLING,
        CHASING,
        FLOCKING,
        DEAD
    }

    [HideInInspector] public GameObject player;                 // Player object
    [HideInInspector] public NavMeshAgent enemyAI;              // Enemy object
    public GameObject alarmSearchArea;                          // Search area object
    private EnemyAIstate enemyAIstate;                          // Enemy state object
    public GameObject prefabClone;                              // Search area (clone) object

    public float enemySpeed;                // EnemyAI Speed

    public GameObject[] patrolWaypoints;
    //public Transform[] patrolWaypoints;     // List of patrol waypoints
    public int patrolWaypointIndex;         // Current patrol waypoint index

    private float distanceToPlayer;         // Distance between Player & EnemyAI
    public float distanceToWaypoint;        // Distance between Player & Next waypoint
    public float distanceFinishPatrol;      // Distance between Next Waypoint & range to reach waypoint

    public bool isWaypointReached;          // Waypoint status (Reached/Not Reached)
    public bool isPlayerVisible;            // Player visibility status (Visible/Not visible)  
    public bool isPatrolling;               // Enemy patrolling status (Patrolling/Not patrolling)  

    public float playerChaseRange;          // Distance to chase player
    public RaycastHit hit;                  // RaycastHit
    public Ray ray;                         // Ray
    public float rayRadius;                 // LOS radius of raycast
    public Vector3 raycastDirection;        // Direction of raycast

    public int currentPrefabs;              // Current no of search areas (clones)
    public int desiredPrefabs;              // Desired no of search areas (clones)
    public float prefabLifeTime;            // Lifetime of search area (clone) when enemy loses sight of player

    public float enemyDMGTaken;                            // Enemy DMG taken
    [SerializeField] public float enemyHP;                 // Enemy Health (HitPoints)
    [SerializeField] public float enemyMaxHP;              // Enemy MAX Health (HitPoints)

    private void Start()
    {

        // REFERENCES
        enemyAI = GetComponent<NavMeshAgent>();
        //player = GameObject.FindGameObjectWithTag("Players");
        patrolWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        isPlayerVisible = false;            // Instantiating the visibility state
        isPatrolling = true;                // Instantiating the patrolling state

        playerChaseRange = 8;               // Setting the distance to chase player
        distanceFinishPatrol = 1;           // Setting the distance to reach waypoint
        rayRadius = 1;                      // Setting the LOS radius of enemy

        desiredPrefabs = 1;                 // Setting the desired no of search areas (clones)
        prefabLifeTime = 1;                 // Setting the search area (clone) lifetime

        enemySpeed = 2f;                    // Setting the enemy speed
        enemyAI.speed = enemySpeed;         // Setting the enemy speed

        enemyMaxHP = 2;                     // Setting the enemy max HP
        enemyHP = enemyMaxHP;               // Setting the enemy HP

        // SET INITIAL STATE TO PATROLLING STATE
        enemyAIstate = EnemyAIstate.PATROLLING;
    }

    private void Update()
    {
        FindClosestPlayer();

        switch (enemyAIstate)
        {
            default:

            case EnemyAIstate.PATROLLING:

                EnemyPatrolling();  // PATROLS AREA
                EnemySearching();   // SEARCHES FOR PLAYER
                break;

            case EnemyAIstate.CHASING:

                EnemyAlerting();    // ALERTS NORMAL ENEMIES
                EnemyChasing();     // CHASES PLAYER
                break;

            /* This case was MAINLY implemented in case a 2nd enemy of type GUARD was to exist */
            /* This was an unfinished work-in progress as the GUARDS would clash with each */
            /* other when setting up a search area */

            case EnemyAIstate.FLOCKING:

                EnemyMovementFlocking();    // GOES TO A SEARCH AREA
                EnemySearching();           // SEARCHES FOR PLAYER
                break;

            case EnemyAIstate.DEAD:

                EnemyDead();    // ENEMY IS DEAD
                break;
        }
    }

    private void FindClosestPlayer()
    {

        float distanceToClosestPlayer = Mathf.Infinity;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Players");

        foreach (GameObject p in allPlayers)
        {
            float distanceToPlayer = (p.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToPlayer < distanceToClosestPlayer)
            {
                distanceToClosestPlayer = distanceToPlayer;
                player = p;
            }
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO PATROL THE AREA
    private void EnemyPatrolling()
    {

        distanceToWaypoint = enemyAI.remainingDistance;                             // Setting the distance to waypoint

        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;    // Getting the current search areas deployed

        // CHECK IF A SEARCH AREA IS DEPLOYED TO GO TO
        if (currentPrefabs == desiredPrefabs)
        {

            // CHANGE STATE TO FLOCKING
            enemyAIstate = EnemyAIstate.FLOCKING;
        }

        // CHECK IF PATROL WAYPOINT HAS BEEN REACHED
        if (isPatrolling && distanceToWaypoint < distanceFinishPatrol && !isWaypointReached)
        {

            isWaypointReached = true;   // Setting the waypoint reached status to reached
            StartCoroutine(Patrol());   // CONTINUES PATROLLING TO NEXT WAYPOINT
        }
        else
        {

            isWaypointReached = false;  // Waypoint reached status remains not reached
        }

        // ENUMERATOR FOR PATROLLING
        IEnumerator Patrol()
        {

            // PATROLLING BREAKS IF THERE ARE NO WAYPOINTS
            if (patrolWaypoints.Length == 0)
            {

                yield break;
            }

            else
            {

                isWaypointReached = true;   // Setting the waypoint reached status to reached

                // SETTING THE ENEMY DESTINATION TO NEXT WAYPOINT
                //enemyAI.destination = patrolWaypoints[patrolWaypointIndex].position;
                enemyAI.destination = patrolWaypoints[patrolWaypointIndex].transform.position;

                // RANDOMIZING THE WAYPOINT INDEX FOR THE NEXT WAYPOINT IN SEQUENCE
                patrolWaypointIndex = Random.Range(0, patrolWaypoints.Length);
                Debug.Log("Patrol index: " + patrolWaypointIndex);
            }
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO CHASE THE PLAYER
    private void EnemyChasing()
    {

        // SETTING THE ENEMY TO CHASE THE PLAYER
        enemyAI.SetDestination(player.transform.position);

        // GETTING THE DISTANCE TO PLAYER
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // CHECK IF PLAYER IS WITHIN RANGE TO CHASE
        if (distanceToPlayer > playerChaseRange)
        {

            // SEARCH AREA IS DESTROYED
            Destroy(prefabClone);

            // CHANGE STATUS TO PATROLLING
            enemyAIstate = EnemyAIstate.PATROLLING;
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO ALERT OTHER ENEMIES OF TYPE NORMAL
    public void EnemyAlerting()
    {

        // GETTING THE CURRENT SEARCH AREAS DEPLOYED
        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;
        Debug.Log("Search areas deployed: " + currentPrefabs);

        // GETTING THE DISTANCE TO PLAYER
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // CHECK IF A SEARCH AREA IS NOT DEPLOYED
        if (currentPrefabs < desiredPrefabs)
        {

            // DEPLOY A SEARCH AREA
            prefabClone = Instantiate(alarmSearchArea, player.transform.position, Quaternion.identity);
        }

        // CHECK IF A SEARCH AREA IS DEPLOYED
        if (currentPrefabs == desiredPrefabs)
        {

            // MOVE SEARCH AREA TO LAST POSITION SEEN BY ENEMY
            prefabClone.transform.position = player.transform.position;
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO SEARCH FOR THE PLAYER
    private void EnemySearching()
    {

        // SETTING THE RAYCAST DIRECTION
        raycastDirection = Vector3.forward;

        // SETTING THE RAY
        ray = new Ray(transform.position, transform.TransformDirection(raycastDirection * playerChaseRange));

        // DRAWING THE RAY (DEBUGGIN)
        Debug.DrawRay(transform.position, transform.TransformDirection(raycastDirection * playerChaseRange));

        // CHECK IF PLAYER IS HIT BY RAY
        if (Physics.SphereCast(ray, rayRadius, out hit, playerChaseRange))
        {

            if (hit.collider.tag == "Players")
            {

                isPlayerVisible = true;     // Setting the player visibility status to visible

                // CHANGE STATUS TO CHASING
                enemyAIstate = EnemyAIstate.CHASING;
            }
            else
            {

                isPlayerVisible = false;    // Setting the player visibility status to not visible
            }
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO GO TO A SEARCH AREA CREATED BY ANOTHER ENEMY
    private void EnemyMovementFlocking()
    {

        // GETTING THE SEARCH AREA OBJECT
        alarmSearchArea = GameObject.FindGameObjectWithTag("SearchArea");

        // GETTING THE CURRENT NO OF SEARCH AREAS
        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;

        // CALCULATE DISTANCE OF PLAYER FROM ENEMY
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // CHECK IF A SEARCH AREA IS DEPLOYED
        if (currentPrefabs == desiredPrefabs)
        {

            // GOTO SEARCH AREA
            enemyAI.SetDestination(alarmSearchArea.transform.position);
        }

        // CHECK IF PLAYER IS VISIBLE
        if (isPlayerVisible)
        {

            // CHANGE STATUS TO CHASING
            enemyAIstate = EnemyAIstate.CHASING;
        }

        // CHECK IF A SEARCH AREA IS NOT DEPLOYED
        else if (currentPrefabs == 0)
        {

            // CHANGE STATUS TO PATROLLING
            enemyAIstate = EnemyAIstate.PATROLLING;
        }
    }

    // FUNCTION THAT KILLS THE ENEMY
    private void EnemyDead()
    {

        Destroy(prefabClone);   // Search area (clone) object is destroyed
        Destroy(gameObject);    // Enemy object is destroyed
    }
}





