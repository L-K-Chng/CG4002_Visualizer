using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/*
 * Shield Logic:
 * Each shield has 30 hp
 * Start by setting it to inactive.
 * Upon pressing the shield button then set to active with 30 shield hp
 * no need timer.
 */

public class ShieldScript : MonoBehaviour
{
    public Button shieldButton;
    public GameObject shieldPrefab;
    public AudioSource shieldSound;
    public float shieldDuration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // edited here
        // shieldPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // checks the shield hp and number of shields to ensure that player can use the shield.
    public bool UseShield(int hp, int nshields)
    {
        if (hp == 0 && nshields != 0)
        {
            shieldSound.Play();
            shieldPrefab.SetActive(true);
            return true;
        }
        return false;
    }

    // CHANGED THIS PART
    /*
    public void UseShield()
    {
        StartCoroutine(ShieldEffect());
    }
    */
    

    public bool IsPlayerShieldActive()
    {
        return shieldPrefab.activeSelf;
    }

    public IEnumerator ShieldEffect()
    {
        shieldSound.Play();
        shieldPrefab.SetActive(true);
        yield return new WaitForSeconds(shieldDuration);
        shieldPrefab.SetActive(false);
    }

    // set to 3 seconds for shield.
    private IEnumerator shieldCooldown()
    {
        shieldButton.interactable = false;
        yield return new WaitForSeconds(3f);
        shieldButton.interactable = true;
    }

    public void ButtonCooldown()
    {
        StartCoroutine(shieldCooldown());
    }
}
