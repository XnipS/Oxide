using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class ui_crafting : MonoBehaviour
{
    [HideInInspector]
    public playerInventory player;
    Sprite[] icons;

    public Sprite[] tab_icons;
    public string[] tab_names;
    public GameObject tab_prefab;
    public Transform tab_transform;
    ui_crafting_tab[] tab_objs;
    public Text tab_title;

    public Transform pan_transform;
    public GameObject pan_prefab;


    // Start is called before the first frame update
    void Start()
    {
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
        //Open default tab
        //OpenTab(0);
    }

    public void OpenTab(int id)
    {
        //Get icons
        icons = FindObjectOfType<itemDictionary>().icons;
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
        selected = data.Where(x => x.tab == id).ToList();
        List<inv_recipe> final = new List<inv_recipe>();
        //Remove unknown engrams
        foreach (inv_recipe r in selected)
        {
            bool shown = false;
            Debug.Log(player);
            Debug.Log(player.myMemory.itemId.Length);
            for (int i = 0; i < player.myMemory.itemId.Length; i++)
            {
                if (player.myMemory.itemId[i] == r.outputItem)
                {
                    shown = player.myMemory.learned[i];
                }
            }
            if (shown)
            {
                final.Add(r);
            }
        }
        //Render final list
        itemDictionary dic = FindObjectOfType<itemDictionary>();
        foreach (inv_recipe r in final)
        {
            GameObject g = Instantiate(pan_prefab, pan_transform);
            g.GetComponent<ui_craftingSlot>().icon.sprite = icons[r.outputItem];
            g.GetComponent<ui_craftingSlot>().bigText.text = dic.GetDataFromItemID(r.outputItem).title;
            string str = "";
            for (int x = 0; x < r.inputItems.Length; x++)
            {
                str += dic.GetDataFromItemID(r.inputItems[x]).title;
                str += ",";
                str += r.inputAmount[x];
                str += " ";
            }
            g.GetComponent<ui_craftingSlot>().smallText.text = str;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
