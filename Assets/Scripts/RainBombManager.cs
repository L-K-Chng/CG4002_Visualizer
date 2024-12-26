using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RainBombManager : MonoBehaviour
{
    public GameObject rainPrefab;
    public float spawnHeight = 1.5f;
    // try vuforia anchoring
    public AnchorBehaviour anchor;

    // Method to spawn the rainbomb at the collision point of the grenade 
    public GameObject DisplayRain(Vector3 hitPosition)
    {
        // Calculate the position directly above the Image Target at the specified height 
        Vector3 spawnPosition = new Vector3(hitPosition.x, hitPosition.y + spawnHeight, hitPosition.z);
        Quaternion rainbombRotation = Quaternion.LookRotation(Vector3.down);



        //Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + 0.5f)); 
        //Quaternion rainbombRotation = Quaternion.LookRotation(Camera.main.transform.rotation); 

        // Instantiate the rainbomb at the calculated position 
        GameObject rainEffect = Instantiate(rainPrefab, spawnPosition, Camera.main.transform.rotation, Camera.main.transform);

        // Make sure the rainbomb stays in world space (no parent-child relationship with the AR objects) 
        rainEffect.transform.SetParent(Camera.main.transform);
        return rainEffect;
    }
}
