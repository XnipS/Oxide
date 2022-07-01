using UnityEngine;

[CreateAssetMenu(fileName = "ID_", menuName = "ScriptableObjects/Inv_Recipe", order = 1)]
public class inv_recipe : ScriptableObject
{
    public int outputItem;
    public int outputAmount;
    public int tab;
    //Id,Count
    public int[] inputItems;
    public int[] inputAmount;
}
