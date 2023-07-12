/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] float globalLightIntensity = 0.1f;

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
