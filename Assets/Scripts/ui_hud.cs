using UnityEngine;
using UnityEngine.UI;

public class ui_hud : MonoBehaviour
{
    public enum statusType
    {
        health = 0, water = 1, hunger = 2
    }
    public RectMask2D healthMask;
    public RectMask2D hungerMask;
    public RectMask2D waterMask;

    public void UpdateStatusHud(statusType type, int amount)
    {
        ui_manager manager = FindObjectOfType<ui_manager>();
        int max = manager.maxStats[type];
        int val = (215 - ((max / 215) * amount));
        Debug.Log(val);
        switch (type)
        {
            case (statusType.health):
                healthMask.padding = new Vector4(0, 0, val, 0);
                break;
            case (statusType.water):
                waterMask.padding = new Vector4(0, 0, val, 0);
                break;
            case (statusType.hunger):
                hungerMask.padding = new Vector4(0, 0, val, 0);
                break;
        }
    }
}
