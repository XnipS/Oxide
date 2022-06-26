using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ui_slot : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IEndDragHandler
{
    Vector2 startScale;
    public Image icon;
    [HideInInspector]
    public Sprite[] icons;
    public Text text;
    [HideInInspector]
    public int slot;
    //[HideInInspector]
    public bool storage;
    void Start()
    {
        startScale = gameObject.transform.localScale;
        gameObject.transform.localScale = startScale * 0.95f;
    }
    public void UpdateIconData(inv_item data)
    {
        if (data.amount != 0)
        {
            text.enabled = true;
            text.text = data.amount.ToString();
        }
        else
        {
            text.enabled = false;
        }
        if (data.id != 0)
        {
            icon.enabled = true;
            icon.sprite = icons[data.id];
        }
        else
        {
            icon.enabled = false;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponentInParent<ui_inventory>().PickedItem(this, storage);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<ui_slot>())
        {
            GetComponentInParent<ui_inventory>().DroppedItem(eventData.pointerEnter.GetComponent<ui_slot>(), eventData.pointerEnter.GetComponent<ui_slot>().storage);
        }
        else
        {
            GetComponentInParent<ui_inventory>().StopDrag(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 0.95f;
    }

}