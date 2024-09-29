using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlingScript : MonoBehaviour
{
    public Button bowlingButton;

    public GameObject bowlingBallPrefab;
    public GameObject explosionPrefab;
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

            // Destroy the soccerball after 1.6 seconds
            Destroy(bowlingball, 1.6f);
        }
    }

    public void DisplayExplosion(Vector3 enemyPosition)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, enemyPosition, Quaternion.identity);
        bowlingStrikeSound.Play();
        Destroy(explosionInstance, 1f);
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
