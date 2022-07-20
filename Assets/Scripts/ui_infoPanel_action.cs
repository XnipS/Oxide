using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ui_infoPanel_action : MonoBehaviour, IPointerClickHandler
{
    public enum actionType
    {
        drop,
        learn,
        upgrade,
        reveal,
        consume
    }
    public actionType myType;
    public Sprite[] icons;
    public TMP_Text m_text;
    public Image m_image;
    public ui_slot mySlot;
    public bool myStore;
    inv_item myItem;

    public void SetupActionButton(actionType type, ui_slot s, bool store, inv_item it)
    {
        //Remember
        myType = type;
        mySlot = s;
        myItem = it;
        myStore = store;
        //Set ui
        switch (type)
        {
            case actionType.drop:
                m_text.text = "Drop";
                m_image.sprite = icons[0];
                break;
            case actionType.learn:
                m_text.text = "Learn";
                m_image.sprite = icons[1];
                break;
            case actionType.upgrade:
                m_text.text = "Upgrade";
                m_image.sprite = icons[2];
                break;
            case actionType.reveal:
                m_text.text = "Reveal";
                m_image.sprite = icons[3];
                break;
            case actionType.consume:
                m_text.text = "Consume";
                m_image.sprite = icons[4];
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (myType)
        {
            case actionType.drop:
                FindObjectOfType<ui_inventory>().PickedItem(mySlot, myStore);
                FindObjectOfType<ui_inventory>().StopDrag(true);
                FindObjectOfType<ui_notifyManager>().Notify("Dropped!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.minus);
                FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
                break;
            case actionType.consume:
                FindObjectOfType<ui_inventory>().DestroyItem(mySlot.slot, 1, myStore);
                playerHealth phealth = FindObjectOfType<ui_inventory>().player.GetComponent<playerHealth>();
                itemDictionary dic = itemDictionary.singleton;
                phealth.currentHealth += dic.GetDataFromItemID(myItem.id).deltaHealth;
                phealth.currentHunger += dic.GetDataFromItemID(myItem.id).deltaHunger;
                phealth.currentWater += dic.GetDataFromItemID(myItem.id).deltaWater;
                if (phealth.currentHealth > phealth.maxHealth)
                {
                    phealth.currentHealth = phealth.maxHealth;
                }
                if (phealth.currentHunger > phealth.maxHunger)
                {
                    phealth.currentHunger = phealth.maxHunger;
                }
                if (phealth.currentWater > phealth.maxWater)
                {
                    phealth.currentWater = phealth.maxWater;
                }
                if (!FindObjectOfType<ui_inventory>().HasEnough(myItem.id, 1))
                {
                    FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
                }
                break;
            case actionType.reveal:
                ui_inventory rinv = FindObjectOfType<ui_inventory>();
                switch (myItem.id)
                {
                    case 30:
                        if (rinv.HasEnough(myItem.id, 20))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = FindObjectOfType<lootDictionary>().loot_bp_common[Random.Range(0, FindObjectOfType<lootDictionary>().loot_bp_common.Count)];
                            i.amount = 1;
                            i.blueprint = true;
                            rinv.DestroyItem(myItem.id, 20);
                            rinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                    case 31:
                        if (rinv.HasEnough(myItem.id, 1))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = FindObjectOfType<lootDictionary>().loot_bp_uncommon[Random.Range(0, FindObjectOfType<lootDictionary>().loot_bp_uncommon.Count)];
                            i.amount = 1;
                            i.blueprint = true;
                            rinv.DestroyItem(myItem.id, 1);
                            rinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                    case 32:
                        if (rinv.HasEnough(myItem.id, 1))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = FindObjectOfType<lootDictionary>().loot_bp_rare[Random.Range(0, FindObjectOfType<lootDictionary>().loot_bp_rare.Count)];
                            i.amount = 1;
                            i.blueprint = true;
                            rinv.DestroyItem(myItem.id, 1);
                            rinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                    case 33:
                        if (rinv.HasEnough(myItem.id, 1))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = FindObjectOfType<lootDictionary>().loot_bp_veryrare[Random.Range(0, FindObjectOfType<lootDictionary>().loot_bp_veryrare.Count)];
                            i.amount = 1;
                            i.blueprint = true;
                            rinv.DestroyItem(myItem.id, 1);
                            rinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;

                }
                if (!FindObjectOfType<ui_inventory>().HasEnough(myItem.id, 1))
                {
                    FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
                }
                break;
            case actionType.learn:
                ui_inventory pinv = FindObjectOfType<ui_inventory>();
                if (pinv.myMemory.Contains(myItem.id))
                {
                    FindObjectOfType<ui_notifyManager>().Notify("Already learned!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.learn);
                }
                else
                {
                    pinv.myMemory.Add(myItem.id);
                    FindObjectOfType<ui_inventory>().DestroyItem(mySlot.slot, myStore);
                    FindObjectOfType<ui_notifyManager>().Notify("Learned!", ui_notification.NotifyColourType.blue, ui_notification.NotifyIconType.learn);
                    FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
                }
                break;
            case actionType.upgrade:
                ui_inventory uiinv = FindObjectOfType<ui_inventory>();
                switch (myItem.id)
                {
                    case 30:
                        if (uiinv.HasEnough(myItem.id, 60))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = 31;
                            i.amount = 1;
                            uiinv.DestroyItem(myItem.id, 60);
                            uiinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                    case 31:
                        if (uiinv.HasEnough(myItem.id, 5))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = 32;
                            i.amount = 1;
                            uiinv.DestroyItem(myItem.id, 5);
                            uiinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                    case 32:
                        if (uiinv.HasEnough(myItem.id, 4))
                        {
                            inv_item i = ScriptableObject.CreateInstance<inv_item>();
                            i.id = 33;
                            i.amount = 1;
                            uiinv.DestroyItem(myItem.id, 4);
                            uiinv.GiveItem(i);
                        }
                        else
                        {
                            FindObjectOfType<ui_notifyManager>().Notify("Not enough!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.plus);
                        }
                        break;
                }
                if (!FindObjectOfType<ui_inventory>().HasEnough(myItem.id, 1))
                {
                    FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
                }
                break;
        }


    }
}
