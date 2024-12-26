using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolleyballScript : MonoBehaviour
{
    public Button volleyballButton;
    public GameObject volleyballPrefab;
    public GameObject explosionPrefab;
    public GameObject playerShield;
    public GameObject player;
    public Transform hitPoint;
    public AudioSource volleyballHitSound;
    public AudioSource volleyballExplosionSound;
    public float forwardForce = 15f; // Forward force for the basketball
    public float upwardForce = 6f;
    public float rollTorque = 80f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;

    public void HitVolleyball(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - hitPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);
        float gravity = Physics.gravity.magnitude;
        float totalTime = 1.5f;

        float hSpeed = distanceToTarget / totalTime;
        float deltaY = enemyPosition.y - hitPoint.position.y;
        float vSpeed = (deltaY / totalTime) + 0.5f * gravity * totalTime;

        // float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject volleyball = Instantiate(volleyballPrefab, hitPoint.position, hitPoint.rotation);
        Rigidbody rb = volleyball.GetComponent<Rigidbody>();
        // make sure the volleyball don't collide with the player shield.
        Collider shieldCollider = playerShield.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Collider volleyBallCollider = volleyball.GetComponent<Collider>();
        if (shieldCollider != null && volleyBallCollider != null)
        {
            Physics.IgnoreCollision(shieldCollider, volleyBallCollider);
        }
        if (playerCollider != null && volleyBallCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, volleyBallCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.velocity = new Vector3(direction.x * hSpeed, vSpeed, direction.z * hSpeed);
                // rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
            }
            else
            {
                // Apply the force to the Rigidbody
                Vector3 force = hitPoint.forward * forwardForce + hitPoint.up * upwardForce;
                // rb.AddForce(hitPoint.forward * forwardForce, ForceMode.Impulse);
                rb.AddForce(force, ForceMode.Impulse);
            }
            // Apply torque for rolling effect
            rb.AddTorque(hitPoint.right * rollTorque, ForceMode.Impulse);
            Destroy(volleyball, 2.5f);
            
            // Play the soccerball shot sound
            if (volleyballHitSound != null)
            {
                volleyballHitSound.Play();
            }
        }
    }

    public void DisplayExplosion(Vector3 enemyPosition)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, enemyPosition, Quaternion.identity);
        volleyballExplosionSound.Play();
        Destroy(explosionInstance, 1f);
    }

    public void ButtonCooldown()
    {
        StartCoroutine(volleyballCooldown());
    }

    private IEnumerator volleyballCooldown()
    {
        volleyballButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        volleyballButton.interactable = true;
    }
}
