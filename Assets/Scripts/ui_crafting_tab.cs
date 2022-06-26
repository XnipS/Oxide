using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ui_crafting_tab : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void UpdateRecipeData(inv_recipe data)
    {

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parent.OpenTab(id);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
