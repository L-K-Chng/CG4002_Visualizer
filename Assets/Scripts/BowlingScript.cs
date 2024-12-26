using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlingScript : MonoBehaviour
{
    public Button bowlingButton;

    public GameObject bowlingBallPrefab;
    public GameObject explosionPrefab;
    public GameObject playerShield;
    public GameObject player;
    public Transform rollPoint;
    public AudioSource bowlingSound;
    public AudioSource bowlingStrikeSound;
    public float rollForce = 40f;
    public float rollTorque = 60f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;

    public void RollBowlingBall(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - rollPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(rollPoint.position, enemyPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject bowlingball = Instantiate(bowlingBallPrefab, rollPoint.position, rollPoint.rotation);
        Rigidbody rb = bowlingball.GetComponent<Rigidbody>();
        // make sure the bowling ball don't collide with the player shield.
        Collider shieldCollider = playerShield.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Collider bowlingBallCollider = bowlingball.GetComponent <Collider>();
        if (shieldCollider != null && bowlingBallCollider != null)
        {
            Physics.IgnoreCollision(shieldCollider, bowlingBallCollider);
        }
        if (playerCollider != null && bowlingBallCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, bowlingBallCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
            }
            else
            {
                // Shoot straight ahead if no target detected
                rb.AddForce(rollPoint.forward * rollForce, ForceMode.Impulse);
            }

            // Apply torque for rolling effect
            rb.AddTorque(rollPoint.right * rollTorque, ForceMode.Impulse);
            // Play the soccerball shot sound
            if (bowlingSound != null)
            {
                bowlingSound.Play();
            }

            // Destroy the soccerball after 3 seconds
            Destroy(bowlingball, 3f);
        }
    }

    public void ButtonCooldown()
    {
        StartCoroutine(bowlingCooldown());
    }

    private IEnumerator bowlingCooldown()
    {
        bowlingButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        bowlingButton.interactable = true;
    }
}
