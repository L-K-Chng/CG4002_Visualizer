using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controls the gameplay of the enemy within the game.
/*
 * Controls the gameplay of the enemy within the game.
 * Enemy deals damage to the player health and the player shield, while also lowering their shield count.
 * Enemy manager also controls the enemy ammo, rain bomb count and score.
 * Here, we should control: 
 * Enemy ammo, rain bomb count, score
 * Player health, shield health and shield count
*/

public class EnemyManagerScript : MonoBehaviour
{
    // Enemy buttons
    public Button enemyFireButton;
    public Button enemyReloadButton;
    public Button enemyRainBombButton;
    public Button enemyShieldButton;
    // List of enemy buttons
    public List<Button> enemySpecialActionButtons;
    public List<Button> enemyGenericActionButtons;
    public GameObject enemyShieldPrefab;
    // Effects for when a player gets hit. Display blood and a corresponding grunt sound
    public AudioSource playerHurtSound;
    public GameObject playerBloodPrefab;

    // Required scripts
    public EnemyUIScript enemyUIScript;
    public PlayerScript playerScript;
    public ShieldScript shieldScript;
    public GameManager gameManager;

    // Initialisation values
    public int maxHp = 100;
    public int maxShieldHp = 30;
    public int maxShields = 3;
    public int maxBullets = 6;
    public int maxRainBombs = 2;
    public int initialScore = 0;

    // In game values
    // FOR PLAYER
    public int playerHp;
    public int playerShieldHp;
    public int playerShieldCount;
    // FOR ENEMY
    public int ammoCount;
    public int rainBombCount;
    public int enemyScore;

    public int genericActionDamage = 5;
    public int specialActionDamage = 10;


    void Start()
    {
        enemyShieldPrefab.SetActive(false);
        enemyShieldButton.onClick.AddListener(OnEnemyShieldButtonPressed);
        enemyReloadButton.onClick.AddListener(OnEnemyReloadButtonPressed);
        enemyFireButton.onClick.AddListener(OnEnemyFireButtonPressed);
        enemyRainBombButton.onClick.AddListener(OnEnemyRainBombButtonPressed);
        foreach (Button button in enemySpecialActionButtons)
        {
            button.onClick.AddListener(() => OnEnemySpecialActionButtonPressed(button));
        }
        
        // to set the player stats. MAY BE UPDATED.
        // Initialise the player UI as well as the enemy score.
        playerHp = maxHp;
        playerShieldHp = 0;
        playerShieldCount = maxShields;
        ammoCount = maxBullets;
        rainBombCount = maxRainBombs;
        enemyScore = initialScore;
        playerScript.SetMaxHp();
        playerScript.SetShieldHp(playerShieldHp);
        playerScript.SetPlayerMaxShields();
        enemyUIScript.SetEnemyMaxBullets();
        enemyUIScript.SetEnemyMaxRainBombs();
        enemyUIScript.UpdateEnemyScore(enemyScore);

        playerBloodPrefab.SetActive(false);
        shieldScript.shieldPrefab.SetActive(false);
    }

    void Update()
    {
        // Check for whether the player respawns in every frame
        // Actively listens for whether the player's health/shield is at 0. if at 0, then respawns the players hp/shield.
        if (playerScript.CheckPlayerRespawn())
        {
            // UPON DEATH OF PLAYER WE NEED TO UPDATE:
            // 1. PLAYER HP
            // 2. PLAYER SHIELD HP
            // 3. PLAYER SHIELD COUNT
            // 4. PLAYER AMMO COUNT (IN GAME MANAGER FUNCTION)
            // 5. PLAYER RAIN BOMB COUNT (IN GAME MANAGER FUNCTION)
            // 6. ENEMY SCORE
            // EDITED THE PLAYER SHIELD HP ONCE AGAIN. CHANGED THE HP TO BE 0
            Debug.Log("Player Respawned.");
            playerHp = maxHp;
            playerShieldHp = 0;
            playerShieldCount = maxShields;
            enemyScore++;
            playerScript.SetMaxHp();
            playerScript.SetShieldHp(playerShieldHp);
            playerScript.SetPlayerMaxShields();
            enemyUIScript.UpdateEnemyScore(enemyScore);
            gameManager.RefreshPlayerAmmunition();
        }

        /*
        // Check whether the player's shield is refreshed in every frame
        // dont refresh the shield Hp if the enemy has used up their last shield
        // ORIGINAL CODE. MAKE SURE TO CHANGE BACK IF NOT WORKING!!!!
        if (playerScript.CheckPlayerShieldRefresh())
        {
            playerShieldHp = maxShieldHp;
            playerScript.SetMaxShieldHp();
            playerShieldCount--;
            playerScript.SetPlayerShields(playerShieldCount);
        }
        */

        // Update function ensures that if player shield hp is 0, the shield prefab automatically disappears.
        // if (playerShieldHp == 0)
        if (playerScript.GetPlayerShieldHp() == 0)
        {
            shieldScript.shieldPrefab.SetActive(false);
        }

        // instantly stop the shield from popping up once the enemy shield hp and count are both at 0.
        if ((enemyUIScript.GetEnemyShieldCount() == 0) && enemyUIScript.GetEnemyShieldHp() == 0)
        {
            enemyShieldPrefab.SetActive(false);
        }
    }

    public void RefreshEnemyAmmunition()
    {
        // Refresh the enemy ammunition upon their respawn
        ammoCount = maxBullets;
        rainBombCount = maxRainBombs;
        enemyUIScript.SetEnemyMaxBullets();
        enemyUIScript.SetEnemyMaxRainBombs();
    }

    // Detects whether the enemy's shield is active when a player attack is done
    public bool IsEnemyShieldActive()
    {
        return enemyShieldPrefab.activeSelf;
    }

    // shield button cooldown 3s
    public void OnEnemyShieldButtonPressed()
    {
        // StartCoroutine(ButtonCooldown(enemyShieldButton));
        StartCoroutine(EnemyShieldButtonCooldown());
        enemyShieldEffect();
    }

    public void enemyShieldEffect()
    {
        if (enemyUIScript.GetEnemyShieldCount() == 0 && enemyUIScript.GetEnemyShieldHp() == 0)
        {
            return;
        }
        else
        {
            if (gameManager.enemyShieldHp == 0)
            {
                // enemy activates their shield if their shield hp is 0.
                enemyShieldPrefab.SetActive(true);
                gameManager.enemyShieldCount--;
                gameManager.enemyShieldHp = maxShieldHp;
                enemyUIScript.SetEnemyShieldHp(gameManager.enemyShieldHp);
                enemyUIScript.SetEnemyShields(gameManager.enemyShieldCount);
            }
        }
    }

    // First, before action we: need to detect whether the enemy shield is active. and if it is, we need to damage the shield
    // instead of the enemy hp
    public void OnEnemySpecialActionButtonPressed(Button button)
    {
        StartCoroutine(ButtonCooldown(button));
        if (shieldScript.IsPlayerShieldActive())
        {
            playerShieldHp -= specialActionDamage;
            playerScript.SetShieldHp(playerShieldHp);
        }
        else
        {
            playerHurtSound.Play();
            //playerBloodPrefab.SetActive(true);
            playerBloodSplatterEffect();
            playerHp -= specialActionDamage;
            playerScript.SetHp(playerHp);
        }
    }

    public void OnEnemyFireButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyFireButton));
        if (enemyUIScript.GetEnemyBulletCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                playerShieldHp -= genericActionDamage;
                playerScript.SetShieldHp(playerShieldHp);
            }
            else
            {
                playerHurtSound.Play();
                //playerBloodPrefab.SetActive(true);
                playerBloodSplatterEffect();
                playerHp -= genericActionDamage;
                playerScript.SetHp(playerHp);
            }
            // upon firing, reduce ammo by 1 each time.
            ammoCount--;
            enemyUIScript.UpdateBulletCount(ammoCount);
        }
    }

    public void OnEnemyRainBombButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyRainBombButton));
        if (enemyUIScript.GetEnemyRainBombCount() != 0)
        {
            if (shieldScript.IsPlayerShieldActive())
            {
                playerShieldHp -= genericActionDamage;
                playerScript.SetShieldHp(playerShieldHp);
            }
            else
            {
                playerHurtSound.Play();
                //playerBloodPrefab.SetActive(true);
                playerBloodSplatterEffect();
                playerHp -= genericActionDamage;
                playerScript.SetHp(playerHp);
            }
            // Reduce rain bombs by 1 each time they are used.
            rainBombCount--;
            enemyUIScript.UpdateRainBombCount(rainBombCount);
        }
    }

    public void OnEnemyReloadButtonPressed()
    {
        // only reload if ammo is at 0.
        StartCoroutine(ButtonCooldown(enemyReloadButton));
        if (ammoCount == 0)
        {
            enemyUIScript.SetEnemyMaxBullets();
            ammoCount = maxBullets;
        }
    }

    // Controls the enemy buttons
    private IEnumerator ButtonCooldown(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(1.5f);
        button.interactable = true;
    }

    // shield button is separate as the cooldown is longer.
    private IEnumerator EnemyShieldButtonCooldown()
    {
        enemyShieldButton.interactable = false;
        yield return new WaitForSeconds(3.2f);
        enemyShieldButton.interactable = true;
    }

    // for displaying the player's blood upon getting hit
    public void playerBloodSplatterEffect()
    {
        StartCoroutine(playerBloodSplatterDuration());
    }

    private IEnumerator playerBloodSplatterDuration()
    {
        yield return new WaitForSeconds(0.2f);
        playerBloodPrefab.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        playerBloodPrefab.SetActive(false);
    }
}
