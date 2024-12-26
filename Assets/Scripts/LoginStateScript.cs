using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginStateScript : MonoBehaviour
{
    public static int stored_player_id;
    public GameObject arShooterText;
    public Button tapToPlayButton;
    public GameObject tapToPlayText;
    public GameObject chooseYourPlayerText;
    public Button playerOneButton;
    public Button playerTwoButton;
    public AudioSource startGameMusic;
    public GameObject playerOneHighlight;
    public GameObject playerTwoHighlight;
    public GameObject playerOneSelectedText;
    public GameObject playerTwoSelectedText;


    // Start is called before the first frame update
    void Start()
    {
        chooseYourPlayerText.SetActive(false);
        playerOneButton.gameObject.SetActive(false);
        playerTwoButton.gameObject.SetActive(false);

        StartCoroutine(BlinkText());
        startGameMusic.Play();

        tapToPlayButton.onClick.AddListener(OnPlayButtonPressed);
        playerOneButton.onClick.AddListener(OnPlayerOneButtonPressed);
        playerTwoButton.onClick.AddListener(OnPlayerTwoButtonPressed);

        playerOneHighlight.SetActive(false);
        playerTwoHighlight.SetActive(false);
        playerOneSelectedText.SetActive(false);
        playerTwoSelectedText.SetActive(false);
    }

    // for aesthetics. Make the ui blink.
    private IEnumerator BlinkText()
    {
        while (true)
        {
            // Toggle active state of tapToPlayText
            tapToPlayText.SetActive(!tapToPlayText.activeSelf);
            // Wait for half a second before toggling again
            yield return new WaitForSeconds(1.2f);
        }
    }

    public void OnPlayButtonPressed()
    {
        tapToPlayButton.gameObject.SetActive(false);
        arShooterText.SetActive(false);
        chooseYourPlayerText.SetActive(true);
        playerOneButton.gameObject.SetActive(true);
        playerTwoButton.gameObject.SetActive(true);
    }

    public void OnPlayerOneButtonPressed()
    {
        Debug.Log("Player 1 chosen! Stored player id has been set as 1!");
        stored_player_id = 1;
        StartCoroutine(LoadingScreenUI(playerOneHighlight, playerOneSelectedText));
        //SceneManager.LoadScene("SampleScene");
        //startGameMusic.Stop();
    }

    public void OnPlayerTwoButtonPressed()
    {
        Debug.Log("Player 2 chosen! Stored player id has been set as 2!");
        stored_player_id = 2;
        StartCoroutine(LoadingScreenUI(playerTwoHighlight, playerTwoSelectedText));
        //SceneManager.LoadScene("SampleScene");
        //startGameMusic.Stop();
    }

    // show the loading screen
    private IEnumerator LoadingScreenUI(GameObject highlight, GameObject text)
    {
        highlight.SetActive(true);
        text.SetActive(true);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("SampleScene");
        startGameMusic.Stop();
    }
}
