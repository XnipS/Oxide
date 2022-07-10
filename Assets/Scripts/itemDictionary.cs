using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class itemDictionary : MonoBehaviour
{
    public static itemDictionary singleton { get; private set; }
    //Input
    public inv_item_data[] dataDictionary;
    //Output
    [HideInInspector]
    public Sprite[] icons;

    public void Regenerate()
    {
        dataDictionary = Resources.LoadAll<inv_item_data>("Items/Data");
        dataDictionary = dataDictionary.OrderBy(x=>x.id).ToArray();
    }

    void Start()
    {
        singleton = this;
        //generate sprite list
        List<Sprite> sprites = new List<Sprite>();
        sprites.Add(null);
        foreach (inv_item_data da in dataDictionary)
        {
            sprites.Add(da.icon);
        }
        icons = sprites.ToArray();
    }

    public inv_item_data GetDataFromItemID(int id)
    {
        foreach (inv_item_data da in dataDictionary)
        {
            if (da.id == id)
            {
                return da;
            }
        }
        Debug.LogError("ID not amended to dictionary.");
        return null;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(itemDictionary))]
public class itemDictionary_editor : Editor
{
    public override void OnInspectorGUI()
    {
        itemDictionary man = (itemDictionary)target;
        if (GUILayout.Button("Regenerate"))
        {
            man.Regenerate();

        }
        base.OnInspectorGUI();
    }
}
#endif