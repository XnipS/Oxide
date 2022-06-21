using UnityEngine;

public class inv_item : ScriptableObject
{
    public int id;
    public int slot;
    public int amount;
    public inv_item (int Id, int Slot, int Amount) {
        id = Id;
        slot = Slot;
        amount = Amount;
    }
}
