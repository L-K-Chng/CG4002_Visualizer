using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolleyballScript : MonoBehaviour
{
    public Button volleyballButton;
    public GameObject volleyballPrefab;
    public GameObject explosionPrefab;
    public Transform hitPoint;
    public AudioSource volleyballHitSound;
    public AudioSource volleyballExplosionSound;
    public float forwardForce = 25f; // Forward force for the basketball
    public float rollTorque = 80f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitVolleyball(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - hitPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(hitPoint.position, enemyPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject volleyball = Instantiate(volleyballPrefab, hitPoint.position, hitPoint.rotation);
        Rigidbody rb = volleyball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                Destroy(volleyball, 1.5f);
            }
            else
            {
                // Apply the force to the Rigidbody
                rb.AddForce(hitPoint.forward * forwardForce, ForceMode.Impulse);
                Destroy(volleyball, 2.5f);
            }

            // Apply torque for rolling effect
            rb.AddTorque(hitPoint.right * rollTorque, ForceMode.Impulse);
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
