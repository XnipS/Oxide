using UnityEngine;
using UnityEditor;
public class craftingDictionary : MonoBehaviour
{
    public inv_recipe[] craftingData;
    public void Regenerate()
    {
        craftingData = (inv_recipe[])Resources.FindObjectsOfTypeAll(typeof(inv_recipe));
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