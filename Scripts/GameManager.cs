using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls the gameplay of the player within the game.
/* 
 * Controls the gameplay of the player within the game.
 * Player deals damage to the enemy health and the enemy shield, while also lowering their shield count.
 * Game manager also controls the player ammo, player rain bomb count and player score.
 * Here, we should control: 
 * Player ammo, rain bomb count, score
 * Enemy health, shield health and shield count
*/

public class GameManager : MonoBehaviour
{
    // Scripts
    public NewGunScript gunScript;
    public BasketballScript basketballScript;
    public SoccerBallScript soccerballScript;
    public VolleyballScript volleyballScript;
    public BowlingScript bowlingScript;
    public EnemyUIScript enemyUIScript;
    public RainBombScript rainBombScript;
    public ShieldScript shieldScript;
    public PlayerScript playerScript;
    public BloodScript bloodScript;
    public EnemyManagerScript enemyManagerScript;

    // Buttons
    public Button fireButton;
    public Button reloadButton;
    public Button basketballButton;
    public Button soccerballButton;
    public Button volleyballButton;
    public Button bowlingButton;
    public Button rainBombButton;
    public Button shieldButton;

    const float timeToNextFire = 1.5f; //cooldown timer for firing.
    const float timeToProjectileImpact = 1f;
    const float timeToBombImpact = 1.5f;
    const float timeToBallImpact = 1.5f;
    
    // Initialise the health, bullets, shields and rain bombs
    public int maxHp = 100;
    public int maxShieldHp = 30;
    public int maxBullets = 6;
    public int maxShields = 3;
    public int maxRainBombs = 2;
    public int initialScore = 0;
    
    // FOR ENEMY
    public int enemyHp;
    public int enemyShieldHp;
    public int enemyShieldCount;
    // FOR PLAYER
    public int bulletCount;
    public int rainBombCount;
    public int playerScore;

    // Damage
    private int genericActionDamage = 5;
    private int specialActionDamage = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        // Add listener for buttons
        fireButton.onClick.AddListener(OnFireButtonPressed);
        reloadButton.onClick.AddListener(OnReloadButtonPressed);
        basketballButton.onClick.AddListener(OnBasketballButtonPressed);
        soccerballButton.onClick.AddListener(OnSoccerballButtonPressed);
        volleyballButton.onClick.AddListener(OnVolleyballButtonPressed);
        bowlingButton.onClick.AddListener(OnBowlingButtonPressed);
        rainBombButton.onClick.AddListener(OnRainBombButtonPressed);
        shieldButton.onClick.AddListener(OnShieldButtonPressed);

        // Initialise the max number of bullets, shields and rain bombs
        // Initialise the enemy UI, as well as our own player score.
        enemyHp = maxHp;

        // new changes here
        // enemyShieldHp = maxShieldHp;
        enemyShieldHp = 0;

        enemyShieldCount = maxShields;
        bulletCount = maxBullets;
        rainBombCount = maxRainBombs;
        playerScore = initialScore;
        enemyUIScript.SetEnemyMaxHp();

        // edits here
        // enemyUIScript.SetEnemyMaxShieldHp();
        enemyUIScript.SetEnemyShieldHp(enemyShieldHp);

        enemyUIScript.SetEnemyMaxShields();
        playerScript.SetMaxBullets();
        playerScript.SetMaxRainBomb();
        playerScript.UpdatePlayerScore(playerScore);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for whether the enemy respawns in every frame
        if (enemyUIScript.CheckEnemyRespawn())
        {    
            // TESTING
            // UPON DEATH OF ENEMY WE NEED TO UPDATE:
            // 1. ENEMY HP
            // 2. ENEMY SHIELD HP
            // 3. ENEMY SHIELD COUNT
            // 4. ENEMY AMMO COUNT (IN ENEMY MANAGER FUNCTION)
            // 5. ENEMY RAIN BOMB COUNT (IN ENEMY MANAGER FUNCTION)
            // 6. PLAYER SCORE 
            Debug.Log("Enemy Respawned.");
            enemyHp = maxHp;
            enemyShieldHp = 0;
            enemyShieldCount = maxShields;
            playerScore++;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
            enemyUIScript.SetEnemyMaxHp();
            enemyUIScript.SetEnemyMaxShields();
            enemyManagerScript.RefreshEnemyAmmunition();
            playerScript.UpdatePlayerScore(playerScore);
        }

        // Check whether the shield is refreshed in every frame
        /*
        if (enemyUIScript.CheckEnemyShieldRefresh())
        {
            enemyShieldHp = maxShieldHp;
            enemyUIScript.SetEnemyMaxShieldHp();
            enemyShieldCount--;
            enemyUIScript.SetEnemyShields(enemyShieldCount);
        }
        */

        // once shield hp hit 0, make the enemyshieldprefab disappear
        /*
        if (enemyShieldHp == 0)
        {
            enemyManagerScript.enemyShieldPrefab.SetActive(false);
        }
        */

        if (enemyUIScript.GetEnemyShieldHp() == 0)
        {
            enemyManagerScript.enemyShieldPrefab.SetActive(false);
        }

        // Instantly stops player's shield from being displayed once their shield hp and count are both at 0.
        if (playerScript.GetPlayerShieldCount() == 0 && playerScript.GetPlayerShieldHp() == 0)
        {
            shieldScript.shieldPrefab.SetActive(false);
        }
    }

    // To refresh player ammunition upon respawn. To be placed inside EnemyManager to coordinate the two manager scripts.
    public void RefreshPlayerAmmunition()
    {
        bulletCount = maxBullets;
        rainBombCount = maxRainBombs;
        playerScript.SetMaxBullets();
        playerScript.SetMaxRainBomb();
    }

    // Logic for firing action: Fires the projectile. If enemy cannot be spotted, simply shoot forward
    // Else, shoot towards the enemy, hitting them in 1.5s
    // OnBulletImpact instantiates the blood effect upon contact with the enemy and deals damage accordingly
    // ORIGINAL
    /*
    public void PlayerDoFireBullets()
    {
        if (playerScript.GetPlayerBulletCount() != 0)
        {
            gunScript.Shoot(enemyUIScript.GetEnemyPosition());

            // Enemy only take damage if they are visible on the visualizer screen
            if (enemyUIScript.isEnemyVisible)
            {
                StartCoroutine(OnBulletImpact());
            }
            bulletCount--;
            playerScript.UpdateBulletCount(bulletCount);
            gunScript.FiringButtonCooldown();
        }
        // Case for when there are no more bullets.
        else
        {
            gunScript.ShowOutOfAmmoText();
            gunScript.OutOfAmmoSound();
        }
    }
    */


    public void PlayerDoFireBullets()
    {
        if (playerScript.GetPlayerBulletCount() != 0)
        {
            gunScript.Shoot(enemyUIScript.GetEnemyPosition());
            // Enemy only take damage if they are visible on the visualizer screen
            bulletCount--;
            playerScript.UpdateBulletCount(bulletCount);
            gunScript.FiringButtonCooldown();
        }
        // Case for when there are no more bullets.
        else
        {
            gunScript.ShowOutOfAmmoText();
            gunScript.OutOfAmmoSound();
        }
    }

    // Reload Logic: Only allows player to reload once the bullet count hits 0. If player tries to reload
    // before the bullet count hits 0, they will be shown an error text and hear a click sound.
    public void PlayerDoReload()
    {
        if (playerScript.GetPlayerBulletCount() == 0)
        {
            StartCoroutine(ReloadProcess());
            gunScript.ReloadButtonCooldown();
        }
        else
        {
            gunScript.OutOfAmmoSound();
            gunScript.ShowReloadErrorText();
        }
    }

    // LOGIC FOR AI actions: Shoot a projectile at the enemy.
    // If enemy cannot be seen, simply launch the projectile forward
    // Else, launch the projectile in the direction of the enemy and hit the enemy in 1.5s
    // Upon impact, use the function On?????Impact to display the explosion effect upon collision with the target.


    // ORIGINAL
    /*
    public void PlayerDoBasketball()
    {
        basketballScript.ShootBasketball(enemyUIScript.GetEnemyPosition());
        // Enemy only takes damage if they are visible on the visualizer screen
        if (enemyUIScript.isEnemyVisible)
        {
            // Should display the blood prefab as well as explosion
            StartCoroutine(OnBasketballImpact());
        }
        basketballScript.ButtonCooldown();
    }

    public void PlayerDoSoccer()
    {
        soccerballScript.KickSoccerball(enemyUIScript.GetEnemyPosition());
        if (enemyUIScript.isEnemyVisible)
        {
            StartCoroutine(OnSoccerballImpact());
        }
        soccerballScript.ButtonCooldown();
    }

    public void PlayerDoVolleyball()
    {
        volleyballScript.HitVolleyball(enemyUIScript.GetEnemyPosition());
        if (enemyUIScript.isEnemyVisible)
        {
            StartCoroutine(OnVolleyballImpact());
        }
        volleyballScript.ButtonCooldown();
    }

    public void PlayerDoBowling()
    {
        bowlingScript.RollBowlingBall(enemyUIScript.GetEnemyPosition());
        if (enemyUIScript.isEnemyVisible)
        {
            StartCoroutine(OnBowlingBallImpact());
        }
        bowlingScript.ButtonCooldown();
    }
    */

    // TEST
    public void PlayerDoBasketball()
    {
        basketballScript.ShootBasketball(enemyUIScript.GetEnemyPosition());
        // Enemy only takes damage if they are visible on the visualizer screen
        basketballScript.ButtonCooldown();
    }

    public void PlayerDoSoccer()
    {
        soccerballScript.KickSoccerball(enemyUIScript.GetEnemyPosition());
        soccerballScript.ButtonCooldown();
    }

    public void PlayerDoVolleyball()
    {
        volleyballScript.HitVolleyball(enemyUIScript.GetEnemyPosition());
        volleyballScript.ButtonCooldown();
    }

    public void PlayerDoBowling()
    {
        bowlingScript.RollBowlingBall(enemyUIScript.GetEnemyPosition());
        bowlingScript.ButtonCooldown();
    }

    // Rain Bomb Logic: launches a rain bomb. if enemy is not visible, simply launches it forward.
    // if enemy is visible, it launches the bomb at them, Rain Bomb count decreases by 1
    // Separate function attached to rainbomb prefab: RainBombCollisionScript. if collision detected by rainbomb, it will result in the rain effect. Using OnCollisionEnter.
    // Separate function attached to rain prefab: RainEffectDamageScript. each time enemy enters the boundary of the rainbomb, they will take 5 damage. Using OnTriggerEnter.
    // ORIGINAL
    /*
    public void PlayerDoRainBomb()
    {
        if (rainBombCount != 0)
        {
            rainBombScript.LaunchRainBomb(enemyUIScript.GetEnemyPosition());
            if (enemyUIScript.isEnemyVisible)
            {
                StartCoroutine(OnRainBombImpact());
            }
            // Upon throwing the rainbomb, update the rain bomb count.
            rainBombCount--;
            // EDITED HERE
            // rainBombScript.UpdateRainBombCount(rainBombCount);
            playerScript.UpdateRainBombCount(rainBombCount);
            rainBombScript.ButtonCooldown();
        }
        else
        {
            rainBombScript.ShowOutOfRainBombsText();
            rainBombScript.PlayOutOfRainBombSound();
        }
    }
    */

    // TESTING
    public void PlayerDoRainBomb()
    {
        if (rainBombCount != 0)
        {
            rainBombScript.LaunchRainBomb(enemyUIScript.GetEnemyPosition());
            // Upon throwing the rainbomb, update the rain bomb count.
            rainBombCount--;
            // EDITED HERE
            playerScript.UpdateRainBombCount(rainBombCount);
            rainBombScript.ButtonCooldown();
        }
        else
        {
            rainBombScript.ShowOutOfRainBombsText();
            rainBombScript.PlayOutOfRainBombSound();
        }
    }

    // SHIELD LOGIC: Player is not able to activate shield if both shield hp and shield count are 0. 
    // 
    public void PlayerDoShield()
    {
        if (playerScript.GetPlayerShieldHp() == 0 && playerScript.GetPlayerShieldCount() == 0)
        {
            return;
        }
        else
        {
            // changed here
            if (shieldScript.UseShield(enemyManagerScript.playerShieldHp, enemyManagerScript.playerShieldCount))
            {
                enemyManagerScript.playerShieldHp = maxShieldHp;
                enemyManagerScript.playerShieldCount--;
                playerScript.SetShieldHp(enemyManagerScript.playerShieldHp);
                playerScript.SetPlayerShields(enemyManagerScript.playerShieldCount);
            }
            // shieldScript.UseShield();
            shieldScript.ButtonCooldown();
        }
    }

    public void OnFireButtonPressed()
    {
        PlayerDoFireBullets();
    }

    public void OnReloadButtonPressed()
    {
        PlayerDoReload();
    }

    public void OnBasketballButtonPressed()
    {
        PlayerDoBasketball();
    }

    public void OnSoccerballButtonPressed()
    {
        PlayerDoSoccer();
    }

    public void OnVolleyballButtonPressed()
    {
        PlayerDoVolleyball();
    }

    public void OnBowlingButtonPressed()
    {
        PlayerDoBowling();
    }

    public void OnRainBombButtonPressed()
    {
        PlayerDoRainBomb();
    }

    public void OnShieldButtonPressed()
    {
        PlayerDoShield();
    }

    // Function for dealing rain damage.
    public void OnEnteringRain()
    {
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            enemyShieldHp -= genericActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            bloodScript.BloodSplatterEffect();
            enemyHp -= genericActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    // New functions for dealing damage by AI action.
    // To be called in EnemyCollisionScript
    // TEST
    public void OnAIActionImpactWithoutShield()
    {
        bloodScript.BloodSplatterEffect();
        enemyHp -= specialActionDamage;
        enemyUIScript.SetEnemyHp(enemyHp);
    }

    // To be called in EnemyShieldCollisionScript
    public void OnAIActionImpactWithShield()
    {
        enemyShieldHp -= specialActionDamage;
        enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
    }

    // To be called for Bullet and damage
    public void OnBulletImpactWithoutShield()
    {
        bloodScript.BloodSplatterEffect();
        enemyHp -= genericActionDamage;
        enemyUIScript.SetEnemyHp(enemyHp);
    }

    public void OnBulletImpactWithShield()
    {
        enemyShieldHp -= genericActionDamage;
        enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
    }

    // Only allows the player to fire bullets after the reload process has been completed. 
    private IEnumerator ReloadProcess()
    {
        gunScript.GunReload();
        yield return gunScript.ReloadDuration();
        // wait for the reload duration then update the bullet count to max bullets
        bulletCount = maxBullets;
    }

    /*

    // FUNCTIONS FOR DIFFERENT IMPACTS.
    public IEnumerator OnBasketballImpact()
    {
        yield return new WaitForSeconds(timeToBallImpact);

        // handles the case where enemy shield is active vs not active at the point of impact.
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            // maybe do getenemyshieldposition()
            basketballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            enemyShieldHp -= specialActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            basketballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            bloodScript.BloodSplatterEffect();
            enemyHp -= specialActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    public IEnumerator OnVolleyballImpact()
    {
        yield return new WaitForSeconds(timeToBallImpact);

        // handles the case where enemy shield is active vs not active at the point of impact.
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            volleyballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            enemyShieldHp -= specialActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            volleyballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            bloodScript.BloodSplatterEffect();
            enemyHp -= specialActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    public IEnumerator OnSoccerballImpact()
    {
        yield return new WaitForSeconds(timeToBallImpact);

        // handles the case where enemy shield is active vs not active at the point of impact.
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            soccerballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            enemyShieldHp -= specialActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            soccerballScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            bloodScript.BloodSplatterEffect();
            enemyHp -= specialActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    public IEnumerator OnBowlingBallImpact()
    {
        yield return new WaitForSeconds(timeToBallImpact);

        // handles the case where enemy shield is active vs not active at the point of impact.
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            bowlingScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            enemyShieldHp -= specialActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            bowlingScript.DisplayExplosion(enemyUIScript.GetEnemyPosition());
            bloodScript.BloodSplatterEffect();
            enemyHp -= specialActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    public IEnumerator OnBulletImpact()
    {
        yield return new WaitForSeconds(timeToProjectileImpact);

        // handles the case where enemy shield is active vs not active at the point of impact.
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            enemyShieldHp -= genericActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            bloodScript.BloodSplatterEffect();
            enemyHp -= genericActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }

    public IEnumerator OnRainBombImpact()
    {
        // instantiate the rain bomb effect here
        yield return new WaitForSeconds(timeToBombImpact);
        
        if (enemyManagerScript.IsEnemyShieldActive())
        {
            // Testing
            // rainBombScript.DisplayRainEffect(enemyUIScript.GetEnemyPosition());
            enemyShieldHp -= genericActionDamage;
            enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
        }
        else
        {
            // Testing
            // rainBombScript.DisplayRainEffect(enemyUIScript.GetEnemyPosition());
            bloodScript.BloodSplatterEffect();
            enemyHp -= genericActionDamage;
            enemyUIScript.SetEnemyHp(enemyHp);
        }
    }
    */
}