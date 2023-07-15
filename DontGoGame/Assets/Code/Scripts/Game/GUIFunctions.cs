/*
Collection of functions that are used by the GUI

Gilberto Echeverria
2023-07-13
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIFunctions : MonoBehaviour
{
    public void StartGame()
    {
        // Reset the player status
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("OilCans");
        PlayerPrefs.DeleteKey("Sanity");
        PlayerPrefs.SetInt("HouseIndex", 0);
        
        // Reset the collected bottles
        PlayerPrefs.SetString("collected", "{collected:[]}");
        
        // Load the first scene
        SceneManager.LoadScene("MainMap");
    }
}
