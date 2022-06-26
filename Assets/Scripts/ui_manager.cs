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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && player != null)
        {
            ToggleBackpack();
        }
    }

    void ToggleBackpack()
    {
        if (inventory.inventoryStatus)
        {
            inventory.CloseInventory();
            crafting.UpdateCraftingUI(false);
        }
        else
        {
            inventory.OpenInventory();
            crafting.UpdateCraftingUI(true);
        }
    }
}
