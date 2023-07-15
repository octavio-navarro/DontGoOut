/*
Script to control the general state of the game

Gilberto Echeverria
2023-07-12
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] Monster_FSM[] monsters;
    [SerializeField] Lamp lamp;
    [SerializeField] Transform[] houseRestartPositions;
    [SerializeField] Sprite fullBottle;
    [SerializeField] Sprite emptyBottle;
    [SerializeField] Image[] bottles;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider sanitySlider;
    [SerializeField] VolumeProfile globalVolume;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] float globalLightIntensity = 0.1f;
    [SerializeField] float sanityDrainRate = 0.1f;
    [SerializeField] float damageEachNSecs = 10;
    [SerializeField] int damageTaken = 5;
    [SerializeField] bool useUI = false;
    [SerializeField] GameSettingsSO gameSettings;
    int currentDialogue = 0;
    LampState currentState;
    GameObject player;
    public CharacterStatus playerStatus;
    CharacterMotion playerMotion;
    CharacterDialogue dialogueController;
    float nextDamage = 0;
    float lensDistortionAngle = 0.0f, lensDistortionIntensity = 0.0f, lensDistortionSpeed = 1f;
    private bool showingText = false;
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
        player = GameObject.FindWithTag("Player");

        if (player != null) {
            playerStatus = player.GetComponent<CharacterStatus>();
            playerMotion = player.GetComponent<CharacterMotion>();
            InitializePlayer(player);
            InitializeUI();
        }

        if(dialogueBox != null)
            dialogueController = dialogueBox.GetComponent<CharacterDialogue>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Dim the global light
        globalLight.intensity = globalLightIntensity;
        currentState = lamp.state;
        nextDamage = damageEachNSecs;
        monsterSpeed = 1f;

        currentDialogue = gameSettings.currentDialogue;

        if((SceneManager.GetActiveScene().name == "MainMap" && currentDialogue == 0) || SceneManager.GetActiveScene().name == "SafeHouse")
                StartCoroutine(StartDialogue());
    }

    void Update()
    {
        if(!showingText)
        {
            SanityEffects();
            LightEffects();
        }

        if(showingText != dialogueController.showingText)
        {
            showingText = dialogueController.showingText;

            if(dialogueController.showingText)
            {
                playerMotion.canMove = false;
            }
            else
            {
                if(dialogueBox.activeSelf)
                    dialogueBox.SetActive(false);

                playerMotion.canMove = true;
            }
            ToggleMonsters();
        }
    }  

    void ToggleMonsters()
    {
        foreach(Monster_FSM monster in monsters)
            monster.gameObject.SetActive(!monster.gameObject.activeSelf);
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1f);

        if(!dialogueBox.activeSelf)
            dialogueBox.SetActive(true);

        dialogueController.LoadDialogue(currentDialogue);
        // currentDialogue++;
        // gameSettings.currentDialogue = currentDialogue;
    }

    void SanityEffects()
    {
        playerStatus.DrainSanity(sanityDrainRate * Time.deltaTime);

        if (playerStatus.sanity <= 75)
        {
            lensDistortionIntensity = 0.1f;
            lensDistortionSpeed = 1.2f;
            monsterSpeed = 1.2f;
        }

        if (playerStatus.sanity <= 50)
        {
            lensDistortionIntensity = 0.3f;
            lensDistortionSpeed = 1.5f;
            monsterSpeed = 1.8f;
        }

        if (playerStatus.sanity <= 25)
        {
            lensDistortionIntensity = 0.5f;
            lensDistortionSpeed = 2f;
            monsterSpeed = 2.5f;
        }

        if (playerStatus.sanity <= 0)
        {
            nextDamage -= Time.deltaTime;

            if (nextDamage <= 0)
            {
                nextDamage = damageEachNSecs;
                playerStatus.TakeDamage(damageTaken);
            }
        }

        UpdatePostProcessingEffects();
    }

    private void UpdatePostProcessingEffects()
    {
        if (globalVolume.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.intensity.value = 1 - (playerStatus.sanity / playerStatus.maxSanity);
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
            if(lamp.state == LampState.ON)
                lamp.ConsumeOil();

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
    void InitializePlayer(GameObject player)
    {
        // Check if we are getting back from a house
        if (SceneManager.GetActiveScene().name == "MainMap") {
            // Get the index of the house to enter, or the default position
            int houseIndex = gameSettings.houseIndex;
            // Set the position of the player
            player.transform.position = houseRestartPositions[houseIndex].position;
        }
        
        // // Restore the saved status of the player
        playerStatus.health = gameSettings.health;
        playerStatus.oilCans = gameSettings.oilCans;
        playerStatus.sanity = gameSettings.sanity;
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