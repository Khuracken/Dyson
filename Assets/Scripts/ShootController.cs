using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PROJECTILE FIRE SCRIPT */

public class ShootController : MonoBehaviour
{

    public Rigidbody projectile;                                // Projectile object (rigidbody)
    public float projectileSpeed;                               // Projectile speed
    public float projectileFireRate;                            // Projectile rate of fire
    [SerializeField] public float projectileShoot;              // Projectile timer to shoot next shot
    public float projectileLife;                                // Projectile max time to exist on field

    public Rigidbody bomb;                                // Bomb object (rigidbody)
    public float bombSpeed;                               // Bomb speed
    public float bombFireRate;                            // Bomb rate of fire
    [SerializeField] public float bombShoot;              // Bomb timer to shoot next bomb
    public float bombLife;                                // Bomb max time to exist on field

    public int desiredBombs = 2;          // Desired number of bombs to exist on field
    public int currentBombs;          // Count of current existing bombs on field

    public GameObject player;                                   // Player object

    public void Start() {

        projectileSpeed = 30f;
        projectileFireRate = 0.2f;
        projectileShoot = 0f;
        projectileLife = 0.3f;

        bombSpeed = 0.5f;
        bombFireRate = 2f;
        bombShoot = 0f;
        bombLife = 5f;
    }

    private void Update() {

        // SHOOT PROJECTILE ACTION ( WHEN COOLDOWN IS REACHED )
        if (Input.GetKey(KeyCode.LeftShift) && Time.time > projectileShoot) {

            FireProjectile();
        }

        // SHOOT BOMB ACTION ( WHEN COOLDOWN IS REACHED & WHEN NO PICKUP ITEM IS IN POSSESSION )
        if (Input.GetKey(KeyCode.E) && Time.time > bombShoot) {

            ThrowBomb();
        }
    }

    public void FireProjectile() {

        // CALCULATE WHEN TO SHOOT
        projectileShoot = Time.time + projectileFireRate;

        // CREATE A NEW PROJECTILE ( AS RIGIDBODY )
        Rigidbody Projectile = Instantiate( projectile,
                                            transform.position,
                                            transform.rotation) as Rigidbody;

        // CALCULATE THE VELOCITY OF THE PROJECTILE
        Projectile.velocity = transform.TransformDirection(new Vector3(0, 0, projectileSpeed));

        // DESTROY THE OBJECT AFTER A CERTAIN TIME
        Destroy(Projectile.gameObject, projectileLife);
    }

    public void ThrowBomb() {

        // CALCULATE WHEN TO SHOOT
        bombShoot = Time.time + bombFireRate;

        // COUNT THE NO OF ENEMIES ON FIELD
        currentBombs = GameObject.FindGameObjectsWithTag("Bomb").Length;

        // IF THERE IS LESS NO OF DESIRED ENEMIES ON FIELD ... SPAWN MISSING DESIRED ENEMIES
        if (currentBombs < desiredBombs)
        {
            // DEPLOY A SEARCH AREA
            Rigidbody Bomb = Instantiate(bomb,
                                    transform.position,
                                    transform.rotation) as Rigidbody;

            // CALCULATE THE VELOCITY OF THE PROJECTILE
            Bomb.velocity = transform.TransformDirection(new Vector3(0, 0, bombSpeed));

            Destroy(Bomb.gameObject, bombLife);
        }
    }

    IEnumerator ExplodeBomb(float time)
    {
        
        yield return new WaitForSeconds(time);  // Code execution delay

    }
}

