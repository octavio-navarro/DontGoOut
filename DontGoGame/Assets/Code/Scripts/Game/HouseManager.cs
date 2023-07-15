/*
Script to control the interactions within the safe house

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;

public class HouseManager : MonoBehaviour
{
    [SerializeField] GameObject houseFire;
    [SerializeField] KeyCode interactKey = KeyCode.F;

    CharacterStatus playerStatus;

    bool nearFireplace = false;
    bool fireIsLit = false;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindWithTag("Player").GetComponent<CharacterStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nearFireplace && !fireIsLit && Input.GetKeyDown(interactKey)) {
            Instantiate(houseFire, transform.position, Quaternion.identity);
            playerStatus.RecoverSanity(playerStatus.maxSanity * 0.5f);
            fireIsLit = true;
        }
    }

    void OnTriggerEnter2D( Collider2D other )
    {
        if (other.CompareTag("Player")) {
            nearFireplace = true;
        }
    }

    void OnTriggerExit2D( Collider2D other )
    {
        if (other.CompareTag("Player")) {
            nearFireplace = false;
        }
    }
}
