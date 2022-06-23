using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_manager : MonoBehaviour
{
    public Dictionary<ui_hud.statusType, int> maxStats = new Dictionary<ui_hud.statusType, int> (){
	{ui_hud.statusType.health, 100},
	{ui_hud.statusType.water, 250},
	{ui_hud.statusType.hunger, 250}
    };
    public ui_inventory inventory;
    public ui_hud hud;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            ToggleBackpack();
        }
    }

    void ToggleBackpack () {
        if(inventory.inventoryStatus) {
            inventory.CloseInventory();    
        }else {
            inventory.OpenInventory();
        }
    } 
}
