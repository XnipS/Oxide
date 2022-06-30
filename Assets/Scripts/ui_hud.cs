using UnityEngine;
using UnityEngine.UI;

public class ui_hud : MonoBehaviour
{
    public enum statusType
    {
        health = 0, water = 1, hunger = 2
    }
    public Slider healthMask;
    public Slider hungerMask;
    public Slider waterMask;

    public void UpdateStatusHud(statusType type, float amount)
    {
        ui_manager manager = FindObjectOfType<ui_manager>();
        int max = manager.maxStats[type];
      //  Debug.Log(amount);
        switch (type)
        {
            case (statusType.health):
                healthMask.value = amount;
                break;
            case (statusType.water):
                waterMask.value = amount;
                break;
            case (statusType.hunger):
                hungerMask.value = amount;
                break;
        }
    }
}
