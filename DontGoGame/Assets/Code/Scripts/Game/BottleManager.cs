/*
Keep track of the bottles that have already been picked up by the player

Gilberto Echeverria
2023-07-14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CollectedList
{
    public List<int> collected = new List<int>();
}

public class BottleManager : MonoBehaviour
{
    [SerializeField] GameObject[] bottles;
    [SerializeField] CollectedList collected = new CollectedList();

    // Start is called before the first frame update
    void Start()
    {
        bottles = GameObject.FindGameObjectsWithTag("OilCan");
        // Delete bottles that have been collected
        RemoveUsedBottles();
    }

    void RemoveUsedBottles()
    {
        string collectedString = PlayerPrefs.GetString("collected", "{collected:[]}");
        //Debug.Log("COLLECTED JSON READ: " + collectedString);
        collected = JsonUtility.FromJson<CollectedList>(collectedString);
        foreach(GameObject bottle in bottles) {
            int pos = collected.collected.IndexOf(bottle.GetComponent<BottleID>().id);
            if (pos >= 0) {
                Destroy(bottle);
            }
        }
    }

    public void RegisterCollected(GameObject bottle)
    {
        collected.collected.Add(bottle.GetComponent<BottleID>().id);
    }

    public void SaveCollected()
    {
        string json = JsonUtility.ToJson(collected);
        //Debug.Log("COLLECTED JSON TO SAVE: " + json);
        PlayerPrefs.SetString("collected", json);
    }
}