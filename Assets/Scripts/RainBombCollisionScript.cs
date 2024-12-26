using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainBombCollisionScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject rainPrefab;
    public float verticalOffset = 3f;  // to make the rainfall start higher than the point we hit the target.
    public AudioSource rainSound;

    // private float spawnOffsetZ = 3f;
    public RainBombManager rainBombManager;

    // For anchoring rain to a set point using the world position.
    // Start is called before the first frame update
    void Start()
    {
        rainBombManager = GameObject.FindWithTag("RainBombManager").GetComponent<RainBombManager>();
        // gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        // rainSound = GameObject.FindWithTag("RainSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ORIGINAL CODE
    // Anchors specifically at the area of collision.
    /*
    public void OnCollisionEnter(Collision collision)
    {
        // Get the position of the collision point
        Vector3 collisionPosition = collision.contacts[0].point;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Rainbomb hit enemy!");
            // Instantiate the rain effect at the collision position
            Vector3 rainEffectPosition = new Vector3(collisionPosition.x, collisionPosition.y + verticalOffset, collisionPosition.z);
            Debug.Log("Rain effect position: " + rainEffectPosition);

            // GameObject rainInstance = Instantiate(rainPrefab, rainEffectPosition, Quaternion.Euler(90, 0, 0));
            GameObject rainInstance = Instantiate(rainPrefab, rainEffectPosition, Quaternion.identity);

            // TESTING
            // Make sure the rain instance is not parented to anything
            rainInstance.transform.SetParent(null);  // This detaches it from any AR tracking object

            if (rainSound != null)
            {
                rainSound.Play();
            }
            // Destroy the rainbomb after instantiation of the rain effect
            Destroy(gameObject);  
            // call the function once rain bomb hits the target.
            gameManager.OnRainBombImpact();
        }
    }
    */
    

    // Testing
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("hit");
            // Get the point of collision 
            Vector3 collisionPoint = collision.contacts[0].point;

            GameObject rb = rainBombManager.DisplayRain(collisionPoint);

            // Set the rainbombs parent to be the enemy, and then set z upwards, and then reset the parent. 
            rb.transform.parent = (collision.gameObject.transform);

            Vector3 enemyPosition = collision.gameObject.transform.position;
            enemyPosition.z += spawnOffsetZ;
            rb.transform.position = enemyPosition;
            rb.transform.parent = null;

            if (rainSound != null)
            {
                rainSound.Play();
            }

            // Destroy the grenade after collision 
            Destroy(gameObject);
            // call the function once rain bomb hits the target. deal damage.



            // TO BE DETERMINED WHETHER THIS SCRIPT NEEDS TO BE USED.
            // gameManager.OnRainBombImpact();
        }
    }
    */
}
