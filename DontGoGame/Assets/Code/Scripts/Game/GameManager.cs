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
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] public CharacterStatus characterStatus;
    [SerializeField] Monster_FSM[] monsters;
    [SerializeField] Lamp lamp;
    [SerializeField] float globalLightIntensity = 0.1f;
    [SerializeField] float sanityDrainRate = 0.1f;

    LampState currentState;

    [SerializeField] Transform[] houseRestartPositions;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");

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
        playerStatus.sanity = PlayerPrefs.GetFloat("Sanity", playerStatus.maxSanity);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Dim the global light
        globalLight.intensity = globalLightIntensity;

        currentState = lamp.state;
    }

    void SanityEffects()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        characterStatus.DrainSanity(sanityDrainRate * Time.deltaTime);

        if (lamp.state == LampState.OFF || lamp.state == LampState.ON)
        {
            if(lamp.state != currentState)
            {
                currentState = lamp.state;
                
                foreach(Monster_FSM monster in monsters)
                {
                    monster.lampState = currentState;

                    if(currentState == LampState.OFF)
                        monster.GetComponent<NavMeshAgent>().speed *= 0.5f;
                    else if(currentState == LampState.ON)
                        monster.GetComponent<NavMeshAgent>().speed *= 2f;

                }
            }
        }
    }
}
