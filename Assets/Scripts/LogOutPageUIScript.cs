using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogOutPageUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text playerOneScore;
    public TMP_Text playerTwoScore;
    public TMP_Text playerText;
    public TMP_Text gameWinnerText;
    public TMP_Text drawText;
    public AudioSource gameOverSound;

    void Start()
    {
        string playerOneSavedScore = LogOutScript.playerOneScore;
        string playerTwoSavedScore = LogOutScript.playerTwoScore;
        playerOneScore.text = playerOneSavedScore;
        playerTwoScore.text = playerTwoSavedScore;
        int playerOneScoreNumeric = int.TryParse(playerOneSavedScore, out var result) ? result : 0;
        int playerTwoScoreNumeric = int.TryParse(playerTwoSavedScore, out var result2) ? result2 : 0;

        if (playerOneScoreNumeric > playerTwoScoreNumeric)
        {
            playerText.text = "Player";
            gameWinnerText.text = "1";
            drawText.gameObject.SetActive(false);
        }
        else if (playerOneScoreNumeric < playerTwoScoreNumeric) 
        {
            playerText.text = "Player";
            gameWinnerText.text = "2"; 
            drawText.gameObject.SetActive(false);
        }
        else
        {
            playerText.gameObject.SetActive(false);
            gameWinnerText.gameObject.SetActive(false);
            drawText.gameObject.SetActive(true);
        }
        gameOverSound.Play();
    }
}
