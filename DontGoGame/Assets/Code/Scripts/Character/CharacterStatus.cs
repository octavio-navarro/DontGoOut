/*
Variables and methods to affect the current state of the player

Gilberto Echeverria
2023-07-10
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [SerializeField] public int health = 100;
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int oilCans = 3;
    [SerializeField] public int maxOilCans = 6;
    [SerializeField] public float sanity = 100;
    [SerializeField] public float maxSanity = 100;
    [SerializeField] public bool isSafe = false;

    [SerializeField] AudioSource pickBottleSound;
    [SerializeField] AudioSource useBottleSound;

    GameManager manager;

    BottleManager bottleManager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        bottleManager = GameObject.FindWithTag("GameController").GetComponent<BottleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Test methods to check the status of the player
        if (Input.GetKeyDown(KeyCode.X)) {
            DrainSanity(10);
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            RecoverSanity(50);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            DebugResetState();
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) {
            health = 0;
        }
        manager.UpdateHealth();
    }

    public void DrainSanity(float amount)
    {
        if(!isSafe)
        {
            sanity -= amount;
            if (sanity < 0) {
                sanity = 0;
            }
        }
        manager.UpdateSanity();
    }

    public void RecoverHealth(int amount)
    {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
        manager.UpdateHealth();
    }

    public void RecoverSanity(float amount)
    {
        if (isSafe && oilCans >= 3) {
            oilCans -= 3;
            sanity += amount;
            if (sanity > maxSanity) {
                sanity = maxSanity;
            }
        }
        manager.UpdateSanity();
        manager.UpdateBottles();
    }

    public void ConsumeOil(int amount)
    {
        oilCans -= amount;
        if (oilCans < 0) {
            oilCans = 0;
        }
        useBottleSound.Play();
        manager.UpdateBottles();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OilCan") && oilCans < maxOilCans) {
            if (oilCans < maxOilCans) {
                oilCans++;
                Destroy(other.gameObject);
                pickBottleSound.Play();
                manager.UpdateBottles();
                bottleManager.RegisterCollected(other.gameObject);
            }
        }
        if (other.CompareTag("SafeZone")) {
            isSafe = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone")) {
            isSafe = false;
        }
    }

    void DebugResetState()
    {
        health = maxHealth;
        oilCans = maxOilCans;
        sanity = maxSanity;
        manager.UpdateHealth();
        manager.UpdateBottles();
        manager.UpdateSanity();
    }
}