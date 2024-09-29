using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you're using TextMeshPro UI

/*
 *   Controls the player UI.
*/
public class PlayerScript : MonoBehaviour
{
    // Health and Shield bars
    public Slider hpSlider;
    public Slider shieldHpSlider;
    public GameObject playerHpBar;
    public GameObject playerShieldBar;
    public Gradient gradient;
    public UnityEngine.UI.Image fill;

    // In Game Stats
    public int maxHp = 100;
    public int maxShieldHp = 30;
    public int maxBullets = 6;
    public int maxShields = 3;
    public int maxRainBombs = 2;
    public int initialScore = 0;
    public int playerHp;
    public int playerShieldHp;
    public int playerBulletCount;
    public int playerShieldCount;
    public int playerRainBombCount;

    // In Game Text
    public TMP_Text playerShieldCountText;
    public TMP_Text playerRainBombText;
    public TMP_Text playerBulletsText;
    public TMP_Text playerScore;
    // new additions
    public TMP_Text playerHealthPointsText;
    public TMP_Text playerShieldPointsText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // updates the player score UI
    public void UpdatePlayerScore(int score)
    {
        playerScore.text = score.ToString();
    }

    // 1. Sets the player's HP
    public void SetHp(int hp)
    {
        // ensures hp dont go below 0.
        hp = Mathf.Max(0, hp);
        hpSlider.value = hp;
        fill.color = gradient.Evaluate(hpSlider.normalizedValue);
        playerHealthPointsText.text = hp.ToString();
    }

    public void SetMaxHp()
    {
        hpSlider.value = maxHp;
        fill.color = gradient.Evaluate(1f);
        playerHealthPointsText.text = maxHp.ToString();
    }

    // 2. Sets the player's shield HP
    public void SetShieldHp(int hp)
    {
        // ensures hp dont go below 0.
        hp = Mathf.Max(0, hp);
        shieldHpSlider.value = hp;
        playerShieldPointsText.text = hp.ToString();
    }

    public void SetMaxShieldHp()
    {
        shieldHpSlider.value = maxShieldHp;
        playerShieldPointsText.text = maxShieldHp.ToString();
    }

    // 3. Sets the player's shield count
    public void SetPlayerShields(int nShields)
    {
        playerShieldCountText.text = nShields.ToString();
    }

    public void SetPlayerMaxShields()
    {
        playerShieldCountText.text = maxShields.ToString();
    }

    // 4. Sets the player's bullets
    public void UpdateBulletCount(int ammoCount)
    {
        playerBulletCount = ammoCount;
        playerBulletsText.text = playerBulletCount.ToString();
    }

    public void SetMaxBullets()
    {
        playerBulletsText.text = maxBullets.ToString();
    }

    // 5. Sets the player's rain bombs
    public void SetMaxRainBomb()
    {
        playerRainBombText.text = maxRainBombs.ToString();
    }

    public void UpdateRainBombCount(int newRainBombCount)
    {
        playerRainBombCount = newRainBombCount;
        playerRainBombText.text = newRainBombCount.ToString();
    }

    // Functions to get player stats
    public float GetPlayerHp()
    {
        return hpSlider.value;
    }

    public float GetPlayerShieldHp()
    {
        return shieldHpSlider.value;
    }

    public int GetPlayerShieldCount()
    {
        int shieldCount;
        int.TryParse(playerShieldCountText.text, out shieldCount);
        return shieldCount;
    }

    public int GetPlayerBulletCount()
    {
        int ammoCount;
        int.TryParse(playerBulletsText.text, out ammoCount);
        return ammoCount;
    }

    public int GetPlayerRainBombCount()
    {
        int rainBombCount;
        int.TryParse(playerRainBombText.text, out rainBombCount);
        return rainBombCount;
    }

    // Functions to check whether player respawns or if the shield refreshes
    public bool CheckPlayerRespawn()
    {
        if (GetPlayerHp() == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckPlayerShieldRefresh()
    {
        if (GetPlayerShieldHp() == 0 && GetPlayerShieldCount() != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}