using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ui_crafting : MonoBehaviour
{
    [HideInInspector]
    public playerInventory player;
    Sprite[] icons;

    public Transform crafting;
    //Tabs
    public Sprite[] tab_icons;
    public string[] tab_names;
    public GameObject tab_prefab;
    public Transform tab_transform;
    ui_crafting_tab[] tab_objs;
    public TMP_Text tab_title;
    public TMP_Text tab_info;
    //Panels
    public Transform pan_transform;
    public GameObject pan_prefab;
    int currentTab;

    ui_inventory inventory;

    public void UpdateCraftingUI(bool enabled)
    {
        crafting.gameObject.SetActive(enabled);
        if (enabled)
        {
            StartCoroutine(OpenTab(currentTab));
        }
    }

    void Start()
    {
        //Start hidden
        UpdateCraftingUI(false);
        //Spawn tabs
        List<ui_crafting_tab> tabs = new List<ui_crafting_tab>();
        for (int i = 0; i < tab_icons.Length; i++)
        {
            GameObject g = Instantiate(tab_prefab, tab_transform);
            g.GetComponent<ui_crafting_tab>().id = i;
            g.GetComponent<ui_crafting_tab>().icon.sprite = tab_icons[i];
            g.GetComponent<ui_crafting_tab>().parent = this;
            tabs.Add(g.GetComponent<ui_crafting_tab>());
        }
        tab_objs = tabs.ToArray();
        //Assing
        inventory = GetComponent<ui_inventory>();
    }

    public IEnumerator OpenTab(int id)
    {
        float dev_startTime = Time.time;
        currentTab = id;
        //Get icons
        icons = itemDictionary.singleton.icons;
        //Set tab name
        tab_title.text = tab_names[id];
        //Delete old data
        foreach (Transform t in pan_transform)
        {
            Destroy(t.gameObject);
        }
        //Get all data in tab
        inv_recipe[] data = FindObjectOfType<craftingDictionary>().craftingData;
        List<inv_recipe> selected = new List<inv_recipe>();
        selected = data.Where(x => ((int)x.tab) == id).ToList();
        List<inv_recipe> final = new List<inv_recipe>();
        //Remove unknown engrams
        foreach (inv_recipe r in selected)
        {
            if (inventory.myMemory.Any(x => x == r.outputItem))
            {
                final.Add(r);
            }
        }
        //Set tab info
        tab_info.text = "UNLOCKED " + final.Count + "/" + selected.Count;
        //Render final list
        itemDictionary dic = itemDictionary.singleton;
        foreach (inv_recipe r in final)
        {
            yield return new WaitForEndOfFrame();
            GameObject g = Instantiate(pan_prefab, pan_transform);
            g.GetComponent<ui_craftingSlot>().icon.sprite = icons[r.outputItem];
            g.GetComponent<ui_craftingSlot>().myRec = r;
            g.GetComponent<ui_craftingSlot>().bigText.text = dic.GetDataFromItemID(r.outputItem).title;
            string str = "";
            for (int x = 0; x < r.inputItems.Length; x++)
            {
                //Check if afford
                if (inventory.HasEnough(r.inputItems[x], r.inputAmount[x]))
                {
                    str += dic.GetDataFromItemID(r.inputItems[x]).title;
                    str += " ";
                    str += r.inputAmount[x];
                    if (x != r.inputItems.Length - 1)
                    {
                        str += ", ";
                    }
                }
                else
                {
                    str += "<color=#ff0000ff>";
                    str += dic.GetDataFromItemID(r.inputItems[x]).title;
                    str += " ";
                    str += r.inputAmount[x];
                    if (x != r.inputItems.Length - 1)
                    {
                        str += ", ";
                    }
                    str += "</color>";
                }
            }
            g.GetComponent<ui_craftingSlot>().smallText.text = str;
        }
        Debug.LogWarning("Crafting UI took: " + (Time.time - dev_startTime));
    }

    public void Craft(inv_recipe recipe)
    {
        //Check if can afford
        for (int x = 0; x < recipe.inputItems.Length; x++)
        {
            //Check if afford
            if (!inventory.HasEnough(recipe.inputItems[x], recipe.inputAmount[x]))
            {
                return;
            }
        }
        //Remove input
        for (int x = 0; x < recipe.inputItems.Length; x++)
        {
            inventory.DestroyItem(recipe.inputItems[x], recipe.inputAmount[x]);
        }
        //Give output
        inv_item it = inv_item.CreateInstance<inv_item>();
        it.amount = recipe.outputAmount;
        it.id = recipe.outputItem;
        it.durability = itemDictionary.singleton.GetDataFromItemID(it.id).maxDurability;
        inventory.GiveItem(it);
        //Refresh
        inventory.CloseInventory();
        UpdateCraftingUI(false);
        inventory.OpenInventory();
        UpdateCraftingUI(true);
    }


}
