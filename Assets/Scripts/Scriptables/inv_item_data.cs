using UnityEngine;

[CreateAssetMenu(fileName = "ID_", menuName = "ScriptableObjects/Inv_Slot_Data", order = 1)]
public class inv_item_data : ScriptableObject
{
    [Header("BASIC")]//////////////////////////////////////
    public int id;
    public int maxAmount = 1;
    public string title;
    public string description = "No description";
    public Sprite icon;
    [Header("EQUIPPABLE")]//////////////////////////////////////
    public int weaponId = 0;
    public int fireType;
    public DamageProfile dmgProfile;
    [Header("Projectile")]
    public GameObject projectile;
    public int projectileCount = 1;
    public float projectileAngle = 0;
    [Header("Ray")]
    public float ray_range = 2f;
    [Header("Ammo")]
    public int ammo = 0;
    public int maxAmmo = 0;
    public bool automatic = true;
    public bool aimRequiredToShoot = false;
    [Header("Effects")]
    public float cooldown = 1f;
    public float deltaFov = 0f;
    public float maxDurability = 0;
    public float durabilityOnUse = 0;
    [Header("Recoil")]
    public Vector2 recoil_std;
    public Vector2 recoil_rnd;
    [Header("Animations")]
    public AnimationClip anim_equip;
    public AnimationClip anim_attack;
    public AnimationClip anim_attack_hit;
    public AnimationClip anim_aim;
    public AnimationClip anim_aim_fire;
    public AnimationClip anim_reload;
    [Header("PLACABALE")]//////////////////////////////////////
    public int placeId = 0;
    [Header("HEALTH EFFECTOR")]//////////////////////////////////////
    public float deltaHealth = 0f;
    public float deltaHunger = 0f;
    public float deltaWater = 0f;
    [System.Serializable]
    public class DamageProfile
    {
        public float damage;

        public float mul_player = 1f;
        public float mul_deployables = 1f;
        public float mul_buildings_stick = 1f;
        public float mul_buildings_wood = 0f;
        public float mul_buildings_stone = 0f;
        public float mul_buildings_metal = 0f;
        public float mul_buildings_reinforced = 0f;
        public float mul_npc = 1f;
    }
}
