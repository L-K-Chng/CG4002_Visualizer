using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewGunScript : MonoBehaviour
{
    public Button fireButton;
    public Button reloadButton;

    public GameObject smokePrefab; // Reference to the smoke prefab
    public GameObject bulletPrefab;
    public GameObject reloadErrorText;
    public GameObject outOfAmmoText;
    public PlayerScript playerScript;

    public Transform smokePoint;
    public Transform shootPoint;
    public AudioSource firingSound;
    public AudioSource reloadSound;
    public AudioSource outOfAmmoSound;
    public float shootForce = 15f;
    public float rollTorque = 100f; // Torque for rolling the ball
    public float projectileTravelTime = 1.5f;
    public int maxBullets = 6;
   

    // To be completed: Recoil
    /*
    private Quaternion originalRotation;
    public float recoilAngle = 5f; // Angle in degrees
    public float recoilSpeed = 0.1f; // Time to complete recoil
    */

    // Start is called before the first frame update
    void Start()
    {
        reloadErrorText.SetActive(false);
        outOfAmmoText.SetActive(false);
    }

    // Set the target to be guaranteed hit after 1.5s.
    public void Shoot(Vector3 enemyPosition)
    {
        // Calculate the distance between the shooter and the enemy
        Vector3 direction = (enemyPosition - shootPoint.position).normalized;
        float distanceToTarget = Vector3.Distance(shootPoint.position, enemyPosition);
        float adjustedForce = distanceToTarget / projectileTravelTime; // Adjust force to hit target in 1 second

        // Instantiate smoke and bullet
        GameObject smokeInstance = Instantiate(smokePrefab, smokePoint.position, smokePoint.rotation);
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                // destroy the projectile simultaneously as it hits the target.
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
                Destroy(bullet, 1.1f);
            }
            else
            {
                // Shoot straight ahead if no target detected
                rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
                Destroy(bullet, 3f);
            }

            // Apply torque for rolling effect
            rb.AddTorque(shootPoint.right * rollTorque, ForceMode.Impulse);
        }

        // Play the firing sound
        if (firingSound != null)
        {
            firingSound.Play();
        }
        Destroy(smokeInstance, 2.5f);
    }
    
    public IEnumerator ReloadDuration()
    {
        // updates the bullet count on the UI only upon completion of the reload sound
        yield return new WaitForSeconds(reloadSound.clip.length);

        // UPDATED HERE.
        //currentBullets.text = maxBullets.ToString();
        playerScript.playerBulletsText.text = maxBullets.ToString();
    }

    // play reload sound and only display bullets after reload complete
    public void GunReload()
    {
        reloadSound.Play();
        StartCoroutine(ReloadDuration());
    }

    // play sound upon out of ammo/trying to reload without ammo
    public void OutOfAmmoSound()
    {
        outOfAmmoSound.Play();
    }

    public void ShowReloadErrorText()
    {
        StartCoroutine(ReloadErrorTextDuration());
    }

    public void ShowOutOfAmmoText()
    {
        StartCoroutine(OutOfAmmoTextDuration());
    }

    public void FiringButtonCooldown()
    {
        StartCoroutine(FiringCooldown());
    }

    public void ReloadButtonCooldown()
    {
        StartCoroutine(ReloadCooldown());
    }

    // also try to make the gun tilt up and then down to simulate the recoil of the gun.

    private IEnumerator FiringCooldown()
    {
        fireButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        fireButton.interactable = true;
    }

    private IEnumerator ReloadCooldown()
    {
        reloadButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        reloadButton.interactable = true;
    }

    private IEnumerator OutOfAmmoTextDuration()
    {
        outOfAmmoText.SetActive(true);
        yield return new WaitForSeconds(2f);
        outOfAmmoText.SetActive(false);
    }

    private IEnumerator ReloadErrorTextDuration()
    {
        reloadErrorText.SetActive(true);
        yield return new WaitForSeconds(2f);
        reloadErrorText.SetActive(false);
    }
}