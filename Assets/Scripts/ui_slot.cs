using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ui_slot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 startScale;
    public Image icon;
    [HideInInspector]
    public Sprite[] icons;
    public TMP_Text text;
    public Slider durability;
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
        //Debug.Log(data.amount);
        if (FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxAmmo != 0)
        {
            text.text = data.ammoLoaded + "/" + FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxAmmo;
        }
        else
        if (FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxAmount > 1)
        {
            text.text = data.amount.ToString();
        }
        else
        {
            text.text = "";
        }
        durability.value = data.durability;
        durability.maxValue = FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxDurability;
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