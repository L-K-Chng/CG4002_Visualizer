using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class RainBombScript : MonoBehaviour
{
    // for anchoring
    public Button rainBombButton;

    public GameObject rainBombPrefab;
    public GameObject rainPrefab;
    public Transform launchPoint; // Point from where the rainbomb is shot
    public AudioSource rainBombThrowSound; // Sound of launching the rainbomb
    public AudioSource outOfRainBombSound;
    public AudioSource rainSound;
    public float forwardForce = 10f; // Forward force for the rainbomb
    // public float upwardForce = 5f; // Upward force for the rainbomb
    public float downwardForce = 0.1f; // Downward force for the rainbomb
    public float rollTorque = 50f; // Torque for rolling the rainbomb
    public float projectileTravelTime = 1.5f;
    public int rainBombCount;
    public int maxRainBombs = 2;
    public TMP_Text rainBombText;
    public GameObject outOfRainBombsText;

    // Start is called before the first frame update
    void Start()
    {
        outOfRainBombsText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOutOfRainBombsText()
    {
        StartCoroutine(OutOfRainBombsDuration());
    }

    public IEnumerator OutOfRainBombsDuration()
    {
        outOfRainBombsText.SetActive(true);
        yield return new WaitForSeconds(2f);
        outOfRainBombsText.SetActive(false);
    }

    public void PlayOutOfRainBombSound()
    {
        outOfRainBombSound.Play();
    }

    public void ButtonCooldown()
    {
        StartCoroutine(rainBombCooldown());
    }

    private IEnumerator rainBombCooldown()
    {
        rainBombButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        rainBombButton.interactable = true;
    }

    public void LaunchRainBomb(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - launchPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(launchPoint.position, enemyPosition);

        // Calculate force required to hit target in 1.5s
        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second.

        // Instantiate the rainbomb prefab
        GameObject rainBomb = Instantiate(rainBombPrefab, launchPoint.position, launchPoint.rotation);
        Rigidbody rb = rainBomb.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                // destroy the projectile simultaneously as it hits the target.
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                Destroy(rainBomb, 1.4f);
            }
            else
            {
                // Vector3 force = launchPoint.forward * forwardForce + launchPoint.up * upwardForce;
                Vector3 force = launchPoint.forward * forwardForce - launchPoint.up * downwardForce;
                rb.AddForce(force, ForceMode.Impulse);
                Destroy(rainBomb, 1.4f);
            }

            // Play the rainbomb sound
            if (rainBombThrowSound != null)
            {
                rainBombThrowSound.Play();
            }
        }
    }
    /*
    public void DisplayRainEffect(Vector3 enemyPosition)
    {
        float verticalOffset = 6f;
        // dont know why but here the vertical offset is z instead of y.
        Vector3 rainEffectPosition = new Vector3(enemyPosition.x, enemyPosition.y, enemyPosition.z + verticalOffset);

        // Instantiate the rain effect at the enemy's position
        GameObject rainInstance = Instantiate(rainPrefab, rainEffectPosition, Quaternion.Euler(90, 0, 0));
        // GameObject rainInstance = Instantiate(rainPrefab, rainEffectPosition, Quaternion.identity);
        rainSound.Play();
    }
    */
}
