using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/*
 * For detecting projectile collisions with the enemy shield. To be placed on the shield of the image target.
 * 
 * 
*/
public class EnemyShieldCollisionScript : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameManager gameManager;
    public GameObject rainBombExplosionEffect;

    // Testing for Rain Bomb effect.
    public GameObject rainPrefab;
    public AudioSource rainSound;
    // private float spawnOffsetZ = 3f;
    public RainBombManager rainBombManager;
    public RainAnchorController rainAnchorController;
    public EnemyUIScript enemyUIScript;
    // Testing

    public AudioSource basketballExplosionSound;
    public AudioSource soccerballExplosionSound;
    public AudioSource volleyballExplosionSound;
    public AudioSource bowlingballExplosionSound;

    // Start is called before the first frame update
    void Start()
    {
        rainBombManager = GameObject.FindWithTag("RainBombManager").GetComponent<RainBombManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Basketball")
        {
            Debug.Log("Basketball hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            basketballExplosionSound.Play();
            // gameManager.OnAIActionImpactWithShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "SoccerBall")
        {
            Debug.Log("Soccerball hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            soccerballExplosionSound.Play();
            // gameManager.OnAIActionImpactWithShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Volleyball")
        {
            Debug.Log("Volleyball hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            volleyballExplosionSound.Play();
            // gameManager.OnAIActionImpactWithShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BowlingBall")
        {
            Debug.Log("Bowling ball hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            bowlingballExplosionSound.Play();
            // gameManager.OnAIActionImpactWithShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Bullet hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            // Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // bowlingballExplosionSound.Play();
            // gameManager.OnBulletImpactWithShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        // Testing for Rain Bomb for Shield collisions.
        /*
        if (collision.gameObject.tag == "RainBomb")
        {
            Debug.Log("Rainbomb hit target");
            // Get the point of collision
            Vector3 collisionPoint = collision.contacts[0].point;

            GameObject rb = rainBombManager.DisplayRain(collisionPoint);

            // Set the rainbombs parent to be the enemy, and then set z upwards, and then reset the parent. 
            rb.transform.parent = (collision.gameObject.transform);

            Vector3 enemyPosition = collision.gameObject.transform.position;
            enemyPosition.z += spawnOffsetZ;
            rb.transform.position = enemyPosition;
            rb.transform.parent = null;

            if (rainSound != null)
            {
                rainSound.Play();
            }

            gameManager.OnBulletImpactWithShield();
            // Destroy the rain bomb after collision, not the shield
            Destroy(collision.gameObject);
            // call the function once rain bomb hits the target. deal damage.
            gameManager.OnRainBombImpact();
        }
        */

        // Testing for Rain Bomb
        /*
        if (collision.gameObject.tag == "RainBomb")
        {
            Debug.Log("Rain bomb hit target");
            // Get the point of collision 
            Vector3 collisionPoint = collision.contacts[0].point;

            GameObject rain = rainBombManager.DisplayRain(collisionPoint);

            // Set the rainbomb's parent to be the enemy
            rain.transform.parent = gameObject.transform;

            // Adjust the rain bomb's position with the Z offset
            Vector3 enemyPosition = gameObject.transform.position;
            enemyPosition.z += spawnOffsetZ;
            rain.transform.position = enemyPosition;

            // Store the world position before detaching from the parent
            Vector3 worldPosition = rain.transform.position;

            // Detach the rain from the enemy
            rain.transform.parent = null;

            // Set the world position after detaching to ensure it stays in place
            rain.transform.position = worldPosition;

            if (rainSound != null)
            {
                rainSound.Play();
            }


            // NEW CHANGES HERE. SET UP RAIN COLLIDER HERE.
            // Setup the rain collider as a trigger
            Collider rainCollider = rain.GetComponent<Collider>();
            if (rainCollider != null)
            {
                rainCollider.isTrigger = true;  // Ensure the collider is set as a trigger
                Debug.Log("Rain collider set as trigger.");
            }
            else
            {
                Debug.LogError("Rain effect does not have a collider attached.");
            }


            gameManager.OnBulletImpactWithShield();

            // Destroy the rain bomb after collision, instead of the enemy target.
            Destroy(collision.gameObject);

            // Call the function once rain bomb hits the target to deal damage.
            // gameManager.OnRainBombImpact();
        }
        */

        if (collision.gameObject.CompareTag("RainBomb"))
        {
            Debug.Log("Rain bomb hit target");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(rainBombExplosionEffect, collisionPoint, Quaternion.identity);
            Vector3 enemyPosition = enemyUIScript.GetEnemyPosition();
            // Define a height offset
            float heightOffset = 1.3f; // Adjust this value as needed

            // Create a new point that is higher in the Y-axis
            Vector3 spawnPoint = new Vector3(enemyPosition.x, enemyPosition.y + heightOffset, enemyPosition.z);

            // Create unique name
            string anchorName = "RainAnchor_" + Time.time.ToString("F2");

            // Call ConfigureAnchor and get the instantiated rain effect with 90 degrees rotation on the X-axis
            // GameObject rainInstance = rainAnchorController.ConfigureAnchor("RainAnchor", spawnPoint, Quaternion.Euler(90, 0, 0));
            // Call SetAnchor with the unique name
            GameObject rainInstance = rainAnchorController.SetAnchor(anchorName, spawnPoint, Quaternion.identity);
            // Detach rainInstance from any parent to ensure it stays fixed in space
            // rainInstance.transform.parent = null;
            // Set Rigidbody to kinematic if it exists
            //Rigidbody rb = rainInstance.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.isKinematic = true; // Prevents the rain from being affected by physics
            //}

            // Log position for debugging
            Debug.Log($"Rain Instance Position after anchoring: {rainInstance.transform.position}");

            if (rainSound != null)
            {
                rainSound.Play();
            }

            // Setup the rain collider as a trigger for the instantiated rain instance
            //Collider rainCollider = rainInstance.GetComponent<Collider>();
            //if (rainCollider != null)
            //{
            //    rainCollider.isTrigger = true;  // Ensure the collider is set as a trigger
            //    Debug.Log("Rain collider set as trigger.");
            //}
            //else
            //{
            //    Debug.LogError("Rain effect does not have a collider attached.");
            //}

            // Destroy the rain bomb after collision
            Destroy(collision.gameObject);
        }
    }
}
