/*
Script to handle the transitions from one scene to another

Gilberto Echeverria
2023-07-12
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanges : MonoBehaviour
{
    [SerializeField] string sceneName = "MainMap";
    [SerializeField] int houseIndex = 0;
    [SerializeField] bool fromMainMap = true;
    [SerializeField] GameSettingsSO gameSettings;

    BottleManager bottleManager;

    // Start is called before the first frame update
    void Start()
    {
        bottleManager = GameObject.FindWithTag("GameController").GetComponent<BottleManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (fromMainMap) 
            {
                // Store the reference to the house entered
                gameSettings.houseIndex = houseIndex;
                bottleManager.SaveCollected();
            }
            
            if(SceneManager.GetActiveScene().name == "FinalHouse")
            {
                StartCoroutine(DeadPanelFadeIn());
            }
            else
            {
                Debug.Log("Changing to scene: " + sceneName);

                // Store the player status variables
                CharacterStatus playerStatus = other.GetComponent<CharacterStatus>();
                gameSettings.health = playerStatus.health;
                gameSettings.oilCans = playerStatus.oilCans;
                gameSettings.sanity = playerStatus.sanity;
                
                // Change to the new scene
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    IEnumerator DeadPanelFadeIn()
    {
        float alpha = 0f;
        GameObject YouLivedPanel = GameObject.Find("YouLivedPanel");
        GameObject YouLivedText = GameObject.Find("YouLivedText");


        Color originalPanelColor = YouLivedPanel.GetComponent<Image>().color;
        Color originalTextColor = YouLivedText.GetComponentInChildren<TMPro.TMP_Text>().color;

        while(alpha < 1)
        {
            alpha += 0.01f;
            YouLivedPanel.GetComponent<Image>().color = new Color(originalPanelColor.r, originalPanelColor.g, originalPanelColor.b, alpha);
            YouLivedText.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, alpha);

            
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene("TitleProposal");
    }
}
