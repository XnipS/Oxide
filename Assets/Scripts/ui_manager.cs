using System.Collections.Generic;
using UnityEngine;

public class ui_manager : MonoBehaviour
{
    public Dictionary<ui_hud.statusType, int> maxStats = new Dictionary<ui_hud.statusType, int>(){
    {ui_hud.statusType.health, 100},
    {ui_hud.statusType.water, 250},
    {ui_hud.statusType.hunger, 250}
    };
    public ui_inventory inventory;
    public ui_crafting crafting;
    public ui_hud hud;
    public playerInventory player;
    playerInventory lastPlayer;

    void Update()
    {
        if(lastPlayer == null) {
            CloseAll();
        }
        if (lastPlayer != player)
        {
            lastPlayer = player;
            CloseAll();
            UpdateStatus(ui_hud.statusType.health, 100f);
        }
        if (Input.GetKeyDown(KeyCode.Tab) && player != null)
        {
            ToggleBackpack();
        }
    }

    public void UpdateStatus(ui_hud.statusType type, float update)
    {
        hud.UpdateStatusHud(type, update);
    }

    void ToggleBackpack()
    {
        if (inventory.inventoryStatus)
        {
            CloseAll();
        }
        else
        {
            inventory.OpenInventory();
            crafting.UpdateCraftingUI(true);
        }
    }

    public void CloseAll()
    {
        inventory.CloseInventory();
        crafting.UpdateCraftingUI(false);
            //Delete data
        inventory.currentStorage = null;
        inventory.currentStorageSlots = 0;
    }
}
