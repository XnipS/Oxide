using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_inventory : MonoBehaviour
{
    public Transform tra_belt;
    public Transform tra_bag;
    public GameObject pre_slot;
    public int bag_count;
    public int belt_count;
    public List<ui_slot> bag;
    public List<ui_slot> belt;
    public List<inv_item> invent;
    public bool inventoryStatus;
    //PickedItem
    public ui_slotCursor cursor;
    bool picked_picking;
    ui_slot picked_slot;
    inv_item picked_inv;
    Sprite[] picked_icons;

    void Start () {
        inv_item x = ScriptableObject.CreateInstance<inv_item>();
        x.slot = 23;
        x.id = 1;
        x.amount = 1;
        invent.Add(x);
        inv_item y = ScriptableObject.CreateInstance<inv_item>();
        y.slot = 24;
        y.id = 2;
        invent.Add(y);
        UpdateBelt();
        picked_icons = pre_slot.GetComponent<ui_slot>().icons;
    }

    void Update () {
        //Icon cursor follow real cursor if active
        if(picked_picking) { 
            Vector2 movePos;
            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out movePos);
            cursor.gameObject.transform.position = canvas.transform.TransformPoint(movePos);
        }
    }

    public void UpdateBelt () {
        //Delete old UI
        foreach (Transform item in tra_belt)
        {
            Destroy(item.gameObject);
        }
        //Clear list
        belt = new List<ui_slot>();
        //Generate Slots
        for (int x = 0; x < belt_count; x++)
        {
            GameObject ga = Instantiate(pre_slot, tra_belt); //Make slots
            ga.transform.GetChild(1).gameObject.SetActive(false); //Remove slot number
            ga.GetComponent<ui_slot>().slot = x + 24; //Assign slot number
            belt.Add(ga.GetComponent<ui_slot>()); //Add to list
        }
        //Populate Slots
        foreach(inv_item inv in invent) {
            if(inv.slot > 23) {
                belt[inv.slot - 24].UpdateIconData(inv);
            }
        }
    }

    public void OpenInventory () {
        //Toggle variable
        inventoryStatus = true;
        //Generate Slots
        for (int x = 0; x < bag_count; x++)
        {
            GameObject ga = Instantiate(pre_slot, tra_bag); //Make slots
            ga.transform.GetChild(1).gameObject.SetActive(false); //Remove slot number
            ga.GetComponent<ui_slot>().slot = x; //Assign slot number
            bag.Add(ga.GetComponent<ui_slot>()); //Add to list
        }
        //Populate Slots
        foreach(inv_item inv in invent) {
            if(inv.slot < 24) {
                bag[inv.slot].UpdateIconData(inv);
            }
        }
    }

    public void CloseInventory () {
        //Delete Slots
        inventoryStatus = false;
        foreach (Transform item in tra_bag)
        {
            Destroy(item.gameObject);
        }
        bag = new List<ui_slot>();
    }

    public void UpdateInventory () {
        CloseInventory();
        OpenInventory();
    }

    public void PickedItem (ui_slot input) {
        if(picked_picking) {return;}
        //Find item data we picked up
        foreach(inv_item inv in invent) {
            if(inv.slot == input.slot) {
                picked_inv = inv;
            }
        }
        if(picked_inv == null || picked_inv.id == 0) { return;} //Check if we got data or nothing
        picked_picking = true; //Toggle pick
        cursor.GetComponentInChildren<Image>().enabled = true; //Enable cursor
        cursor.GetComponentInChildren<Image>().sprite = picked_icons[picked_inv.id]; //Set Icon
        input.icon.enabled = false;
        input.text.enabled = false;
    }
    public void DroppedItem (ui_slot input) {
        if(!picked_picking) {return;}
        Debug.Log("Dropped" + input.slot);
        //ChangeSlots
        picked_inv.slot = input.slot;
        StopDrag();
    }
    public void StopDrag () {
        //UpdateInventory
        UpdateInventory();
        UpdateBelt();
        //Reset Cursor
        picked_picking = false; //Toggle pick
        cursor.GetComponentInChildren<Image>().enabled = false; //Enable cursor
    }
}
