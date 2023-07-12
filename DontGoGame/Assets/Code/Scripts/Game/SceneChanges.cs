/*
Script to handle the transitions from one scene to another

Gilberto Echeverria
2023-07-12
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanges : MonoBehaviour
{
    [SerializeField] string sceneName = "MainMap";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            SceneManager.LoadScene(sceneName);
        }
    }
}
