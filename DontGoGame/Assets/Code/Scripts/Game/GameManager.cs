/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
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
    [SerializeField] float damageEachNSecs = 10;
    [SerializeField] int damageTaken = 5;

    LampState currentState;

    [SerializeField] bool useUI = false;

    [SerializeField] Transform[] houseRestartPositions;

    [SerializeField] Sprite fullBottle;
    [SerializeField] Sprite emptyBottle;

    [SerializeField] Image[] bottles;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider sanitySlider;

    [SerializeField] VolumeProfile globalVolume;

    CharacterStatus playerStatus;
    float nextDamage = 0;

    float lensDistortionAngle = 0.0f, lensDistortionIntensity = 0.0f, lensDistortionSpeed = 1f;

    float _monsterSpeed;
    float monsterSpeed {
        get {
            return _monsterSpeed;
        }
        set {
            _monsterSpeed = value;
            foreach(Monster_FSM monster in monsters)
            {
                monster.GetComponent<NavMeshAgent>().speed = _monsterSpeed;
                if(currentState == LampState.OFF)
                        monster.GetComponent<NavMeshAgent>().speed *= 0.5f;
                else if(currentState == LampState.ON)
                        monster.GetComponent<NavMeshAgent>().speed *= 2f;   
            }
        }
    }

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");

        PlayerPrefs.DeleteAll();

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

        currentState = lamp.state;

        nextDamage = damageEachNSecs;

        monsterSpeed = 1f;
    }

    void SanityEffects()
    {
        characterStatus.DrainSanity(sanityDrainRate * Time.deltaTime);

        if (characterStatus.sanity <= 75)
        {
            lensDistortionIntensity = 0.1f;
            lensDistortionSpeed = 1.2f;
            monsterSpeed = 1.2f;
        }

        if (characterStatus.sanity <= 50)
        {
            lensDistortionIntensity = 0.3f;
            lensDistortionSpeed = 1.5f;
            monsterSpeed = 1.8f;
        }

        if (characterStatus.sanity <= 25)
        {
            lensDistortionIntensity = 0.5f;
            lensDistortionSpeed = 2f;
            monsterSpeed = 2.5f;
        }

        if (characterStatus.sanity <= 0)
        {
            nextDamage -= Time.deltaTime;

            if (nextDamage <= 0)
            {
                nextDamage = damageEachNSecs;
                characterStatus.TakeDamage(damageTaken);
            }
        }

        UpdatePostProcessingEffects();
    }

    private void UpdatePostProcessingEffects()
    {
        if (globalVolume.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.intensity.value = 1 - (characterStatus.sanity / characterStatus.maxSanity);
        }

        lensDistortionAngle += Time.deltaTime * lensDistortionSpeed;
        float intensity = (Mathf.Sin(lensDistortionAngle) * lensDistortionIntensity + lensDistortionIntensity) * 0.5f + 0.1f;

        if (globalVolume.TryGet<LensDistortion>(out var lensDistortion))
        {
            lensDistortion.intensity.value = intensity;
        }
    }

    void LightEffects()
    {
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
    // Update is called once per frame
    void Update()
    {
        SanityEffects();
        LightEffects();
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
        playerStatus.sanity = PlayerPrefs.GetFloat("Sanity", playerStatus.maxSanity);
    }

    void InitializeUI()
    {
        if (useUI) 
        {
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