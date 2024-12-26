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

    // Player Collision Text
    // Show text for 3s upon collision.
    public GameObject hitByBasketballText;
    public GameObject hitByBowlingBallText;
    public GameObject hitByBulletText;
    public GameObject hitBySoccerBallText;
    public GameObject hitByRainBombText;
    public GameObject hitByVolleyballText;
    public GameObject outOfShieldsText;
    public GameObject killText;
    public GameObject deathText;
    public GameObject redoActionErrorMessage;
    public GameObject playerInRainText;
    public TMP_Text playerInRainCountText;

    public GameObject playerOneOutline;
    public GameObject playerTwoOutline;

    void Start()
    {
        hitByBasketballText.SetActive(false);
        hitByBowlingBallText.SetActive(false);
        hitByBulletText.SetActive(false);
        hitBySoccerBallText.SetActive(false);
        hitByRainBombText.SetActive(false);
        hitByVolleyballText.SetActive(false);
        outOfShieldsText.SetActive(false);
        killText.SetActive(false);
        deathText.SetActive(false);
        redoActionErrorMessage.SetActive(false);
        playerInRainText.SetActive(false);

        // To indicate to the player which player they are.
        if (LoginStateScript.stored_player_id == 1)
        {
            playerOneOutline.SetActive(true);
            playerTwoOutline.SetActive(false);
        }
        else
        {
            playerTwoOutline.SetActive(true);
            playerOneOutline.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public Vector3 GetPlayerPosition()
    {
        // Get the main AR camera's position
        Vector3 arCameraPosition = Camera.main.transform.position;
        Debug.Log("Player position is: " +  arCameraPosition);
        return arCameraPosition;
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

    // try get player score in new scene
    public int GetPlayerScore()
    {
        int score;
        int.TryParse(playerScore.text, out score);
        return score;
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

    // Display message for 4 seconds
    public  IEnumerator DisplayMessage(GameObject message)
    {
        // Enable the message object to show it
        message.SetActive(true);

        // Wait for 3 seconds
        yield return new WaitForSeconds(4f);

        // Disable the message object to hide it
        message.SetActive(false);
    }

    public void DisplayInRainMessage(int rainCount)
    {
        playerInRainText.SetActive(true);
        playerInRainCountText.text = rainCount.ToString();
    }

    public void DeactivateInRainMessage()
    {
        playerInRainText.SetActive(false);
    }
}