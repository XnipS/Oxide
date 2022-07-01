using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ui_slot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
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
    bool hover = false;
    PointerEventData lastData;
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
    void Update()
    {
        if (hover)
        {
           
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }


    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 0.95f;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}