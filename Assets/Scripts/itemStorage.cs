using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class itemStorage : NetworkBehaviour
{
    [SyncVar]
    public List<inv_item> storage;
    [SyncVar]
    public int slots;
    ui_inventory inv;
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

            storage = str;
            slots = slo;
            inv.CloseInventory();
            inv.OpenInventory();
            inv.OpenStorage(storage, slots, this);

        }
        else
        {
            storage = str;
            slots = slo;
        }
    }
}
