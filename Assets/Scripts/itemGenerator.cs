using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class itemGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[CustomEditor(typeof(itemGenerator))]
public class itemGenerator_editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        itemGenerator script = (itemGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            itemGenerator_editor_popup window = (itemGenerator_editor_popup)EditorWindow.GetWindow(typeof(itemGenerator_editor_popup));
            window.script = script;
            window.Show();
        }
    }
}

public class itemGenerator_editor_popup : EditorWindow
{
    //BASIC
    public int id;
    public int maxAmount = 1;
    public string itemName;
    public Sprite icon;
    //EQUIPPABLE
    public int weaponId = 0;
    public bool automatic = true;
    public float cooldown = 1f;
    public AnimationClip anim_equip;
    public AnimationClip anim_attack;
    public AnimationClip anim_attack_hit;

    public itemGenerator script;
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        id = EditorGUILayout.IntField("ID:", id);
        maxAmount = EditorGUILayout.IntField("Stacksize:", maxAmount);
        if (GUILayout.Button("Done!"))
        {
            // script.Generate(input_id);
            this.Close();
        }
    }
}
#endif