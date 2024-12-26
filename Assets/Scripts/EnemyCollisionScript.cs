using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using Vuforia;

/*
 * For detecting projectile collisions with the enemy. To be placed on the image target.
 * 
*/

public class EnemyCollisionScript : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject rainBombExplosionEffect;
    public GameManager gameManager;
    public GameObject enemy;

    // Testing for Rain Bomb effect.
    public GameObject rainPrefab;
    public AudioSource rainSound;
    // private float spawnOffsetZ = 3f;
    public RainBombManager rainBombManager;
    public EnemyUIScript enemyUIScript;
    public BloodScript bloodScript;
    // Testing

    public AudioSource basketballExplosionSound;
    public AudioSource soccerballExplosionSound;
    public AudioSource volleyballExplosionSound;
    public AudioSource bowlingballExplosionSound;

    public RainAnchorController rainAnchorController;

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
            // gameManager.OnAIActionImpactWithoutShield();
            bloodScript.BloodSplatterEffect();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "SoccerBall")
        {
            Debug.Log("Soccerball hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            soccerballExplosionSound.Play();
            // gameManager.OnAIActionImpactWithoutShield();
            bloodScript.BloodSplatterEffect();
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
            // gameManager.OnAIActionImpactWithoutShield();
            bloodScript.BloodSplatterEffect();
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
            // gameManager.OnAIActionImpactWithoutShield();
            bloodScript.BloodSplatterEffect();
            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Bullet hit target");
            Vector3 collisionPoint = collision.contacts[0].point;

            // Instantiate explosion effect at the collision point and apply rotation if needed
            // gameManager.OnBulletImpactWithoutShield();
            bloodScript.BloodSplatterEffect();
            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }


        /*
        if (collision.gameObject.CompareTag("RainBomb"))
        {
            Debug.Log("Rain bomb hit target");

            // Get the point of collision 
            Vector3 collisionPoint = collision.contacts[0].point;

            // Define a height offset
            float heightOffset = 5f; // Adjust this value as needed

            // Create a new point that is higher in the Y-axis
            Vector3 spawnPoint = new Vector3(collisionPoint.x, collisionPoint.y + heightOffset, collisionPoint.z);

            // Create unique name
            string anchorName = "RainAnchor_" + Time.time.ToString("F2");


            // Call ConfigureAnchor
            GameObject rainInstance = rainAnchorController.SetAnchor(anchorName, spawnPoint, Quaternion.Euler(90, 0, 0));
            // Call SetAnchor with the unique name
            //GameObject rainInstance = rainAnchorController.SetAnchor(anchorName, spawnPoint, Quaternion.identity);

            // Ensure the rain instance is not affected by physics

            Rigidbody rb = rainInstance.GetComponent<Rigidbody>();
            rainInstance.transform.parent = null;
            if (rb != null)
            {
                rb.isKinematic = true; // Prevents the rain from being affected by physics
            }

            if (rainSound != null)
            {
                rainSound.Play();
            }

            // Setup the rain collider as a trigger for the instantiated rain instance
            Collider rainCollider = rainInstance.GetComponent<Collider>();
            
            if (rainCollider != null)
            {
                rainCollider.isTrigger = true;  // Ensure the collider is set as a trigger
                Debug.Log("Rain collider set as trigger.");
            }
            
            gameManager.OnBulletImpactWithoutShield();

            // Destroy the rain bomb after collision
            Destroy(collision.gameObject);
        }*/

        if (collision.gameObject.CompareTag("RainBomb"))
        {
            Debug.Log("Rain bomb hit target");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(rainBombExplosionEffect, collisionPoint, Quaternion.identity);

            // Get the point of collision 
            Vector3 enemyPosition = enemyUIScript.GetEnemyPosition();

            // Define a height offset
            // original height offset: 0.1f.
            float heightOffset = 1.3f; // Cloud height is 1f so the height offset should be around here.

            Vector3 spawnPoint = new Vector3(enemyPosition.x, enemyPosition.y + heightOffset, enemyPosition.z);
            // Vector3 spawnPoint = new Vector3(enemyPosition.x, enemyPosition.y + heightOffset, enemyPosition.z + heightOffset);

            // Create unique name
            string anchorName = "RainAnchor_" + Time.time.ToString("F2");

            // GameObject rainInstance = rainAnchorController.SetAnchor(anchorName, spawnPoint, Quaternion.Euler(90, 0, 0));
            GameObject rainInstance = rainAnchorController.SetAnchor(anchorName, spawnPoint, Quaternion.identity);

            // NEW PART BELOW
            // Set the parent to the GameObject this script is attached to
            //rainInstance.transform.SetParent(this.gameObject.transform, false); // Set rain instance as a child of this GameObject
            // NEW PART ABOVE

            // Ensure the rain instance is not affected by physics
            /*
            Rigidbody rb = rainInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Prevents the rain from being affected by physics
            }*/

            // Log position after anchoring for debugging purposes
            Debug.Log($"Rain Instance anchored at: {rainInstance.transform.position}");

            if (rainSound != null)
            {
                rainSound.Play();
            }

            // Setup the rain collider as a trigger for the instantiated rain instance
            // testing removal of this collider
            //Collider rainCollider = rainInstance.GetComponent<Collider>();
            
            //if (rainCollider != null)
            //{
            //    rainCollider.isTrigger = true;  // Ensure the collider is set as a trigger
            //    Debug.Log("Rain collider set as trigger.");
            //}

            bloodScript.BloodSplatterEffect();
            Destroy(collision.gameObject);
        }
    }
}
