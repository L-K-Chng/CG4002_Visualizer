using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasketballScript : MonoBehaviour
{
    public Button basketballButton;

    public GameObject basketballPrefab;
    public GameObject explosionPrefab;
    public Transform shootPoint; // Point from where the basketball is shot
    public AudioSource basketballShotSound; // Sound of launching the basketball
    public AudioSource basketballExplosionSound;
    public float forwardForce = 10f; // Forward force for the basketball
    public float upwardForce = 5f; // Upward force for the basketball
    public float rollTorque = 50f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;
    public float gravity = 9.81f; // Assuming standard gravity

    public void ShootBasketball(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - shootPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(shootPoint.position, enemyPosition);
        float gravity = Physics.gravity.magnitude;
        // float hSpeed = 20f;
        float totalTime = 1.5f;

        // Calculate total flight time based on horizontal speed and distance
        // float totalTime = distanceToTarget / hSpeed;
        float hSpeed = distanceToTarget / totalTime;

        // Adjusted vertical speed calculation based on height difference
        float deltaY = enemyPosition.y - shootPoint.position.y; // height difference
        float vSpeed = (deltaY / totalTime) + 0.5f * gravity * totalTime;
        // float vSpeed = (0.5f * gravity * totalTime); 

        // Instantiate the basketball prefab
        GameObject basketball = Instantiate(basketballPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = basketball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // destroy after 1.5s
                rb.velocity = new Vector3(direction.x * hSpeed, vSpeed, direction.z * hSpeed);
                Destroy(basketball, 1.5f);
            }
            else
            {
                // Calculate the force direction and magnitude
                Vector3 force = shootPoint.forward * forwardForce + shootPoint.up * upwardForce;

                // Apply the force to the Rigidbody
                rb.AddForce(force, ForceMode.Impulse);
                Destroy(basketball, 3f);
                
            }
            rb.AddTorque(shootPoint.right * rollTorque, ForceMode.Impulse);

        }

        // Play the basketball shot sound
        if (basketballShotSound != null)
        {
            basketballShotSound.Play();
        }
    }

    public void DisplayExplosion(Vector3 enemyPosition)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, enemyPosition, Quaternion.identity);
        basketballExplosionSound.Play();
        Destroy(explosionInstance, 1f);
    }

    public void ButtonCooldown()
    {
        StartCoroutine(BasketballCooldown());
    }

    private IEnumerator BasketballCooldown()
    {
        basketballButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        basketballButton.interactable = true;
    }
}
