using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    public EnemyUIScript enemyUIScript;
    public GameObject rainPrefab;
    public MQTTScript mqttScript;
    // Start is called before the first frame update
    void Start()
    {
        enemyUIScript = GameObject.FindWithTag("EnemyUIScript").GetComponent<EnemyUIScript>();
        mqttScript = GameObject.FindWithTag("MQTTScript").GetComponent<MQTTScript>();   // Testing
        // rainPrefab = GameObject.FindWithTag("RainPrefab");
    }
    void Update()
    {
        // Check enemy visibility
        bool isEnemyVisible = enemyUIScript.GetEnemyVisibility();

        // Iterate through all children and toggle active state if tagged as "RainPrefab"
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RainPrefab"))
            {
                child.gameObject.SetActive(isEnemyVisible);
            }
        }

        // if the enemy is not seen, the current in rain count will be updated accordingly.
        if (!isEnemyVisible)
        {
            mqttScript.currentInRainCount = 0;
            // Debug.Log("Enemy cannot be seen so they are no longer in rain.");
            // this.gameObject.SetActive(false);
        }
    }
}
