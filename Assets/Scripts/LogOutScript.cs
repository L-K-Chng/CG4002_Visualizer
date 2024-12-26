using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOutScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public EnemyUIScript enemyUIScript;
    public static string playerOneScore;
    public static string playerTwoScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLogOutPage()
    {
        playerOneScore = playerScript.GetPlayerScore().ToString();
        playerTwoScore = enemyUIScript.GetEnemyScore().ToString();
        SceneManager.LoadScene("GameOverScene");
    }
}
