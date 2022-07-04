using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ui_infoPanel : MonoBehaviour
{
    //My Prefab
    public GameObject pre_action;
    public Transform tra_action;
    public GameObject panel;
    //My UI
    public TMP_Text itemTitle;
    public TMP_Text infoText;
    public Image itemSprite;
    //Other
    inv_item myItem;
    bool myStore;
    ui_slot mySlot;

    public void CloseInfoPanel()
    {
        panel.SetActive(false);
    }
    public void UpdateInfoPanel(inv_item it, bool storage, ui_slot slot)
    {
        //Open panel
        panel.SetActive(true);
        //Get data
        inv_item_data data = FindObjectOfType<itemDictionary>().GetDataFromItemID(it.id);
        //Set variables
        myItem = it;
        myStore = storage;
        mySlot = slot;
        //Set ui
        itemTitle.text = data.title;
        infoText.text = data.description;
        itemSprite.sprite = FindObjectOfType<itemDictionary>().icons[it.id];
        //Delete old actions
        foreach (Transform t in tra_action)
        {
            Destroy(t.gameObject);
        }
        //Summon actions
        SummonAction(ui_infoPanel_action.actionType.drop);
        if (it.blueprint)
        {
            SummonAction(ui_infoPanel_action.actionType.learn);
        }
    }

    void SummonAction(ui_infoPanel_action.actionType type)
    {
        GameObject g = Instantiate(pre_action, tra_action);
        g.GetComponent<ui_infoPanel_action>().SetupActionButton(type, mySlot, myStore, myItem);
    }
}
