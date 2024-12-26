using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RainEffectDamageScript : MonoBehaviour
{
    public GameManager gameManager;
    public MQTTScript mqttScript;
    public EnemyUIScript enemyUIScript;
    public AudioSource rain;
    public AnchorBehaviour anchor;
    public float damageCooldown = 1.5f; // Cooldown time in seconds
    // private bool enemyDamaged = false;
    // private int rainDamageCount = 0;
    public int inRainCount = 0; 

    // Start is called before the first frame update
    void Start()
    {
        // testing
        // Ensure the rain area has a trigger collider
        // edited.
        // gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        // rain = GameObject.FindWithTag("RainSound").GetComponent<AudioSource>();
        /*
        Collider rainAreaCollider = GetComponent<Collider>();

        if (rainAreaCollider != null)
        {
            rainAreaCollider.isTrigger = true;  // Make sure the rain effect collider is a trigger
            Debug.Log("Collider present!");
        }
        else
        {
            Debug.LogError("No Collider found on rain effect object!");
        }
        */
        enemyUIScript = GameObject.FindWithTag("EnemyUIScript").GetComponent<EnemyUIScript>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        mqttScript = GameObject.FindWithTag("MQTTScript").GetComponent<MQTTScript>();   // Testing
        GameObject rainSoundObject = GameObject.FindWithTag("RainSound");
        if (rainSoundObject != null)
        {
            rain = rainSoundObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("No GameObject found with tag 'RainSound'!");
        }
    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Enemy") || other.CompareTag("EnemyShield"))
        //{
        //    // inRainCount += 1;
        //    mqttScript.currentInRainCount += 1;
        //    // Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + inRainCount);
        //    Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + mqttScript.currentInRainCount);
        //}
        if (other.CompareTag("Enemy"))
        {
            // inRainCount += 1;
            mqttScript.currentInRainCount += 1;
            // Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + inRainCount);
            Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + mqttScript.currentInRainCount);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Debug.Log("On trigger exit triggered");
        //if (other.CompareTag("Enemy") || other.CompareTag("EnemyShield"))
        //{
        //    // inRainCount -= 1;
        //    mqttScript.currentInRainCount -= 1;
        //    // Debug.Log("Enemy exited the rain. Current in_rain count for this rain: " + inRainCount);
        //    Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + mqttScript.currentInRainCount);
        //}
        if (other.CompareTag("Enemy"))
        {
            // inRainCount -= 1;
            mqttScript.currentInRainCount -= 1;
            // Debug.Log("Enemy exited the rain. Current in_rain count for this rain: " + inRainCount);
            Debug.Log("Enemy entered the rain. Current in_rain count for this rain: " + mqttScript.currentInRainCount);
        }
    }
}
