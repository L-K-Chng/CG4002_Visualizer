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
    public GameObject laserBulletPrefab; // for testing
    public GameObject reloadErrorText;
    public GameObject outOfBulletsText;
    public GameObject reloadingText;
    public GameObject reloadCompleteText;
    public GameObject playerShield;
    public GameObject player;

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
        outOfBulletsText.SetActive(false);
        reloadingText.SetActive(false);
        reloadCompleteText.SetActive(false);
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
        // GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        // for testing.
        GameObject bullet = Instantiate(laserBulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // make sure the bullet don't collide with the player shield.
        Collider shieldCollider = playerShield.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Collider bulletCollider = bullet.GetComponent<Collider>();
        if (shieldCollider != null && bulletCollider != null)
        {
            Physics.IgnoreCollision(shieldCollider, bulletCollider);
        }
        if (playerCollider != null && bulletCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, bulletCollider);
        }

        if (rb != null)
        {
            if (enemyPosition != Vector3.zero)
            {
                // Apply precise force to guarantee the hit
                // ORIGINAL
                rb.AddForce(direction * adjustedForce, ForceMode.Impulse);


                // TESTING
                // Calculate the direction to the enemy
                // Vector3 direction = (enemyPosition - shootPoint.position).normalized;

                // Rotate the bullet to face the target
                // bullet.transform.rotation = Quaternion.LookRotation(direction);

                // Apply force to the bullet to move towards the target
                // rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
            }
            else
            {
                // ORIGINAL
                rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);

                // TESTING
                //Vector3 adjustedForward = Quaternion.Euler(0, 0, 0) * Vector3.forward;
                //rb.AddForce(adjustedForward * shootForce, ForceMode.Impulse);
            }
            Destroy(bullet, 3f);
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

    public void ShowReloadText()
    {
        StartCoroutine(ReloadTextDuration());
    }

    // also try to make the gun tilt up and then down to simulate the recoil of the gun.

    private IEnumerator ReloadTextDuration()
    {
        Debug.Log("Starting ReloadTextDuration Coroutine");
        reloadingText.SetActive(true);
        yield return new WaitForSeconds(reloadSound.clip.length);
        Debug.Log("Reload sound finished, showing reloadCompleteText");
        reloadingText.SetActive(false);
        reloadCompleteText.SetActive(true);
        yield return new WaitForSeconds(3f);
        reloadCompleteText.SetActive(false);
    }

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
        outOfBulletsText.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        outOfBulletsText.SetActive(false);
    }

    private IEnumerator ReloadErrorTextDuration()
    {
        reloadErrorText.SetActive(true);
        yield return new WaitForSeconds(2f);
        reloadErrorText.SetActive(false);
    }
}