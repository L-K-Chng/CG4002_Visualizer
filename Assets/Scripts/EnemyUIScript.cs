using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Vuforia;

public class EnemyUIScript : MonoBehaviour
{
    // Health and Shield bars
    public Slider hpSlider;
    public Slider shieldHpSlider;
    public GameObject enemyHpBar;
    public GameObject enemyShieldBar;
    public GameObject enemySpottedText;
    public Gradient gradient;
    public UnityEngine.UI.Image fill;
    public MQTTScript mqttScript;

    // In Game Stats
    public int maxHp = 100;
    public int maxShieldHp = 30;
    public int maxBullets = 6;
    public int maxShields = 3;
    public int maxRainBombs = 2;
    public int initialScore = 0;
    public int enemyHp;
    public int enemyShieldHp;
    public int enemyBulletCount;
    public int enemyShieldCount;
    public int enemyRainBombCount;

    // In Game Text
    public TMP_Text enemyShieldCountText;
    public TMP_Text enemyRainBombText;
    public TMP_Text enemyBulletsText;
    public TMP_Text enemyScore;
    // New additions
    public TMP_Text enemyHealthPointsText;
    public TMP_Text enemyShieldPointsText;

    public bool isEnemyVisible = false;
    // ObserverBehaviour detects whether enemies are in the screen.
    public ObserverBehaviour observerBehaviour;

    // Variables to determine whether to send the visibility message to mqtt
    private float visibilityChangeTime = 0f;
    private const float visibilityDelayThreshold = 5f;
    private bool shouldPublish = false;
    private bool? lastPublishedVisibility = null;

    void Start()
    {
        enemySpottedText.SetActive(false);
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    // original code
    //void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    //{
    //    // Store the previous visibility state
    //    bool previousVisibility = isEnemyVisible;

    //    // Update visibility based on the target status
    //    if (targetStatus.Status == Status.TRACKED)
    //    {
    //        // new text
    //        enemySpottedText.SetActive(true);
    //        isEnemyVisible = true;
    //    }
    //    else
    //    {
    //        enemySpottedText.SetActive(false);
    //        isEnemyVisible = false;
    //    }

    //    // Only publish the message if the visibility status has changed
    //    if (previousVisibility != isEnemyVisible)
    //    {
    //        //var message = new
    //        //{
    //        //    player_id = 1,
    //        //    can_see = isEnemyVisible
    //        //};

    //        //string jsonMessage = JsonConvert.SerializeObject(message);

    //        //// Publish the visibility status to the MQTT broker
    //        //mqttScript.PublishEnemyVisibility(jsonMessage);

    //        // testing new method
    //        mqttScript.SendVisibilityMessage(LoginStateScript.stored_player_id, isEnemyVisible);
    //    }
    //}

    // NEW CODE TEST
    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        bool previousVisibility = isEnemyVisible;

        // Update visibility based on the target status
        isEnemyVisible = targetStatus.Status == Status.TRACKED;
        enemySpottedText.SetActive(isEnemyVisible);

        // Check if visibility status has changed
        if (previousVisibility != isEnemyVisible)
        {
            Debug.Log("Visibility status changed! Resetting timer.");
            // Reset the timer and set flag for publishing
            visibilityChangeTime = Time.time;
            shouldPublish = true;
        }
    }

    // NEW METHOD
    private void Update()
    {
        // Only proceed if a visibility change happened and publishing is needed
        if (shouldPublish && (Time.time - visibilityChangeTime >= visibilityDelayThreshold))
        {
            // Only publish if the current visibility status is different from the last published status
            if (lastPublishedVisibility != isEnemyVisible)
            {
                Debug.Log("Publishing message after 5-second delay.");

                // Publish the message
                mqttScript.SendVisibilityMessage(LoginStateScript.stored_player_id, isEnemyVisible);

                // Update lastPublishedVisibility to current state
                lastPublishedVisibility = isEnemyVisible;

                // Reset shouldPublish to prevent repeated publishing
                shouldPublish = false;
            }
        }
    }

    // check whether enemy visible
    public bool GetEnemyVisibility()
    {
        return isEnemyVisible;
    }

    // to deduce the coordinates of the enemy if they are seen on the screen.
    public Vector3 GetEnemyPosition()
    {
        if (isEnemyVisible && observerBehaviour != null)
        {
            return observerBehaviour.transform.position;
        }
        return Vector3.zero; // Return a default position if not visible
    }

    // For updating the enemy score UI
    public void UpdateEnemyScore(int score)
    {
        enemyScore.text = score.ToString();
    }

    // 1. Sets the enemy's HP
    public void SetEnemyHp(int hp)
    {
        // ensures hp dont go below 0.
        hp = Mathf.Max(0, hp);
        hpSlider.value = hp;
        fill.color = gradient.Evaluate(hpSlider.normalizedValue);
        enemyHealthPointsText.text = hp.ToString();
    }

    public void SetEnemyMaxHp()
    {
        hpSlider.value = maxHp;
        // 1f defines the max value, 0f is the min value
        fill.color = gradient.Evaluate(1f);
        enemyHealthPointsText.text = maxHp.ToString();
    }

    // 2. Sets the enemy shield's hp
    public void SetEnemyShieldHp(int hp)
    {
        // ensures hp dont go below 0.
        hp = Mathf.Max(0, hp);
        shieldHpSlider.value = hp;
        enemyShieldPointsText.text = hp.ToString();
    }

    public void SetEnemyMaxShieldHp()
    {
        shieldHpSlider.value = maxShieldHp;
        enemyShieldPointsText.text = maxShieldHp.ToString();
    }

    // 3. Sets the enemy's shield count
    public void SetEnemyShields(int nShields)
    {
        enemyShieldCountText.text = nShields.ToString();
    }

    public void SetEnemyMaxShields()
    {
        enemyShieldCountText.text = maxShields.ToString();
    }

    // 4. Sets the enemy's bullets
    public void UpdateBulletCount(int ammoCount)
    {
        enemyBulletCount = ammoCount;
        enemyBulletsText.text = enemyBulletCount.ToString();
    }

    public void SetEnemyMaxBullets()
    {
        enemyBulletsText.text = maxBullets.ToString();
    }

    // 5. Sets the enemy's rain bombs
    public void SetEnemyMaxRainBombs()
    {
        enemyRainBombText.text = maxRainBombs.ToString();
    }

    public void UpdateRainBombCount(int newRainBombCount)
    {
        enemyRainBombCount = newRainBombCount;
        enemyRainBombText.text = newRainBombCount.ToString();
    }

    // Functions to get enemy stats
    public float GetEnemyHp()
    {
        return hpSlider.value;
    }

    public float GetEnemyShieldHp()
    {
        return shieldHpSlider.value;
    }

    public int GetEnemyShieldCount()
    {
        int shieldCount;
        int.TryParse(enemyShieldCountText.text, out shieldCount);
        return shieldCount;
    }

    public int GetEnemyBulletCount()
    {
        int bulletCount;
        int.TryParse(enemyBulletsText.text, out bulletCount);
        return bulletCount;
    }

    public int GetEnemyRainBombCount()
    {
        int rainBombCount;
        int.TryParse(enemyRainBombText.text, out rainBombCount);
        return rainBombCount;
    }

    public int GetEnemyScore()
    {
        int score;
        int.TryParse(enemyScore.text, out score);
        return score;
    }

    // To refresh enemy hp once it drops to 0.
    public bool CheckEnemyRespawn()
    {
        if (GetEnemyHp() == 0)
        {
            // set back to full hp
            // SetEnemyMaxHp();
            return true;
        }
        else
        {
            return false;
        }
    }

    // To refresh enemy shield if shield hp is 0 but shield count is more than 0.
    public bool CheckEnemyShieldRefresh()
    {
        if (GetEnemyShieldHp() == 0 && GetEnemyShieldCount() != 0)
        {
            // SetEnemyMaxShieldHp();
            return true;
        }
        else
        {
            return false;
        }
    }
}
