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
    [SerializeField] bool isSafe = false;

    GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
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
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) {
            health = 0;
        }
    }

    public void DrainSanity(float amount)
    {
        sanity -= amount;
        if (sanity < 0) {
            sanity = 0;
        }
        manager.UpdateSanity();
    }

    public void RecoverHealth(int amount)
    {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
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
        manager.UpdateHealth();
        manager.UpdateBottles();
    }

    public void ConsumeOil(int amount)
    {
        oilCans -= amount;
        if (oilCans < 0) {
            oilCans = 0;
        }
        manager.UpdateBottles();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OilCan") && oilCans < maxOilCans) {
            if (oilCans < maxOilCans) {
                oilCans++;
                Destroy(other.gameObject);
                manager.UpdateBottles();
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
}