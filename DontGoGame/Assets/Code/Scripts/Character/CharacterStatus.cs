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
    [SerializeField] int health = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField] public int oilCans = 3;
    [SerializeField] int maxOilCans = 6;
    [SerializeField] int sanity = 100;
    [SerializeField] int maxSanity = 100;
    [SerializeField] bool isSafe = false;

    // Start is called before the first frame update
    void Start()
    {
        
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

    void DrainSanity(int amount)
    {
        sanity -= amount;
        if (sanity < 0) {
            sanity = 0;
        }
    }

    public void RecoverSanity(int amount)
    {
        if (isSafe && oilCans >= 3) {
            oilCans -= 3;
            sanity += amount;
            if (sanity > maxSanity) {
                sanity = maxSanity;
            }
            health += amount;
            if (health > maxHealth) {
                health = maxHealth;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OilCan") && oilCans < maxOilCans) {
            if (oilCans < maxOilCans) {
                oilCans++;
                Destroy(other.gameObject);
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