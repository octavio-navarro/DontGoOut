/*
Control the state and behaviour of the lamp

Giberto Echeverria
2023-07-08
*/

using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum LampState
{
    ON,
    OFF,
    TURNING_ON,
    TURNING_OFF,
    RUNNING_OUT
}

public class Lamp : MonoBehaviour
{
    [SerializeField] LampState state = LampState.ON;
    [SerializeField] KeyCode lampKey = KeyCode.Space;

    [SerializeField] float flickerProbability = 50.0f;
    [SerializeField] float flickerMinIntensity = 0.7f;
    [SerializeField] float offsetRadius = 0.1f;

    Light2D lampLight;

    // Start is called before the first frame update
    void Start()
    {
        lampLight = GetComponentInChildren<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(lampKey)) {
            ToggleLamp();
        }
    }

    void ToggleLamp()
    {
        switch(state) {
            case LampState.ON:
                state = LampState.TURNING_OFF;
                break;
            case LampState.OFF:
                state = LampState.TURNING_ON;
                break;
        }
    }

    void FixedUpdate()
    {
        switch(state) {
            case LampState.TURNING_ON:
                TurnOn();
                break;
            case LampState.TURNING_OFF:
                TurnOff();
                break;
            case LampState.ON:
                LightFlicker();
                break;
        }
    }

    void TurnOn()
    {
        // Increase the intensity of the light
        lampLight.intensity += 0.1f;
        // Check if the light is at full intensity
        if (lampLight.intensity >= 1.0f) {
            lampLight.intensity = 1.0f;
            state = LampState.ON;
        }
    }

    void TurnOff()
    {
        // Decrease the intensity of the light
        lampLight.intensity -= 0.1f;
        // Check if the light is at full intensity
        if (lampLight.intensity <= 0.0f) {
            lampLight.intensity = 0.0f;
            state = LampState.OFF;
        }
    }

    void LightFlicker()
    {
        // Use random numbers to determine when and how much the light flickers
        if (UnityEngine.Random.Range(0, 100) < flickerProbability) {
            lampLight.intensity = Random.Range(flickerMinIntensity, 1.0f);
        }
        // Slightly change the position of the light
        if (UnityEngine.Random.Range(0, 100) < flickerProbability) {
            Vector2 offset = Random.insideUnitCircle * offsetRadius;
            // The light position is displaced with respect to this object
            lampLight.transform.position = new Vector3(
                transform.position.x + offset.x,
                transform.position.y + offset.y,
                transform.position.z
            );
        }
    }
}
