using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ui_crafting_tab : MonoBehaviour, IPointerClickHandler
{
    public enum tabType {
        Construction = 0,
        Items = 1,
        Resources = 2,
        Attire = 3,
        Tools = 4,
        Medical = 5,
        Weapons = 6,
        Ammo = 7
    }
    public int id;
    public Image icon;
    public ui_crafting parent;

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(parent.OpenTab(id));
    }
}
