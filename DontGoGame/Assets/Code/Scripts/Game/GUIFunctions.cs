/*
Collection of functions that are used by the GUI

Gilberto Echeverria
2023-07-13
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIFunctions : MonoBehaviour
{
    [SerializeField] GameSettingsSO gameSettings;

    public void StartGame()
    {
        // Reset the player status
        gameSettings.health = 100;
        gameSettings.oilCans = 6;
        gameSettings.sanity = 100;
        gameSettings.currentDialogue = 0;
        gameSettings.houseIndex = 0;
        
        // Reset the collected bottles
        PlayerPrefs.SetString("collected", "{collected:[]}");
        
        // Load the first scene
        SceneManager.LoadScene("MainMap");
    }
}
