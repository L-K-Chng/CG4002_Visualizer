using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffectDamageScript : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource rain;

    // Start is called before the first frame update
    void Start()
    {
        // testing
        // Ensure the rain area has a trigger collider
        // edited.
        // gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        // rain = GameObject.FindWithTag("RainSound").GetComponent<AudioSource>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Make enemy take damage on entering rain again.
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy entered rain area!");

            gameManager.OnEnteringRain();
            rain.Play();
        }
    }
    

    /*
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy entered rain area!");

            gameManager.OnEnteringRain();
            rain.Play();
        }
    }
    */
}
