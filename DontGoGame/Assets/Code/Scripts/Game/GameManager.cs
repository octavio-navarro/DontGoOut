/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] float globalLightIntensity = 0.1f;

    [SerializeField] bool useUI = false;

    [SerializeField] Transform[] houseRestartPositions;

    [SerializeField] Sprite fullBottle;
    [SerializeField] Sprite emptyBottle;

    [SerializeField] Image[] bottles;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider sanitySlider;

    CharacterStatus playerStatus;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null) {
            playerStatus = player.GetComponent<CharacterStatus>();
            InitializePlayer(player);
            InitializeUI();
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
        playerStatus.health = PlayerPrefs.GetInt("Health", playerStatus.maxHealth);
        playerStatus.oilCans = PlayerPrefs.GetInt("OilCans", playerStatus.maxOilCans);
        playerStatus.sanity = PlayerPrefs.GetInt("Sanity", playerStatus.maxSanity);
    }

    void InitializeUI()
    {
        if (useUI) {
            healthSlider.maxValue = playerStatus.maxHealth;
            sanitySlider.maxValue = playerStatus.maxSanity;
            UpdateBottles();
            UpdateHealth();
            UpdateSanity();
        }
    }

    public void UpdateHealth()
    {
        if (useUI) {
            healthSlider.value = playerStatus.health;
        }
    }

    public void UpdateSanity()
    {
        if (useUI) {
            sanitySlider.value = playerStatus.sanity;
        }
    }   

    public void UpdateBottles()
    {
        if (useUI) {
            for (int i=0; i<playerStatus.maxOilCans; i++) {
                if (i < playerStatus.oilCans) {
                    bottles[i].sprite = fullBottle;
                } else {
                    bottles[i].sprite = emptyBottle;
                }
            }
        }
    }
}