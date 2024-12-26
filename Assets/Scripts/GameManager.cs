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
    public EnemyActionsScript enemyActionsScript;
    public LogOutScript logOutScript;

    // Buttons
    public Button fireButton;
    public Button reloadButton;
    public Button basketballButton;
    public Button soccerballButton;
    public Button volleyballButton;
    public Button bowlingButton;
    public Button rainBombButton;
    public Button shieldButton;
    
    // Initialise the health, bullets, shields and rain bombs
    public int maxHp = 100;
    public int maxShieldHp = 30;
    public int maxBullets = 6;
    public int maxShields = 3;
    public int maxRainBombs = 2;
    public int initialScore = 0;
    
    // REQUIRED VARIABLES FOR PLAYER'S ACTIONS
    // for enemy
    public int enemyHp;
    public int enemyShieldHp;
    public int enemyShieldCount;
    // for player
    public int playerBulletCount;
    public int playerRainBombCount;
    public int playerScore;

    // Damage
    //private int genericActionDamage = 5;
    //private int specialActionDamage = 10;
    // new boolean to control the damage taken inside rain.
    //public bool playerActionFlag = false;

    /*
     * Variables to control enemy actions 
     */
    // Enemy buttons
    public Button enemyFireButton;
    public Button enemyReloadButton;
    public Button enemyRainBombButton;
    public Button enemyShieldButton;

    // New buttons
    public Button enemyBasketballButton;
    public Button enemyBowlingButton;
    public Button enemySoccerButton;
    public Button enemyVolleyballButton;

    // List of enemy buttons
    // public List<Button> enemySpecialActionButtons;
    public GameObject enemyShieldPrefab;
    // Effects for when a player gets hit. Display blood and a corresponding grunt sound
    public AudioSource playerHurtSound;
    public GameObject playerBloodPrefab;

    // In game values for enemy attacks onto player.
    // For Player
    public int playerHp;
    public int playerShieldHp;
    public int playerShieldCount;
    // For Enemy
    public int enemyBulletCount;
    public int enemyRainBombCount;
    public int enemyScore;

    // Start is called before the first frame update
    void Start()
    {
        // FOR CONTROLLING PLAYER ACTIONS ON ENEMY
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
        enemyUIScript.SetEnemyMaxHp();
        enemyUIScript.SetEnemyShieldHp(0);
        enemyUIScript.SetEnemyMaxShields();
        playerScript.SetMaxBullets();
        playerScript.SetMaxRainBomb();
        playerScript.UpdatePlayerScore(0);


        enemyShieldButton.onClick.AddListener(OnEnemyShieldButtonPressed);
        enemyReloadButton.onClick.AddListener(OnEnemyReloadButtonPressed);
        enemyFireButton.onClick.AddListener(OnEnemyFireButtonPressed);
        enemyRainBombButton.onClick.AddListener(OnEnemyRainBombButtonPressed);
        enemyBasketballButton.onClick.AddListener(OnEnemyBasketballButtonPressed);
        enemyBowlingButton.onClick.AddListener(OnEnemyBowlingButtonPressed);
        enemySoccerButton.onClick.AddListener(OnEnemySoccerButtonPressed);
        enemyVolleyballButton.onClick.AddListener(OnEnemyVolleyballButtonPressed);
        
        // Set hp and ammo.
        playerScript.SetMaxHp();
        playerScript.SetShieldHp(0);
        playerScript.SetPlayerMaxShields();
        enemyUIScript.SetEnemyMaxBullets();
        enemyUIScript.SetEnemyMaxRainBombs();
        enemyUIScript.UpdateEnemyScore(0);

        // set these items to false first.
        playerBloodPrefab.SetActive(false);
        shieldScript.shieldPrefab.SetActive(false);
        shieldScript.shieldActivatedText.SetActive(false);
        shieldScript.shieldBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 25 OCTOBER UPDATE: Game engine will be responsible for updating this. we do not need to manually update our values by ourselves.
    }

    // Logic for firing action: Fires the projectile. If enemy cannot be spotted, simply shoot forward
    // Else, shoot towards the enemy, hitting them in 1.5s

    // For each of these actions, we need to add cases based on whether they are player 1 or player 2. DO IT ON 25 OCT

    public void PlayerDoFireBullets(int player_id_stored)
    {
        if (player_id_stored == 1)
        {
            // check for player 1 ammo
            if (playerScript.GetPlayerBulletCount() != 0)
            {
                Debug.Log("Player 1 fired a bullet!");
                gunScript.Shoot(enemyUIScript.GetEnemyPosition());
                // Enemy only take damage if they are visible on the visualizer screen
                gunScript.FiringButtonCooldown();
            }
            // Case for when there are no more bullets.
            else
            {
                gunScript.ShowOutOfAmmoText();
                gunScript.OutOfAmmoSound();
            }
        }
        else
        {
            // Check for player 2 ammo
            if (enemyUIScript.GetEnemyBulletCount() != 0)
            {
                Debug.Log("Player 2 fired a bullet!");
                gunScript.Shoot(enemyUIScript.GetEnemyPosition());
                // Enemy only take damage if they are visible on the visualizer screen
                gunScript.FiringButtonCooldown();
            }
            else
            {
                gunScript.ShowOutOfAmmoText();
                gunScript.OutOfAmmoSound();
            }
        }
    }

    // Reload Logic: Only allows player to reload once the bullet count hits 0. If player tries to reload
    // before the bullet count hits 0, they will be shown an error text and hear a click sound.
    public void PlayerDoReload(int player_id_stored)
    {
        if (player_id_stored == 1)
        {
            // playerScript.GetPlayerBulletCount() refers to player 1's bullet count
            if (playerScript.GetPlayerBulletCount() == 0)
            {
                Debug.Log("Player 1 now reloading!");
                StartCoroutine(ReloadProcess());
                gunScript.ShowReloadText();
                gunScript.ReloadButtonCooldown();
            }
            else
            {
                Debug.Log("Bullets not at 0! Player 1 cannot reload!");
                gunScript.OutOfAmmoSound();
                gunScript.ShowReloadErrorText();
            }
        }
        else
        {
            // enemyUIScript.GetEnemyBulletCount() refers to player 2's bullet count
            if (enemyUIScript.GetEnemyBulletCount() == 0)
            {
                Debug.Log("Player 2 now reloading!");
                StartCoroutine(ReloadProcess());
                gunScript.ShowReloadText();
                gunScript.ReloadButtonCooldown();
            }
            else
            {
                Debug.Log("Bullets not at 0! Player 2 cannot reload!");
                gunScript.OutOfAmmoSound();
                gunScript.ShowReloadErrorText();
            }
        }
    }

    // LOGIC FOR AI actions: Shoot a projectile at the enemy.
    // If enemy cannot be seen, simply launch the projectile forward
    // Else, launch the projectile in the direction of the enemy and hit the enemy in 1.5s
    // Upon impact, use the function On?????Impact to display the explosion effect upon collision with the target.
    public void PlayerDoBasketball()
    {
        // playerActionFlag = true;
        basketballScript.ShootBasketball(enemyUIScript.GetEnemyPosition());
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
    public void PlayerDoRainBomb(int player_id_stored)
    {
        if (player_id_stored == 1)
        {
            // playerScript rain bomb count refers to player 1's rain bomb count.
            if (playerScript.GetPlayerRainBombCount() != 0)
            {
                Debug.Log("Player 1 throwing rain bomb!");
                rainBombScript.LaunchRainBomb(enemyUIScript.GetEnemyPosition());
                rainBombScript.ButtonCooldown();
            }
            else
            {
                Debug.Log("Player 1 out of rain bombs!");
                rainBombScript.ShowOutOfRainBombsText();
                rainBombScript.PlayOutOfRainBombSound();
            }
        }
        else
        {
            // enemyUIScript's rain bomb count refers to player 2's rain bomb count
            if (enemyUIScript.GetEnemyRainBombCount() != 0)
            {
                Debug.Log("Player 2 throwing rain bomb!");
                rainBombScript.LaunchRainBomb(enemyUIScript.GetEnemyPosition());
                rainBombScript.ButtonCooldown();
            }
            else
            {
                Debug.Log("Player 2 out of rain bombs!");
                rainBombScript.ShowOutOfRainBombsText();
                rainBombScript.PlayOutOfRainBombSound();
            }
        }
    }

    public void CheckShieldActive(int player_id_stored)
    {
        // do checks for both player 1 and player 2's shield
        if (player_id_stored == 1)
        {
            if (playerScript.GetPlayerShieldHp() != 0)
            {
                Debug.Log("Player 1 shield has been activated!");
                shieldScript.UseShield2();
            }
            else
            {
                Debug.Log("Player 1 shield has been fully depleted!");
                shieldScript.DeactivateShield();
            }

            if (enemyUIScript.GetEnemyShieldHp() != 0)
            {
                Debug.Log("Enemy (Player 2) shield is active");
                enemyActionsScript.enemyUseShield();
            }
            else
            {
                Debug.Log("Enemy (Player 2) shield is inactive");
                enemyActionsScript.enemyShieldDeactivated();
            }
        }
        else
        {
            if (enemyUIScript.GetEnemyShieldHp() != 0)
            {
                Debug.Log("Player 2 shield has been activated!");
                shieldScript.UseShield2();
            }
            else
            {
                Debug.Log("Player 2 shield has been fully depleted!");
                shieldScript.DeactivateShield();
            }

            if (playerScript.GetPlayerShieldHp() != 0 )
            {
                Debug.Log("Enemy (Player 1) shield is active");
                enemyActionsScript.enemyUseShield();
            }
            else
            {
                Debug.Log("Enemy (Player 2) shield is inactive");
                enemyActionsScript.enemyShieldDeactivated();
            }
        }
    }

    public void PlayerDoShield(int player_id_stored)
    {
        /*
         * Case 1: Shield hp == 0 and nshields != 0 -> Shield activates
         * Case 2: Shield hp == 0 and nshields == 0 -> no more shields so wont activate
         */
        if (player_id_stored == 1)
        {
            if (playerScript.GetPlayerShieldHp() == 0)
            {
                if (playerScript.GetPlayerShieldCount() != 0)
                {
                    shieldScript.UseShield2();
                }
                else
                {
                    Debug.Log("Player 1 has ran out of shields and cannot use it!");
                    // add an OutOfShields Error text here!
                    StartCoroutine(playerScript.DisplayMessage(playerScript.outOfShieldsText));
                }
                return;
            }
            //if (playerScript.GetPlayerShieldHp() == 0 && playerScript.GetPlayerShieldCount() == 0)
            //{
            //    Debug.Log("Player 1 has ran out of shields and cannot use it!");
            //    // add an OutOfShields Error text here!
            //    StartCoroutine(playerScript.DisplayMessage(playerScript.outOfShieldsText));
            //    return;
            //}
        }
        else
        {
            if (enemyUIScript.GetEnemyShieldHp() == 0)
            {
                if (enemyUIScript.GetEnemyShieldCount() != 0)
                {
                    shieldScript.UseShield2();
                }
                else
                {
                    Debug.Log("Player 2 has ran out of shields and cannot use it!");
                    // add an OutOfShields Error text here!
                    StartCoroutine(playerScript.DisplayMessage(playerScript.outOfShieldsText));
                }
                return;
            }
            //if (enemyUIScript.GetEnemyShieldHp() == 0 && enemyUIScript.GetEnemyShieldCount() == 0)
            //{
            //    Debug.Log("Player 2 has ran out of shields and cannot use it!");
            //    // add an OutOfShields Error text here!
            //    StartCoroutine(playerScript.DisplayMessage(playerScript.outOfShieldsText));
            //    return;
            //}
        }
    }

    public void PlayerDoLogOut()
    {
        logOutScript.LoadLogOutPage();
    }

    public void OnFireButtonPressed()
    {
        PlayerDoFireBullets(LoginStateScript.stored_player_id);
    }

    public void OnReloadButtonPressed()
    {
        PlayerDoReload(LoginStateScript.stored_player_id);
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
        PlayerDoRainBomb(LoginStateScript.stored_player_id);
    }

    public void OnShieldButtonPressed()
    {
        PlayerDoShield(LoginStateScript.stored_player_id);
    }    

    // Only allows the player to fire bullets after the reload process has been completed. 
    private IEnumerator ReloadProcess()
    {
        gunScript.GunReload();
        yield return gunScript.ReloadDuration();
        // wait for the reload duration then update the bullet count to max bullets
    }

    /*
     * ALL ENEMY ACTIONS ARE CARRIED OUT FROM THIS POINT ONWARDS.
     * 
     */

    private IEnumerator EnemyShieldButtonCooldown()
    {
        enemyShieldButton.interactable = false;
        yield return new WaitForSeconds(3.2f);
        enemyShieldButton.interactable = true;
    }

    // shield button cooldown 3s
    public void enemyShieldEffect()
    {
        if (enemyUIScript.GetEnemyShieldCount() == 0 && enemyUIScript.GetEnemyShieldHp() == 0)
        {
            return;
        }
        else
        {
            if (enemyShieldHp == 0)
            {
                // enemy activates their shield if their shield hp is 0.
                enemyShieldPrefab.SetActive(true);
                enemyShieldCount--;
                enemyShieldHp = maxShieldHp;
                enemyUIScript.SetEnemyShieldHp(enemyShieldHp);
                enemyUIScript.SetEnemyShields(enemyShieldCount);
            }
        }
    }

    public void OnEnemyShieldButtonPressed()
    {
        StartCoroutine(EnemyShieldButtonCooldown());
        enemyShieldEffect();
    }

    // Controls the enemy buttons
    private IEnumerator ButtonCooldown(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(1.5f);
        button.interactable = true;
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

    // 25 Oct 2024: Added new methods to show hit by enemy on player
    public void PlayerReceiveDamage(IEnumerator displayedMessage)
    {
        playerHurtSound.Play();
        playerBloodSplatterEffect();
        StartCoroutine(displayedMessage);
    }

    public void PlayerReceiveDamageWithShield(IEnumerator displayedMessage)
    {
        playerHurtSound.Play();
        StartCoroutine(displayedMessage);
    }

    public void OnEnemyBasketballButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyBasketballButton));
        enemyActionsScript.EnemyShootBasketball(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
    }

    public void OnEnemyBowlingButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyBowlingButton));
        enemyActionsScript.EnemyRollBowlingBall(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
    }

    public void OnEnemySoccerButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemySoccerButton));
        enemyActionsScript.EnemyKickSoccerBall(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
    }

    public void OnEnemyVolleyballButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyVolleyballButton));
        enemyActionsScript.EnemyHitVolleyball(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
    }

    // call this in mqtt
    // 25 OCTOBER UPDATE:
    // COMMENTED OUT THE ENEMY BULLET AND RAIN BOMB COUNT VALUES.
    public void OnEnemyFireButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyFireButton));
        if (enemyUIScript.GetEnemyBulletCount() != 0)
        {
            enemyActionsScript.EnemyFireBullet(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
        }
    }

    public void OnEnemyFireButtonPressed(int player_id_stored)
    {
        StartCoroutine(ButtonCooldown(enemyFireButton));
        if (player_id_stored == 1)
        {
            // look at p2 bullet count. if not = 0 then fire at p1.
            if (enemyUIScript.GetEnemyBulletCount() != 0)
            {
                enemyActionsScript.EnemyFireBullet(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
            }
        }
        else
        {
            // look at p1 bullet count. if not = 0 then fire at p2.
            if (playerScript.GetPlayerBulletCount() != 0)
            {
                enemyActionsScript.EnemyFireBullet(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
            }
        }
    }

    public void OnEnemyRainBombButtonPressed(int player_id_stored)
    {
        StartCoroutine(ButtonCooldown(enemyRainBombButton));
        if (player_id_stored == 1)
        {
            if (enemyUIScript.GetEnemyRainBombCount() != 0)
            {
                enemyActionsScript.EnemyLaunchRainBomb(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
            }
        }
        // if player id is 2 then look at player 1 rain bomb count.
        else
        {
            if (playerScript.GetPlayerRainBombCount() != 0)
            {
                enemyActionsScript.EnemyLaunchRainBomb(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
            }
        }
    }

    public void OnEnemyRainBombButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyRainBombButton));
        if (enemyUIScript.GetEnemyRainBombCount() != 0)
        {
            enemyActionsScript.EnemyLaunchRainBomb(enemyUIScript.GetEnemyPosition(), playerScript.GetPlayerPosition());
            // Reduce rain bombs by 1 each time they are used.
            // Commented out these values, will not be used.
            //enemyRainBombCount--;
            //enemyUIScript.UpdateRainBombCount(enemyRainBombCount);
        }
    }

    // no need for this method.
    public void OnEnemyReloadButtonPressed()
    {
        StartCoroutine(ButtonCooldown(enemyReloadButton));
        if (enemyUIScript.GetEnemyBulletCount() == 0)
        {
            enemyBulletCount = maxBullets;
            enemyUIScript.SetEnemyMaxBullets();
        }
    }
}