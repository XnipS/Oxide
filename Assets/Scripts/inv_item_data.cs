using UnityEngine;

[CreateAssetMenu(fileName = "ID_", menuName = "ScriptableObjects/Inv_Slot_Data", order = 1)]
public class inv_item_data : ScriptableObject
{
    //BASIC
    public int id;
    public int maxAmount = 1;
    public string title;
    public Sprite icon;
    //EQUIPPABLE
    public int weaponId = 0;
    public bool automatic = true;
    public float cooldown = 1f;
    public AnimationClip anim_equip;
    public AnimationClip anim_attack;
    public AnimationClip anim_attack_hit;
}
