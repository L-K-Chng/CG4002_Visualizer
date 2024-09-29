using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * For detecting projectile collisions with the enemy. To be placed on the image target.
 * 
 * 
*/

public class EnemyCollisionScript : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameManager gameManager;

    // Testing for Rain Bomb effect.
    public GameObject rainPrefab;
    public AudioSource rainSound;
    private float spawnOffsetZ = 3f;
    public RainBombManager rainBombManager;
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
            gameManager.OnAIActionImpactWithoutShield();

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
            gameManager.OnAIActionImpactWithoutShield();

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
            gameManager.OnAIActionImpactWithoutShield();

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
            gameManager.OnAIActionImpactWithoutShield();

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
            gameManager.OnBulletImpactWithoutShield();

            // Optionally, destroy the basketball upon collision
            Destroy(collision.gameObject);
        }


        // Testing for Rain Bomb
        /*
        if (collision.gameObject.tag == "RainBomb")
        {
            Debug.Log("Rain bomb hit target");
            // Get the point of collision 
            Vector3 collisionPoint = collision.contacts[0].point;

            GameObject rb = rainBombManager.DisplayRain(collisionPoint);

            // Set the rainbombs parent to be the enemy, and then set z upwards, and then reset the parent. 
            // rb.transform.parent = (collision.gameObject.transform);
            rb.transform.parent = (gameObject.transform);

            Vector3 enemyPosition = gameObject.transform.position;
            enemyPosition.z += spawnOffsetZ;
            rb.transform.position = enemyPosition;
            rb.transform.parent = null;

            if (rainSound != null)
            {
                rainSound.Play();
            }

            gameManager.OnBulletImpactWithoutShield();
            // Destroy the rain bomb after collision, instead of the enemy target.
            Destroy(collision.gameObject);
            // call the function once rain bomb hits the target. deal damage.
            gameManager.OnRainBombImpact();
        }
        */


        // Testing for Rain Bomb
        if (collision.gameObject.tag == "RainBomb")
        {
            Debug.Log("Rain bomb hit target");
            // Get the point of collision 
            Vector3 collisionPoint = collision.contacts[0].point;

            GameObject rb = rainBombManager.DisplayRain(collisionPoint);

            // Set the rainbomb's parent to be the enemy
            rb.transform.parent = gameObject.transform;

            // Adjust the rain bomb's position with the Z offset
            Vector3 enemyPosition = gameObject.transform.position;
            enemyPosition.z += spawnOffsetZ;
            rb.transform.position = enemyPosition;

            // Store the world position before detaching from the parent
            Vector3 worldPosition = rb.transform.position;

            // Detach the rain bomb from the enemy
            rb.transform.parent = null;

            // Set the world position after detaching to ensure it stays in place
            rb.transform.position = worldPosition;

            if (rainSound != null)
            {
                rainSound.Play();
            }

            gameManager.OnBulletImpactWithoutShield();

            // Destroy the rain bomb after collision, instead of the enemy target.
            Destroy(collision.gameObject);

            // Call the function once rain bomb hits the target to deal damage.
            // gameManager.OnRainBombImpact();
        }

    }
}
