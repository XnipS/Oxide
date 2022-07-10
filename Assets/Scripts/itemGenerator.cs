using UnityEngine;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR

public class itemRemote_editor_popup : EditorWindow
{
    //BASIC
    public int id;
    public int maxAmount = 1;
    public string nameTitle;
    public Sprite icon;
    //EQUIPPABLE
    bool equipable;
    public int weaponId = 0;
    public bool automatic = true;
    public float cooldown = 1f;
    public AnimationClip anim_equip;
    public AnimationClip anim_attack;
    public AnimationClip anim_attack_hit;

    [MenuItem("Oxide/Open Dictionary Controls")]
    static void Init()
    {
        itemRemote_editor_popup window = (itemRemote_editor_popup)EditorWindow.GetWindow(typeof(itemRemote_editor_popup), true, "Dictionary Controls", true);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Item Data Dictionary", EditorStyles.boldLabel);
        if (GUILayout.Button("Regenerate"))
        {
            itemDictionary.singleton.Regenerate();
        }
        GUILayout.Label("Recipe Dictionary", EditorStyles.boldLabel);
        if (GUILayout.Button("Regenerate"))
        {
            FindObjectOfType<craftingDictionary>().Regenerate();
        }
    }
}

public class itemGenerator_editor_popup : EditorWindow
{
    //BASIC
    public int id;
    public int maxAmount = 1;
    public string nameTitle;
    public Sprite icon;
    //EQUIPPABLE
    bool equipable;
    public int weaponId = 0;
    public bool automatic = true;
    public float cooldown = 1f;
    public AnimationClip anim_equip;
    public AnimationClip anim_attack;
    public AnimationClip anim_attack_hit;
    //PLACEABLE
    public bool placeable;
    public int placeId;

    [MenuItem("Oxide/GenerateItem")]
    static void Init()
    {
        itemGenerator_editor_popup window = (itemGenerator_editor_popup)EditorWindow.GetWindow(typeof(itemGenerator_editor_popup), true, "Generate New Item", true);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Basic Required Settings", EditorStyles.boldLabel);
        id = EditorGUILayout.IntField("ID", id);
        maxAmount = EditorGUILayout.IntField("Stack Size", maxAmount);
        nameTitle = EditorGUILayout.TextField("Name", nameTitle);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);
        GUILayout.Label("Optional Settings", EditorStyles.boldLabel);
        equipable = EditorGUILayout.BeginToggleGroup("Equippable", equipable);
        weaponId = EditorGUILayout.IntField("Weapon ID", weaponId);
        automatic = EditorGUILayout.Toggle("Automatic Firing", automatic);
        cooldown = EditorGUILayout.FloatField("Fire cooldown", cooldown);
        anim_equip = (AnimationClip)EditorGUILayout.ObjectField("Equip Animation", anim_equip, typeof(AnimationClip), false);
        anim_attack = (AnimationClip)EditorGUILayout.ObjectField("Attack Animation", anim_attack, typeof(AnimationClip), false);
        anim_attack_hit = (AnimationClip)EditorGUILayout.ObjectField("Hit Animation", anim_attack_hit, typeof(AnimationClip), false);
        EditorGUILayout.EndToggleGroup();
        placeable = EditorGUILayout.BeginToggleGroup("Placeable", placeable);
        placeId = EditorGUILayout.IntField("Placeable ID", placeId);
        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Generate"))
        {
            inv_item_data data = ScriptableObject.CreateInstance<inv_item_data>();
            data.id = id;
            data.maxAmount = maxAmount;
            data.title = nameTitle;
            data.icon = icon;
            data.weaponId = weaponId;
            data.automatic = automatic;
            data.cooldown = cooldown;
            data.anim_equip = anim_equip;
            data.anim_attack_hit = anim_attack_hit;
            data.anim_attack = anim_attack;
            data.placeId = placeId;

            AssetDatabase.CreateAsset(data, "Assets/Resources/Items/Data/ID_" + data.id + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}

public class itemLister_editor_popup : EditorWindow
{
    public string str = "";
    [MenuItem("Oxide/ItemLister")]
    static void Init()
    {
        itemLister_editor_popup window = (itemLister_editor_popup)EditorWindow.GetWindow(typeof(itemLister_editor_popup), true, "Item Dictionary", true);
        window.Show();
    }

    void Initial()
    {
        str = "";
        inv_item_data[] dat = Resources.LoadAll<inv_item_data>("Items/Data");
        dat = dat.OrderBy(x=>x.id).ToArray();
        for (int i = 0; i < dat.Length; i++)
        {
            if (i > 0)
            {
                str += "\n";
            }
            str += dat[i].id;
            str += " - ";
            str += dat[i].title;
        }
    }

    void OnGUI()
    {
        Initial();
        GUILayout.Label("ID - Item", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(str);

    }
}
#endif