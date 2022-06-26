using UnityEngine;

[RequireComponent(typeof(reloadAudio))]
public class harvestableNode : MonoBehaviour
{
    public GameObject[] variants;
    public AudioClip varientShiftSound;
    public GameObject varientShiftEffect;
    [HideInInspector]
    public int id;
    public int maxHealth = 100;
    public int health = 100;
    public int resource_id;
    public int resource_totalAmount;
    int lastModel = 0;

    public void UpdateNode(int newHp)
    {
        //Assign server hp
        health = newHp;
        //Clamp health
        if (health > 0)
        {
            GetComponent<Collider>().enabled = true;
            int targetModel = Mathf.Clamp(variants.Length - 1 - (health / (100 / variants.Length)), 0, variants.Length);
            SelectModel(targetModel);
        }
        else
        {
            SelectModel(-1);
            GetComponent<Collider>().enabled = false;
        }
    }

    void SelectModel(int target)
    {
        foreach (GameObject g in variants)
        {
            g.SetActive(false);
        }

        if (target >= 0)
        {
            variants[target].SetActive(true);
            if (lastModel != target)
            {
                Instantiate(varientShiftEffect, transform.position, transform.rotation);
                GetComponent<reloadAudio>().PlayReloadAudio(varientShiftSound);
                lastModel = target;
            }
        }
    }
}
