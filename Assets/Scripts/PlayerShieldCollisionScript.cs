using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldCollisionScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject explosionEffect;
    public PlayerScript playerScript;
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
            Debug.Log("Enemy AI action hit player with basketball!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByBasketballMessage = playerScript.DisplayMessage(playerScript.hitByBasketballText);
            gameManager.PlayerReceiveDamageWithShield(hitByBasketballMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemyBowlingBall")
        {
            Debug.Log("Enemy AI action hit player with bowling ball!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByBowlingBallMessage = playerScript.DisplayMessage(playerScript.hitByBowlingBallText);
            gameManager.PlayerReceiveDamageWithShield(hitByBowlingBallMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemySoccerBall")
        {
            Debug.Log("Enemy AI action hit player with soccer ball!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitBySoccerBallMessage = playerScript.DisplayMessage(playerScript.hitBySoccerBallText);
            gameManager.PlayerReceiveDamageWithShield(hitBySoccerBallMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemyVolleyball")
        {
            Debug.Log("Enemy AI action hit player with volleyball!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(explosionEffect, collisionPoint, Quaternion.identity);
            // gameManager.OnEnemyAIActionImpactWithoutShield();
            IEnumerator hitByVolleyballMessage = playerScript.DisplayMessage(playerScript.hitByVolleyballText);
            gameManager.PlayerReceiveDamageWithShield(hitByVolleyballMessage);
            Destroy(collision.gameObject);
        }


        if (collision.gameObject.tag == "EnemyBullet")
        {
            Debug.Log("Enemy hit player with bullet!");
            // gameManager.OnEnemyBulletImpactWithoutShield();
            IEnumerator hitByBulletMessage = playerScript.DisplayMessage(playerScript.hitByBulletText);
            gameManager.PlayerReceiveDamageWithShield(hitByBulletMessage);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "EnemyRainBomb")
        {
            Debug.Log("Enemy hit player with rain bomb!");
            Vector3 collisionPoint = collision.contacts[0].point;
            Instantiate(rainBombExplosionEffect, collisionPoint, Quaternion.identity);
            rainSound.Play();
            IEnumerator hitByRainBombMessage = playerScript.DisplayMessage(playerScript.hitByRainBombText);
            gameManager.PlayerReceiveDamageWithShield(hitByRainBombMessage);
            Destroy(collision.gameObject);
        }
    }
}
