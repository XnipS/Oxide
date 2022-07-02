using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ui_spawnManager : MonoBehaviour
{
    public GameObject pre_spawn;
    public Transform tra_list;
    public GameObject deathScreen;
    public connected_client myClient;
    public bool showing = false;

    public void ShowRespawnPoints()
    {
        showing = true;
        //Remove old ui
        foreach (Transform t in tra_list)
        {
            Destroy(t.gameObject);
        }
        //Show death screen
        deathScreen.SetActive(true);
        //Get all spawns
        sleepingBag[] bags = FindObjectsOfType<sleepingBag>();
        //Get my name
        string myName = FindObjectOfType<oxideNetworkManager>().playerName;
        //Get only my bags
        sleepingBag[] myBags = bags.Where(x => x.owner == myName).ToArray();
        //Show default spawn
        GameObject def = Instantiate(pre_spawn, tra_list);
        def.GetComponent<ui_spawnpoint>().UpdateUI("Respawn", "Respawn on a random beach.", true, Vector3.zero);
        //List mybags
        for (int i = 0; i < myBags.Length; i++)
        {
            GameObject d = Instantiate(pre_spawn, tra_list);
            d.GetComponent<ui_spawnpoint>().UpdateUI("Sleeping Bag " + i + 1, "Respawn in a comfy sleeping bag.", false, myBags[i].transform.position);
        }
    }

    public void Hide (bool rnd, Vector3 pos) {
        deathScreen.SetActive(false);
        showing = false;
        myClient.Respawn(rnd, pos);
    }
}
