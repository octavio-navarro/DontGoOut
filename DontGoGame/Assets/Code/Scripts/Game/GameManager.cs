/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using System.Collections;
using System.Collections.Generic;
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
        // Check if we are getting back from a house
        if (SceneManager.GetActiveScene().name == "MainMap") {
            // Get the index of the house to enter, or the default position
            int houseIndex = PlayerPrefs.GetInt("HouseIndex", 0);
            // Set the position of the player
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = houseRestartPositions[houseIndex].position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Dim the global light
        globalLight.intensity = globalLightIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
