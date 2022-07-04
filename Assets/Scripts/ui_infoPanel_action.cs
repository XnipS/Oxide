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
        learn
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
                break;
            case actionType.learn:
                playerInventory pinv = FindObjectOfType<ui_inventory>().player.GetComponent<playerInventory>();
                if (pinv.myMemory.Contains(myItem.id))
                {
                    FindObjectOfType<ui_notifyManager>().Notify("Already learned!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.learn);
                }
                else
                {
                    pinv.myMemory.Add(myItem.id);
                    FindObjectOfType<ui_inventory>().DestroyItem(mySlot, myStore);
                    FindObjectOfType<ui_notifyManager>().Notify("Learned!", ui_notification.NotifyColourType.blue, ui_notification.NotifyIconType.learn);
                }
                break;
        }
        FindObjectOfType<ui_infoPanel>().CloseInfoPanel();
    }
}
