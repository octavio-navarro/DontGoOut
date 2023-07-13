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

            // Store the player status variables
            CharacterStatus playerStatus = other.GetComponent<CharacterStatus>();
            PlayerPrefs.SetInt("Health", playerStatus.health);
            PlayerPrefs.SetInt("OilCans", playerStatus.oilCans);
            PlayerPrefs.SetInt("Sanity", playerStatus.sanity);

            // Change to the new scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
