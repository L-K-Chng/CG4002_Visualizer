using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


// Original rainanchorcontroller

public class RainAnchorController : MonoBehaviour
{
    public GameObject rainPrefab;

    void Start()
    {
        //midAirPositioner.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void ConfigureAnchor(string name, Vector3 position, Quaternion rotation)
    {
        // Instantiate the rain prefab at the specified position and rotation
        GameObject rainInstance = Instantiate(rainPrefab, position, rotation);

        // Get the AnchorBehaviour component
        AnchorBehaviour anchorBehaviour = rainInstance.GetComponent<AnchorBehaviour>();

        if (anchorBehaviour != null)
        {
            // Log the name, position, and rotation for debugging
            Debug.Log($"Anchoring {name} at Position: {position}, Rotation: {rotation}");

            // Use the ConfigureAnchor method to set up the anchor
            anchorBehaviour.ConfigureAnchor(name, position, rotation);
        }
        else
        {
            Debug.LogError("AnchorBehaviour not found on RainPrefab.");
        }
    }
    */

    public GameObject SetAnchor(string name, Vector3 position, Quaternion rotation)
    {
        // Instantiate the rain prefab at the specified position and rotation
        GameObject rainInstance = Instantiate(rainPrefab, position, rotation);

        // Detach the instantiated rain instance from its parent
        rainInstance.transform.SetParent(null, true);  // Setting parent to null ensures it is not linked to any object

        // Get the AnchorBehaviour component
        AnchorBehaviour anchorBehaviour = rainInstance.GetComponent<AnchorBehaviour>();

        if (anchorBehaviour != null)
        {
            // Log the name, position, and rotation for debugging
            Debug.Log($"Anchoring {name} at Position: {position}, Rotation: {rotation}");

            // Use the ConfigureAnchor method to set up the anchor
            anchorBehaviour.ConfigureAnchor(name, position, rotation);
        }
        else
        {
            Debug.LogError("AnchorBehaviour not found on RainPrefab.");
        }

        // Return the instantiated rain instance
        return rainInstance;
    }
}



// Method fails to work.
//public class RainAnchorController : MonoBehaviour
//{
//    public GameObject rainPrefab;  // Reference to the rain effect prefab
//    private List<GameObject> rainInstances = new List<GameObject>();  // List to store multiple rain instances

//    // Method to instantiate and anchor the rain at a specific position
//    public GameObject SetAnchor(string anchorName, Vector3 spawnPosition, Quaternion rotation)
//    {
//        // Instantiate the rain effect at the given position and rotation
//        GameObject newRainInstance = Instantiate(rainPrefab, spawnPosition, rotation);
//        newRainInstance.name = anchorName;  // Set a unique name for the rain instance

//        // Add the new rain instance to the list to keep track of it
//        rainInstances.Add(newRainInstance);

//        // Log the new rain effect's position for debugging
//        Debug.Log($"Rain anchored at world space position: {spawnPosition}");

//        return newRainInstance;  // Return the instantiated rain effect
//    }

//    // Optional: This method allows you to manually add a rain at a specific position
//    public void SpawnRainAtPosition(Vector3 position)
//    {
//        Quaternion rotation = Quaternion.identity;
//        GameObject newRainInstance = Instantiate(rainPrefab, position, rotation);
//        rainInstances.Add(newRainInstance);  // Store this instance in the list
//        Debug.Log($"Rain manually spawned at position: {position}");
//    }

//    // This method returns a list of all instantiated rain instances
//    public List<GameObject> GetAllRainInstances()
//    {
//        return rainInstances;
//    }

//    // Optional: If you need to clean up rain instances (e.g., destroy them)
//    public void ClearAllRainInstances()
//    {
//        foreach (var rain in rainInstances)
//        {
//            if (rain != null)
//            {
//                Destroy(rain);  // Destroy the rain instance
//            }
//        }
//        rainInstances.Clear();  // Clear the list
//        Debug.Log("All rain instances destroyed.");
//    }
//}
