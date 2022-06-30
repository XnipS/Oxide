using UnityEngine;
using UnityEditor;
public class craftingDictionary : MonoBehaviour
{
    public inv_recipe[] craftingData;
    public void Regenerate()
    {
        craftingData = Resources.LoadAll<inv_recipe>("Items/Recipe");
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(craftingDictionary))]
public class craftingDictionary_editor : Editor
{
    public override void OnInspectorGUI()
    {
        craftingDictionary man = (craftingDictionary)target;
        if (GUILayout.Button("Regenerate"))
        {
            man.Regenerate();

        }
        base.OnInspectorGUI();
    }
}
#endif