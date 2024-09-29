using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoccerBallScript : MonoBehaviour
{
    public Button soccerBallButton;
    public GameObject soccerballPrefab;
    public GameObject explosionPrefab;
    public Transform kickPoint;
    public AudioSource soccerballKickSound;
    public AudioSource soccerballExplosionSound;
    public float kickForce = 11f;
    public float rollTorque = 100f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KickSoccerball(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - kickPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(kickPoint.position, enemyPosition);

        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second
        GameObject soccerball = Instantiate(soccerballPrefab, kickPoint.position, kickPoint.rotation);
        Rigidbody rb = soccerball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                Destroy(soccerball, 1.5f);
            }
            else
            {
                // Shoot straight ahead if no target detected
                rb.AddForce(kickPoint.forward * kickForce, ForceMode.Impulse);
                Destroy(soccerball, 2.5f);
            }

            // Apply torque for rolling effect
            rb.AddTorque(kickPoint.right * rollTorque, ForceMode.Impulse);
            // Play the soccerball shot sound
            if (soccerballKickSound != null)
            {
                soccerballKickSound.Play();
            }            
        }
    }

    public void DisplayExplosion(Vector3 enemyPosition)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, enemyPosition, Quaternion.identity);
        soccerballExplosionSound.Play();
        Destroy(explosionInstance, 1f);
    }

    public void ButtonCooldown()
    {
        StartCoroutine(soccerBallKickCooldown());
    }

    private IEnumerator soccerBallKickCooldown()
    {
        soccerBallButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        soccerBallButton.interactable = true;
    }
}
