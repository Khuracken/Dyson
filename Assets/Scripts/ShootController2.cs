using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PROJECTILE FIRE SCRIPT */

public class ShootController2 : MonoBehaviour
{

    public Rigidbody projectile;                                // Projectile object (rigidbody)
    public float projectileSpeed;                               // Projectile speed
    public float projectileFireRate;                            // Projectile rate of fire
    [SerializeField] public float projectileShoot;              // Projectile timer to shoot next shot
    public float projectileLife;                                // Projectile max time to exist on field

    public GameObject shield;                          // Shield object
    public GameObject shieldClone;                              // Search area (clone) object
    public int desiredShield = 1;          // Desired number of shield to exist on field
    public int currentShield;          // Count of current existing shields on field
    public float shieldLife;                                // Shield max time to exist on field
    [SerializeField] public float shieldShoot;              // Shield timer to shoot next shot

    public GameObject player;                                   // Player object
    public bool isShieldDeployed = false;

    public void Start() {

        projectileSpeed = 30f;
        projectileFireRate = 0.2f;
        projectileShoot = 0f;
        projectileLife = 0.3f;

        shieldLife = 5f;
        shieldShoot = 10f;
    }

    private void Update()
    {
        // SHOOT PROJECTILE ACTION ( WHEN COOLDOWN IS REACHED )
        if (Input.GetKey(KeyCode.Return) && Time.time > projectileShoot) {

            FireProjectile();
        }

        // GETTING THE CURRENT SHIELD DEPLOYED
        currentShield = GameObject.FindGameObjectsWithTag("Shield").Length;
        Debug.Log("Shield deployed: " + currentShield);

        // DEPLOY A SHIELD
        if (Input.GetKey(KeyCode.O) && isShieldDeployed == false)
        {

            ShieldDeploy();
        }

        // CHECK IF SHIELD IS DEPLOYED
        if (currentShield == desiredShield)
        {
            // MOVE SHIELD POSITION
            shieldClone.transform.position = player.transform.position;
        }
    }

    public void FireProjectile()
    {

        // LINK W/ PICKUPITEM.CS
        //PickupItem _pickupItem = FindObjectOfType<PickupItem>();

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

    public void ShieldDeploy()
    {

        // CHECK IF A SEARCH AREA IS NOT DEPLOYED
        if (currentShield < desiredShield)
        {
            // DEPLOY A SEARCH AREA
            shieldClone = Instantiate(shield, player.transform.position, Quaternion.identity);
            isShieldDeployed = true;

            // DESTROY THE OBJECT AFTER A CERTAIN TIME
            Destroy(shieldClone.gameObject, shieldLife);

            StartCoroutine(ExecuteAfterTime(shieldShoot));
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);  // Code execution delay
            isShieldDeployed = false;
        }
    }
}

