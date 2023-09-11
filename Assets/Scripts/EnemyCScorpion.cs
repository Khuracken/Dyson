using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* ENEMY CONTROLLER SCRIPT */
/* Description : Enemy of type NORMAL that 1) Wanders around the world aimlessly,
                                           2) Chases player upon proximity (KILL)
*/

public class EnemyCScorpion : MonoBehaviour
{
    // ENEMY AI STATES
    private enum EnemyAIstate
    {
        WANDERING,
        CHASING,
        FLOCKING,
        DEAD
    }

    public GameObject player;         // Player object
    public NavMeshAgent enemyAI;      // Enemy object
    public GameObject alarmSearchArea;                  // Search area (clone) object
    private EnemyAIstate enemyAIstate;                  // Enemy state object

    public float enemyHP;                   // EnemyAI Health (HitPoints)
    public float enemyMaxHP;                // EnemyAI MAX Health (HitPoints)
    public float enemySpeed;                // EnemyAI Speed
    public bool isEnemyDead;

    public float enemyFollowRange;          // Range trigger to follow Player
    public float enemyFlockingRange;        // Range trigger to start flocking (goto search area)
    private float distanceToPlayer;         // Distance between Player & EnemyAI

    private float generalTimer;             // General timer
    public float enemyWanderTimer;          // Wandering timer by EnemyAI
    public float enemyWanderRange;          // Wandering radius by EnemyAI

    public int currentPrefabs;              // Current no of search areas (clones)

    public void Start()
    {

        // REFERENCES
        enemyAI = GetComponent<NavMeshAgent>();

        enemyMaxHP = 2;                     // Setting the enemy max HP
        enemyHP = enemyMaxHP;               // Setting the enemy HP

        enemySpeed = 2f;                 // Setting the enemy speed
        enemyAI.speed = enemySpeed;         // Setting the enemy speed

        enemyFollowRange = 8f;              // Setting the enemy follow range
        enemyFlockingRange = 9f;            // Setting the enemy flocking range
        enemyWanderRange = 10f;             // Setting the enemy wandering range

        enemyWanderTimer = 10f;             // Setting the enemy wandering timer
        generalTimer = enemyWanderTimer;    // Sync general timer w/ EnemyAI wander timer

        // CHANGE STATUS TO WANDERING
        enemyAIstate = EnemyAIstate.WANDERING;
    }

    private void Update()
    {
        FindClosestPlayer();

        // ENEMY STATES
        switch (enemyAIstate)
        {
            default:

            case EnemyAIstate.WANDERING:

                EnemyMovementWander();
                EnemySearching();
                break;

            case EnemyAIstate.CHASING:

                EnemyMovementChasing();
                break;

            case EnemyAIstate.FLOCKING:

                EnemyMovementFlocking();
                break;

            case EnemyAIstate.DEAD:

                EnemyDead();
                break;
        }
    }

    private void FindClosestPlayer() {

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

    // FUNCTION THAT INSTRUCTS THE ENEMY TO WANDER AROUND THE MAP
    private void EnemyMovementWander()
    {

        // ADDING EACH FRAME SECOND(S)
        generalTimer += Time.deltaTime;

        if (generalTimer >= enemyWanderTimer)
        {

            // SET NEW RANDOM DESTINATION FOR ENEMY AI TO GO TO
            Vector3 destination = RandomNavSphere(transform.position, enemyWanderRange, -1);
            enemyAI.SetDestination(destination);

            // RESET THE GENERAL TIMER
            generalTimer = 0;
        }
    }

    // CREATES A RANDOM NAVIGATION SPHERE
    public static Vector3 RandomNavSphere(Vector3 originPosition, float radius, int layermask)
    {

        Vector3 randDirection = Random.insideUnitSphere * radius;
        randDirection += originPosition;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, radius, layermask);

        return navHit.position;
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO SEARCH FOR THE PLAYER
    private void EnemySearching()
    {

        // GETTING THE SEARCH AREA OBJECT
        alarmSearchArea = GameObject.FindGameObjectWithTag("SearchArea");

        // GETTING THE CURRENT NO OF SEARCH AREAS
        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;

        // CALCULATE DISTANCE OF PLAYER FROM ENEMY
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // CHECK IF PLAYER IS WITHIN RANGE
        if (distanceToPlayer <= enemyFollowRange)
        {

            // CHANGE STATUS TO CHASING
            enemyAIstate = EnemyAIstate.CHASING;
        }

        // CHECK IF SEARCH AREA IS DEPLOYED
        if (currentPrefabs > 0 && distanceToPlayer <= enemyFlockingRange)
        {

            // CHANGE STATUS TO FLOCKING
            enemyAIstate = EnemyAIstate.FLOCKING;
        }
    }

    // FUNCTION THAT INSTRUCTS THE ENEMY TO CHASE THE PLAYER
    private void EnemyMovementChasing()
    {

        // CALCULATE DISTANCE OF PLAYER FROM ENEMY
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // GETTING THE CURRENT NO OF SEARCH AREAS
        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;

        // SET ENEMY TO CHASE PLAYER
        enemyAI.SetDestination(player.transform.position);

        // CHECK IF PLAYER IS NOT WITHIN RANGE BUT IS WITHIN FLOCKING RANGE
        if (distanceToPlayer > enemyFollowRange && currentPrefabs > 0
            && distanceToPlayer <= enemyFlockingRange)
        {

            // CHANGE STATUS TO FLOCKING
            enemyAIstate = EnemyAIstate.FLOCKING;
        }

        // CHECK IF PLAYER IS NOT WITHIN RANGE AND THAT A SEARCH AREA IS NOT DEPLOYED
        if (distanceToPlayer > enemyFollowRange && currentPrefabs == 0)
        {

            // CHANGE STATUS TO WANDERING
            enemyAIstate = EnemyAIstate.WANDERING;
        }
    }

    private void EnemyMovementFlocking()
    {

        // GETTING THE SEARCH AREA OBJECT
        alarmSearchArea = GameObject.FindGameObjectWithTag("SearchArea");

        // GETTING THE CURRENT NO OF SEARCH AREAS
        currentPrefabs = GameObject.FindGameObjectsWithTag("SearchArea").Length;

        // CALCULATE DISTANCE OF PLAYER FROM ENEMY
        distanceToPlayer = Vector3.Distance(enemyAI.transform.position, player.transform.position);

        // CHECK IF A SEARCH AREA IS DEPLOYED
        if (currentPrefabs > 0)
        {

            // SET ENEMY TO GOTO SEARCH AREA
            enemyAI.SetDestination(alarmSearchArea.transform.position);
        }

        // CHECK IS PLAYER IS WITHIN RANGE
        if (distanceToPlayer <= enemyFollowRange)
        {

            // CHANGE STATUS TO CHASING
            enemyAIstate = EnemyAIstate.CHASING;
        }

        // ELSE CHECK IS A SEARCH AREA IS NOT DEPLOYED
        else if (currentPrefabs == 0)
        {

            // CHANGE STATUS TO WANDERING
            enemyAIstate = EnemyAIstate.WANDERING;
        }
    }

    // FUNCTION THAT KILLS THE ENEMY
    private void EnemyDead()
    {
        Destroy(gameObject);    // Enemy object is destroyed
    }

    void OnTriggerStay(Collider collider)
    {
        // HANDLE COLLISION WITH PROJECTILE & BOMB
        if ( collider.GetComponent<Collider>().tag == "Projectile" ||
            collider.GetComponent<Collider>().tag == "Bomb" )
        {

            // ENEMY AI RECEIVES DAMAGE
            enemyHP = enemyHP - 1;

            // ENEMY AI DIES IF NO HEALTH
            if (enemyHP <= 0 && gameObject != null)
            {
                // CHANGE STATUS TO DEAD

                enemyAIstate = EnemyAIstate.DEAD;
            }
        }
    }
}


