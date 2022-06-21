using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemDictionary : MonoBehaviour
{
    //Input
    public inv_item_data[] dataDictionary;
    //Output
    public Sprite[] icons;

    void Start()
    {
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
