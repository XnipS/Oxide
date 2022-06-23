using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_inventory : MonoBehaviour
{
    public Transform tra_belt;
    public Transform tra_bag;
    public Transform tra_box;
    public GameObject pre_slot;
    public int bag_count;
    public int belt_count;
    public List<ui_slot> bag;
    public List<ui_slot> belt;
    public List<ui_slot> box;
    public List<inv_item> invent;
    public List<inv_item> storage;
    [HideInInspector]
    public bool inventoryStatus;
    [HideInInspector]
    public playerInventory player;
    Sprite[] icons;
    //PickedItem
    public ui_slotCursor cursor;
    bool picked_picking;
    ui_slot picked_slot;
    inv_item picked_inv;
    bool picked_storage;
    int currentStorageSlots;
    public itemStorage currentStorage;

    void Start()
    {
        //Add initial dev items
        List<inv_item> newInvent = new List<inv_item>();
        foreach (inv_item it in invent)
        {
            newInvent.Add(Instantiate(it));
        }
        invent = newInvent;
        //Get icons
        icons = FindObjectOfType<itemDictionary>().icons;
        //Update
        UpdateBelt();

    }

    void Update()
    {
        //Icon cursor follow real cursor if active
        if (picked_picking)
        {
            Vector2 movePos;
            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out movePos);
            cursor.gameObject.transform.position = canvas.transform.TransformPoint(movePos);
        }
    }

    public void GiveItem(inv_item item)
    {
        int count = 0;
        item = Instantiate(item);
        //Find available slot
        for (int i = 0; i < 30; i++)
        {
            inv_item taken = null;
            foreach (inv_item it in invent)
            {
                if (it.slot == i)
                {
                    taken = it;
                }
            }
            if (taken == null)
            {
                item.slot = i;
                invent.Add(item);
                count += item.amount;
                break;
            }
            else if (taken.id == item.id)
            {
                int max = FindObjectOfType<itemDictionary>().GetDataFromItemID(item.id).maxAmount;
                if ((taken.amount + item.amount) > max)
                {
                    item.amount -= (max - taken.amount);
                    taken.amount = max;
                    count += (max - taken.amount);
                }
                else
                {
                    taken.amount += item.amount;
                    count += item.amount;
                    break;
                }
            }
        }
        FindObjectOfType<ui_notifyManager>().Notify("+" + count + " " + FindObjectOfType<itemDictionary>().GetDataFromItemID(item.id).title, ui_notification.NotifyColourType.green, ui_notification.NotifyIconType.plus);
        //Redrop item because inventory is full
        if (player)
        {
            if (count != item.amount)
            {
                player.CMD_SpawnDroppedItem(item, player.transform.position + player.transform.forward + player.transform.up);
                player.DropItemAnimation();
            }
            return;
        }
        Debug.LogError("NO PLAYER TO DROP ITEM");
    }

    public void UpdateBelt()
    {
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
            ga.GetComponent<ui_slot>().icons = icons; //Assing icons
            ga.GetComponent<ui_slot>().storage = false; //My or other inventory
            belt.Add(ga.GetComponent<ui_slot>()); //Add to list
        }
        //Populate Slots
        foreach (inv_item inv in invent)
        {
            if (inv.slot > 23)
            {
                belt[inv.slot - 24].UpdateIconData(inv);
            }
        }
    }

    public void OpenInventory()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Toggle variable
        inventoryStatus = true;
        //Generate Slots
        for (int x = 0; x < bag_count; x++)
        {
            GameObject ga = Instantiate(pre_slot, tra_bag); //Make slots
            ga.transform.GetChild(1).gameObject.SetActive(false); //Remove slot number
            ga.GetComponent<ui_slot>().slot = x; //Assign slot number
            ga.GetComponent<ui_slot>().icons = icons; //Assing icons
            ga.GetComponent<ui_slot>().storage = false; //My or other inventory
            bag.Add(ga.GetComponent<ui_slot>()); //Add to list
        }
        //Populate Slots
        foreach (inv_item inv in invent)
        {
            if (inv.slot < 24)
            {
                bag[inv.slot].UpdateIconData(inv);
            }
        }
    }
    public void OpenStorage(List<inv_item> store, int slotCount, itemStorage obj)
    {
        currentStorageSlots = slotCount;
        currentStorage = obj;
        storage = store;
        //Generate Slots
        for (int x = 0; x < slotCount; x++)
        {
            GameObject ga = Instantiate(pre_slot, tra_box); //Make slots
            ga.transform.GetChild(1).gameObject.SetActive(false); //Remove slot number
            ga.GetComponent<ui_slot>().slot = x; //Assign slot number
            ga.GetComponent<ui_slot>().icons = icons; //Assing icons
            ga.GetComponent<ui_slot>().storage = true; //My or other inventory
            box.Add(ga.GetComponent<ui_slot>()); //Add to list
        }
        //Populate Slots
        foreach (inv_item inv in storage)
        {
            if (inv.slot < 24)
            {
                box[inv.slot].UpdateIconData(inv);
            }
        }
    }

    public void CloseInventory()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Delete Slots
        inventoryStatus = false;
        foreach (Transform item in tra_bag)
        {
            Destroy(item.gameObject);
        }
        bag = new List<ui_slot>();
        CloseStorage();
    }
    void CloseStorage()
    {
        //Delete Slots
        foreach (Transform item in tra_box)
        {
            Destroy(item.gameObject);
        }
        box = new List<ui_slot>();
    }

    public void UpdateInventory()
    {
        CloseInventory();
        OpenInventory();
    }

    public void UpdateStorage(List<inv_item> storage, int count)
    {
        CloseStorage();
        currentStorage.CMD_UpdateStorage(storage, count);
        OpenStorage(storage, count, currentStorage);
    }

    public void PickedItem(ui_slot input, bool store)
    {
        if (picked_picking) { return; }
        //Find item data we picked up
        if (store)
        {
            foreach (inv_item inv in storage)
            {
                if (inv.slot == input.slot)
                {
                    picked_inv = inv;
                }
            }
        }
        else
        {
            foreach (inv_item inv in invent)
            {
                if (inv.slot == input.slot)
                {
                    picked_inv = inv;
                }
            }
        }
        if (picked_inv == null || picked_inv.id == 0) { return; } //Check if we got data or nothing
        picked_picking = true; //Toggle pick
        picked_storage = store; //Define
        cursor.GetComponentInChildren<Image>().enabled = true; //Enable cursor
        cursor.GetComponentInChildren<Image>().sprite = icons[picked_inv.id]; //Set Icon
        input.icon.enabled = false;
        input.text.enabled = false;
    }
    //Successful drop on slot
    public void DroppedItem(ui_slot newSlot, bool store)
    {
        if (!picked_picking) { return; }
        //Check if slot populated
        inv_item occupied = null;
        if (store)
        {
            foreach (inv_item inv in storage)
            {
                if (inv.slot == newSlot.slot)
                {
                    occupied = inv;
                }
            }
        }
        else
        {
            foreach (inv_item it in invent)
            {
                if (it.slot == newSlot.slot)
                {
                    occupied = it;
                }
            }
        }
        //Check if moved to same slot
        if (occupied == picked_inv) { StopDrag(false); return; }
        if (occupied)
        {
            if (occupied.id != picked_inv.id) { StopDrag(false); return; } //Check if same item in slot
            //Add to slot
            if (Input.GetKey(KeyCode.LeftShift) && picked_inv.amount > 1)
            {
                //Move half
                int amountToSplit = picked_inv.amount / 2; //Split number
                int max = FindObjectOfType<itemDictionary>().GetDataFromItemID(picked_inv.id).maxAmount;
                if ((occupied.amount + amountToSplit) > max)
                {
                    int x = (max - occupied.amount);
                    occupied.amount = max; //Set amount
                    picked_inv.amount -= x; //Remove move amount
                }
                else
                {
                    amountToSplit = occupied.amount - picked_inv.amount;
                    occupied.amount += amountToSplit; //Set amount
                    picked_inv.amount -= amountToSplit; //Remove move amount
                }
            }
            else if (Input.GetKey(KeyCode.LeftControl) && picked_inv.amount > 1)
            {
                //Move half
                int amountToSplit = 1; //Split number
                int max = FindObjectOfType<itemDictionary>().GetDataFromItemID(picked_inv.id).maxAmount;
                if ((occupied.amount + amountToSplit) > max)
                {
                }
                else
                {
                    occupied.amount += amountToSplit; //Set amount
                    picked_inv.amount -= amountToSplit; //Remove move amount
                }
            }
            else
            {
                //Move completely
                int max = FindObjectOfType<itemDictionary>().GetDataFromItemID(picked_inv.id).maxAmount;
                if ((occupied.amount + picked_inv.amount) > max)
                {
                    int amountToSplit = (max - occupied.amount);
                    occupied.amount = max; //Set amount
                    picked_inv.amount -= amountToSplit; //Remove move amount
                }
                else
                {
                    occupied.amount += picked_inv.amount; //Set amount
                    if (picked_storage)
                    {
                        storage.Remove(picked_inv); //Remove empty from inv
                    }
                    else
                    {
                        invent.Remove(picked_inv); //Remove empty from inv
                    }

                }

            }
        }
        else
        {
            //Empty slot
            if (Input.GetKey(KeyCode.LeftShift) && picked_inv.amount > 1)
            {
                //Move half
                int amountToSplit = picked_inv.amount / 2; //Split number
                inv_item newItem = Instantiate(picked_inv); //Duplicate item
                if (store)
                {
                    storage.Add(newItem); //Add item to storage
                }
                else
                {
                    invent.Add(newItem); //Add item to storage
                }
                newItem.amount = amountToSplit; //Set amount
                picked_inv.amount -= amountToSplit; //Remove move amount
                newItem.slot = newSlot.slot; //Set new slot
            }
            else if (Input.GetKey(KeyCode.LeftControl) && picked_inv.amount > 1)
            {
                //Move single
                int amountToSplit = 1; //Split number
                inv_item newItem = Instantiate(picked_inv); //Duplicate item
                if (store)
                {
                    storage.Add(newItem); //Add item to storage
                }
                else
                {
                    invent.Add(newItem); //Add item to storage
                }
                newItem.amount = amountToSplit; //Set amount
                picked_inv.amount -= amountToSplit; //Remove move amount
                newItem.slot = newSlot.slot; //Set new slot
            }
            else
            {
                //Move completely
                if (store == picked_storage)
                {
                    picked_inv.slot = newSlot.slot;
                }
                else
                {
                    if (store)
                    {
                        invent.Remove(picked_inv);
                        picked_inv.slot = newSlot.slot;
                        storage.Add(picked_inv);
                    }
                    else
                    {
                        storage.Remove(picked_inv);
                        picked_inv.slot = newSlot.slot;
                        invent.Add(picked_inv);
                    }
                }
            }
        }
        //Reset
        StopDrag(false);
    }
    //Unsuccessful drop (reset)
    public void StopDrag(bool wantToDrop)
    {
        //Attempt to drop to ground
        if (wantToDrop && picked_inv != null)
        {
            if (player)
            {
                player.DropItemAnimation();
                player.CMD_SpawnDroppedItem(picked_inv, player.transform.position + player.transform.forward + player.transform.up);
                if (picked_storage)
                {
                    storage.Remove(picked_inv);
                }
                else
                {
                    invent.Remove(picked_inv);
                }

            }
        }
        picked_inv = null;
        //UpdateInventory
        UpdateInventory();
        UpdateBelt();
        if (picked_storage)
        {
            UpdateStorage(storage, currentStorageSlots);
            picked_storage = false;
        }
        //Reset Cursor
        picked_picking = false; //Toggle pick
        cursor.GetComponentInChildren<Image>().enabled = false; //Enable cursor
    }
}
