using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class itemStorage : NetworkBehaviour
{
    public string label = "Storage";
    [SyncVar]
    public List<inv_item> storage;
    [SyncVar]
    public int slots;
    ui_inventory inv;
    public GameObject prefab_droppedItem;

    void Start()
    {
        inv = FindObjectOfType<ui_inventory>();
        if (!hasAuthority) { return; }
        //Add initial dev items
        List<inv_item> newInvent = new List<inv_item>();
        foreach (inv_item it in storage)
        {
            newInvent.Add(Instantiate(it));
        }
        storage = newInvent;
    }

    [Command(requiresAuthority = false)]
    public void CMD_UpdateStorage(List<inv_item> str, int slo)
    {
        storage = str;
        slots = slo;
        RPC_UpdateStorage(str, slo);
    }

    [ClientRpc]
    public void RPC_UpdateStorage(List<inv_item> str, int slo)
    {
        if (inv.inventoryStatus && inv.currentStorage == this)
        {
            Debug.Log("HERE");
            storage = str;
            slots = slo;
            //inv.CloseInventory();
            // inv.OpenInventory();
            inv.CloseStorage();
            inv.OpenStorage(storage, slots, this);
        }
        else
        {
            storage = str;
            slots = slo;
        }
    }

    public bool HasEnough(int itemId, int amount)
    {
        int needed = amount;
        foreach (inv_item it in storage)
        {
            if (it.id == itemId)
            {
                needed -= it.amount;
            }
        }
        return (needed <= 0);
    }
    public void DestroyItem(int id, int amount)
    {
        //Find available slot
        for (int x = 0; x < amount; x++)
        {
            for (int i = 0; i < 30; i++)
            {
                inv_item taken = null;
                foreach (inv_item it in storage)
                {
                    if (it.slot == i)
                    {
                        taken = it;
                    }
                }
                if (taken != null && taken.id == id)
                {
                    taken.amount--;
                    if (taken.amount == 0)
                    {
                        storage.Remove(taken);

                    }
                    break;
                }
            }
        }
    }
    public void GiveItem(inv_item item)
    {
        int count = 0;
        item = Instantiate(item);
        //Find available slot
        for (int i = 0; i < slots; i++)
        {
            inv_item taken = null;
            foreach (inv_item it in storage)
            {
                if (it.slot == i)
                {
                    taken = it;
                }
            }
            if (taken == null)
            {
                item.slot = i;
                storage.Add(item);
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
        //Redrop item because inventory is full
        if (count != item.amount)
        {
            CMD_SpawnDroppedItem(item, transform.position + transform.forward + transform.up);
            if (GetComponent<itemCooker>() != null)
            {
                GetComponent<itemCooker>().cookingEnabled = false;
                GetComponent<itemCooker>().RPC_UpdateToggle(false);
            }
        }


    }
    [Command(requiresAuthority = false)]
    public void CMD_SpawnDroppedItem(inv_item data, Vector3 pos)
    {
        GameObject ga = Instantiate(prefab_droppedItem, pos, Quaternion.identity);
        NetworkServer.Spawn(ga);
        ga.GetComponent<droppedItem>().RPC_SyncItemData(data);
    }
}
