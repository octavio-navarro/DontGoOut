/*
Make a light glow using a sine function

Gilberto Echeverria
2023-07-12
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SineGlow : MonoBehaviour
{
    [SerializeField] Light2D glowLight;
    [SerializeField] float minGlowIntensity = 1.0f;
    [SerializeField] float glowSpeed = 1.5f;
    [SerializeField] float glowAmplitude = 0.5f;
    float angle = 0.0f;

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * glowSpeed;
        // Calculate the new intensity
        float intensity = glowAmplitude * (Mathf.Sin(angle) + 1.0f)
                          + minGlowIntensity;
        // Update the light intensity
        glowLight.intensity = intensity;
    }
}
