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
    [SerializeField] GameSettingsSO gameSettings;

    BottleManager bottleManager;

    // Start is called before the first frame update
    void Start()
    {
        bottleManager = GameObject.FindWithTag("GameController").GetComponent<BottleManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            if (fromMainMap) {
                // Store the reference to the house entered
                gameSettings.houseIndex = houseIndex;
                bottleManager.SaveCollected();
            }

            Debug.Log("Changing to scene: " + sceneName);

            // Store the player status variables
            CharacterStatus playerStatus = other.GetComponent<CharacterStatus>();
            gameSettings.health = playerStatus.health;
            gameSettings.oilCans = playerStatus.oilCans;
            gameSettings.sanity = playerStatus.sanity;
            
            // Change to the new scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
