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
    public GameObject shieldActivatedText;
    public GameObject shieldBackground;
    public AudioSource shieldSound;
    public float shieldDuration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // edited here
        shieldPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // set for player 1 and 2.
    // hp and nshields is dependent on the health and shield bars which are for either player 1 or 2
    // checks the shield hp and number of shields to ensure that player can use the shield.
    public bool UseShield(int hp, int nshields)
    {
        if (hp == 0 && nshields != 0)
        {
            shieldSound.Play();
            // shieldPrefab.SetActive(true);
            shieldBackground.SetActive(true);
            shieldActivatedText.SetActive(true);
            return true;
        }
        return false;
    }


    // 25 october new function to handle shield usage.
    public void UseShield2()
    {
        // shieldSound.Play();
        shieldPrefab.SetActive(true);
        // shieldBackground.SetActive(true);
        shieldActivatedText.SetActive(true);
    }

    public void DeactivateShield()
    {
        shieldPrefab.SetActive(false);
        shieldActivatedText.SetActive(false);
    }
    
    public bool IsPlayerShieldActive()
    {
        return shieldPrefab.activeSelf;
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
