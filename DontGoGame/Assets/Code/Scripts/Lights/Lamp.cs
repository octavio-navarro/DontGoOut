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
    [SerializeField] public LampState state = LampState.ON;
    [SerializeField] KeyCode lampKey = KeyCode.Space;

    [SerializeField] float flickerTime = 0.3f;
    [SerializeField] float flickerMinIntensity = 0.7f;
    [SerializeField] float flickerMaxIntensity = 1.1f;
    [SerializeField] float offsetRadius = 0.1f;

    [SerializeField] float maxOilDuration = 10.0f;
    [SerializeField] int oilRequired = 1;

    CharacterStatus characterStatus;

    Light2D lampLight;

    float oilDuration;

    // Light intensity lerping
    float intensityTime = 1.0f;
    float intensityDuration = 0.0f;
    float startIntensity;
    float endIntensity;
    // Color lerping
    float colorTime = 1.0f;
    float colorDuration = 0.0f;
    Color startColor;
    Color endColor;
    // Displacement lerping
    float displacementTime = 1.0f;
    float displacementDuration = 0.0f;
    Vector3 startDisplacement = Vector3.zero;
    Vector3 endDisplacement = Vector3.zero;
    Vector3 displacement;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            characterStatus = player.GetComponent<CharacterStatus>();
        }

        lampLight = GetComponentInChildren<Light2D>();
        oilDuration = maxOilDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(lampKey)) {
            ToggleLamp();
        }
    }

    public void ToggleLamp()
    {
        switch(state) {
            case LampState.ON:
                state = LampState.TURNING_OFF;
                break;
            case LampState.OFF:
                // Initialize the variables for the turn on lerp
                intensityTime = 0.0f;
                intensityDuration = 0.1f;
                startIntensity = lampLight.intensity;
                endIntensity = flickerMaxIntensity * 2;
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
                if (characterStatus != null) {
                    ConsumeOil();
                }
                PositionFlicker();
                ColorFlicker();
                IntensityFlicker();
                break;
        }
    }

    void TurnOn()
    {
        // Lerp into a higher intensity, then go to the normal state
        lampLight.intensity = Mathf.Lerp(startIntensity,
                                         endIntensity,
                                         intensityTime / intensityDuration);
        intensityTime += Time.deltaTime;
        if(intensityTime > intensityDuration) {
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

    void IntensityFlicker()
    {
        if(intensityTime > intensityDuration) {
            intensityTime = 0.0f;
            intensityDuration = Random.Range(0.1f, flickerTime);
            startIntensity = lampLight.intensity;
            endIntensity = Random.Range(flickerMinIntensity, flickerMaxIntensity);
        }
        lampLight.intensity = Mathf.Lerp(startIntensity,
                                         endIntensity,
                                         intensityTime / intensityDuration);
        intensityTime += Time.deltaTime;
    }

    void ColorFlicker()
    {
        if(colorTime > colorDuration) {
            colorTime = 0.0f;
            colorDuration = Random.Range(0.1f, flickerTime);
            startColor = lampLight.color;
            // Stay within a yellowish range
            endColor = new Color(1.0f, Random.Range(0.8f, 1.0f), 0.6f);
        }
        lampLight.color = Color.Lerp(startColor,
                                     endColor,
                                     colorTime / colorDuration);
        colorTime += Time.deltaTime;
    }

    void PositionFlicker()
    {
        if(displacementTime > displacementDuration) {
            displacementTime = 0.0f;
            displacementDuration = Random.Range(0.1f, flickerTime);
            startDisplacement = endDisplacement;
            endDisplacement = Random.insideUnitCircle * offsetRadius;
        }
        displacement = Vector3.Lerp(startDisplacement,
                                    endDisplacement,
                                    displacementTime / displacementDuration);
        // Apply the displacement on the current position of the light
        lampLight.transform.position = transform.position + displacement;
        displacementTime += Time.deltaTime;
    }

    void ConsumeOil()
    {
        if (characterStatus.oilCans > 0) {
            oilDuration -= Time.deltaTime;
            if (oilDuration <= 1.0f) {
                //state = LampState.RUNNING_OUT;
                if (oilDuration <= 0.0f) {
                    characterStatus.ConsumeOil(oilRequired);
                    oilDuration = maxOilDuration;
                    state = LampState.TURNING_ON;
                }
            }
        } else {
            state = LampState.TURNING_OFF;
        }
    }
}