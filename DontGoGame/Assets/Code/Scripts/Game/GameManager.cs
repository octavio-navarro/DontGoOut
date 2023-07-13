/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] float globalLightIntensity = 0.1f;

    [SerializeField] Transform[] houseRestartPositions;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null) {
            InitializePlayer(player);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Dim the global light
        globalLight.intensity = globalLightIntensity;
    }

    void InitializePlayer(GameObject player)
    {
        // Check if we are getting back from a house
        if (SceneManager.GetActiveScene().name == "MainMap") {
            // Get the index of the house to enter, or the default position
            int houseIndex = PlayerPrefs.GetInt("HouseIndex", 0);
            // Set the position of the player
            player.transform.position = houseRestartPositions[houseIndex].position;
        }

        // Restore the saved status of the player
        CharacterStatus playerStatus = player.GetComponent<CharacterStatus>();
        playerStatus.health = PlayerPrefs.GetInt("Health", playerStatus.maxHealth);
        playerStatus.oilCans = PlayerPrefs.GetInt("OilCans", playerStatus.maxOilCans);
        playerStatus.sanity = PlayerPrefs.GetInt("Sanity", playerStatus.maxSanity);
    }
}