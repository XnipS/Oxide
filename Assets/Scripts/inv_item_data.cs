using UnityEngine;

[CreateAssetMenu(fileName = "ID_", menuName = "ScriptableObjects/Inv_Slot_Data", order = 1)]
public class inv_item_data : ScriptableObject
{
    public int id;
    public int maxAmount = 1;
    public string title;
    public Sprite icon;
}
