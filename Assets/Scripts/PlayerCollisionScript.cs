using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerScript playerScript;
    public GameObject explosionEffect;
    public GameObject rainBombExplosionEffect;
    public AudioSource rainSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBasketball")
        {
            Debug.Log("Enemy AI action hit player!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByBasketballMessage = playerScript.DisplayMessage(playerScript.hitByBasketballText);
            gameManager.PlayerReceiveDamage(hitByBasketballMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemyBowlingBall")
        {
            Debug.Log("Enemy AI action hit player!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByBowlingBallMessage = playerScript.DisplayMessage(playerScript.hitByBowlingBallText);
            gameManager.PlayerReceiveDamage(hitByBowlingBallMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemySoccerBall")
        {
            Debug.Log("Enemy AI action hit player!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitBySoccerBallMessage = playerScript.DisplayMessage(playerScript.hitBySoccerBallText);
            gameManager.PlayerReceiveDamage(hitBySoccerBallMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemyVolleyball")
        {
            Debug.Log("Enemy AI action hit player!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByVolleyballMessage = playerScript.DisplayMessage(playerScript.hitByVolleyballText);
            gameManager.PlayerReceiveDamage(hitByVolleyballMessage);
            Destroy(collision.gameObject);
        }
        

        if (collision.gameObject.tag == "EnemyBullet")
        {
            Debug.Log("Enemy hit player!");
            // gameManager.OnEnemyBulletImpactWithoutShield();
            IEnumerator hitByBulletMessage = playerScript.DisplayMessage(playerScript.hitByBulletText);
            gameManager.PlayerReceiveDamage(hitByBulletMessage);
            Destroy(collision.gameObject);
        }

        // must add some additional shit to instantiate rain
        if (collision.gameObject.tag == "EnemyRainBomb")
        {
            Debug.Log("Enemy hit player!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(rainBombExplosionEffect, collisionPoint, Quaternion.identity);
            rainSound.Play();
            IEnumerator hitByRainBombMessage = playerScript.DisplayMessage(playerScript.hitByRainBombText);
            gameManager.PlayerReceiveDamage(hitByRainBombMessage);
            Destroy(collision.gameObject);
        }
    }
}