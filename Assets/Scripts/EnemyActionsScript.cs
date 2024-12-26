using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionsScript : MonoBehaviour
{
    public GameObject enemyVolleyballPrefab;
    public GameObject enemyBowlingBallPrefab;
    public GameObject enemyBasketballPrefab;
    public GameObject enemySoccerBallPrefab;
    public GameObject enemyBulletPrefab;
    public GameObject enemyRainBombPrefab;
    public GameObject enemySmokePrefab;

    public GameObject enemyShield;
    public GameObject enemy;
    public GameObject explosionPrefab;

    public float rollTorque = 80f;
    public float projectileTravelTime = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        enemyShield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Forward direction: Y
     * Leftward/rightward direction: X
     * upward/downward direction: Z
     * 
     */

    public void EnemyFireBullet(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 firingPosition = enemyPosition + new Vector3(-0.8f, 1.0f, 0);
        Vector3 firingPosition = enemyPosition + new Vector3(-0.05f, 0.01f, 0);
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - firingPosition).normalized;
        //float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float distanceToTarget = Vector3.Distance(firingPosition, playerPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject bullet = Instantiate(enemyBulletPrefab, firingPosition, Quaternion.identity);
        GameObject smoke = Instantiate(enemySmokePrefab, firingPosition, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemyBulletCollider = bullet.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemyBulletCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemyBulletCollider);
        }
        if (enemyCollider != null && enemyBulletCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemyBulletCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                // Apply torque to make the ball roll toward the player
                rb.AddTorque(-bullet.transform.right * rollTorque, ForceMode.Impulse); // Torque around the 'right' axis to create a rolling effect
                Destroy(bullet, 2.5f);
                Destroy(smoke, 2.5f);
            }
            else
            {
                // destroy the volleyball object if there is no enemy in scene.
                Destroy(bullet);
                Destroy(smoke);
            }
        }
    }

    public void EnemyLaunchRainBomb(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 launchPosition = enemyPosition + new Vector3(1f, 1.0f, 1.5f);
        Vector3 launchPosition = enemyPosition + new Vector3(0.03f, 0.01f, 0.08f);
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - launchPosition).normalized;
        //float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float distanceToTarget = Vector3.Distance(launchPosition, playerPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject rainBomb = Instantiate(enemyRainBombPrefab, launchPosition, Quaternion.identity);
        Rigidbody rb = rainBomb.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemyRainBombCollider = rainBomb.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemyRainBombCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemyRainBombCollider);
        }
        if (enemyCollider != null && enemyRainBombCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemyRainBombCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                Destroy(rainBomb, 2.5f);
            }
            else
            {
                // destroy the volleyball object if there is no enemy in scene.
                Destroy(rainBomb);
            }
        }
    }

    public void EnemyHitVolleyball(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 hitPosition = enemyPosition + new Vector3(0, 1.0f, 1.0f);
        Vector3 hitPosition = enemyPosition + new Vector3(0, 0.01f, 0.01f);
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - hitPosition).normalized;
        //float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float distanceToTarget = Vector3.Distance(hitPosition, playerPosition);
        float gravity = Physics.gravity.magnitude;

        // Calculate the horizontal speed by dividing distance by projectile travel time
        float hSpeed = distanceToTarget / projectileTravelTime;

        // adjusted vertical speed calc based on height diff
        float deltaY = playerPosition.y - hitPosition.y;
        float vSpeed = (deltaY / projectileTravelTime) + 0.5f * gravity * projectileTravelTime;

        // float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject volleyball = Instantiate(enemyVolleyballPrefab, hitPosition, Quaternion.identity);
        Rigidbody rb = volleyball.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemyVolleyballCollider = volleyball.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemyVolleyballCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemyVolleyballCollider);
        }
        if (enemyCollider != null && enemyVolleyballCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemyVolleyballCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                // rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                rb.velocity = new Vector3(direction.x * hSpeed, vSpeed, direction.z * hSpeed);
                rb.AddTorque(-volleyball.transform.right * rollTorque, ForceMode.Impulse); // Torque around the 'right' axis to create a rolling effect
                Destroy(volleyball, 2.5f);
            }
            else
            {
                // destroy the volleyball object if there is no enemy in scene.
                Destroy(volleyball);
            }
        }
    }

    public void EnemyShootBasketball(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 shootPosition = enemyPosition + new Vector3(0, 1.0f, 1.0f);
        Vector3 shootPosition = enemyPosition + new Vector3(0, 0.01f, 0.03f);
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - shootPosition).normalized;
        float distanceToTarget = Vector3.Distance(shootPosition, playerPosition);
        float gravity = Physics.gravity.magnitude;

        // Calculate horizontal speed by dividing distance by projectile travel time
        float hSpeed = distanceToTarget / projectileTravelTime;

        // Adjusted vertical speed calculation based on height difference
        float deltaY = playerPosition.y - shootPosition.y;
        float vSpeed = (deltaY / projectileTravelTime) + 0.5f * gravity * projectileTravelTime;

        GameObject basketball = Instantiate(enemyBasketballPrefab, shootPosition, Quaternion.identity);
        Rigidbody rb = basketball.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemyBasketballCollider = basketball.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemyBasketballCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemyBasketballCollider);
        }
        if (enemyCollider != null && enemyBasketballCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemyBasketballCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // destroy after 1.5s
                rb.velocity = new Vector3(direction.x * hSpeed, vSpeed, direction.z * hSpeed);
                rb.AddTorque(-basketball.transform.right * rollTorque, ForceMode.Impulse); // Torque around the 'right' axis to create a rolling effect
                Destroy(basketball, 2.5f);
            }
            else
            {
                Destroy(basketball);
            }
        }
    }

    public void EnemyKickSoccerBall(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 kickPosition = enemyPosition + new Vector3(0.2f, 1.0f, -3.6f);
        Vector3 kickPosition = enemyPosition + new Vector3(0.01f, 0.01f, -0.08f);

        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - kickPosition).normalized;
        //float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float distanceToTarget = Vector3.Distance(kickPosition, playerPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject soccerball = Instantiate(enemySoccerBallPrefab, kickPosition, Quaternion.identity);
        Rigidbody rb = soccerball.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemySoccerBallCollider = soccerball.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemySoccerBallCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemySoccerBallCollider);
        }
        if (enemyCollider != null && enemySoccerBallCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemySoccerBallCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                // Apply torque to make the ball roll toward the player
                rb.AddTorque(-soccerball.transform.right * rollTorque, ForceMode.Impulse); // Torque around the 'right' axis to create a rolling effect
                Destroy(soccerball, 2.5f);
            }
            else
            {
                // destroy the volleyball object if there is no enemy in scene.
                Destroy(soccerball);
            }
        }
    }

    public void EnemyRollBowlingBall(Vector3 enemyPosition, Vector3 playerPosition)
    {
        // Vector3 rollPosition = enemyPosition + new Vector3(1.6f, 1.0f, -1.5f);
        Vector3 rollPosition = enemyPosition + new Vector3(0.05f, 0.01f, -0.05f);
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (playerPosition - rollPosition).normalized;
        //float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float distanceToTarget = Vector3.Distance(rollPosition, playerPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject bowlingball = Instantiate(enemyBowlingBallPrefab, rollPosition, Quaternion.identity);
        Rigidbody rb = bowlingball.GetComponent<Rigidbody>();

        // Ignore collision
        Collider enemyShieldCollider = enemyShield.GetComponent<Collider>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        Collider enemyBowlingBallCollider = bowlingball.GetComponent<Collider>();
        if (enemyShieldCollider != null && enemyBowlingBallCollider != null)
        {
            Physics.IgnoreCollision(enemyShieldCollider, enemyBowlingBallCollider);
        }
        if (enemyCollider != null && enemyBowlingBallCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, enemyBowlingBallCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                // Apply torque to make the ball roll toward the player
                rb.AddTorque(-bowlingball.transform.right * rollTorque, ForceMode.Impulse); // Torque around the 'right' axis to create a rolling effect
                Destroy(bowlingball, 2.5f);
            }
            else
            {
                // destroy the volleyball object if there is no enemy in scene.
                Destroy(bowlingball);
            }
        }
    }

    public void enemyUseShield()
    {
        enemyShield.SetActive(true);
    }

    public void enemyShieldDeactivated()
    {
        enemyShield.SetActive(false);
    }    

}
