/*
Script to handle the transitions from one scene to another

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanges : MonoBehaviour
{
    [SerializeField] string sceneName = "MainMap";
    [SerializeField] int houseIndex = 0;
    [SerializeField] bool fromMainMap = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            if (fromMainMap) {
                // Store the reference to the house entered
                PlayerPrefs.SetInt("HouseIndex", houseIndex);
            }
            // Change to the new scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
