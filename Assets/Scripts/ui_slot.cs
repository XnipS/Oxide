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
    public GameObject bp;
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

        //Check if has item
        if (data.id == 0)
        {
            bp.SetActive(false); //Hide bp
            icon.enabled = false;//Hide icon
            durability.gameObject.SetActive(false);//Hide durability
            text.text = "";//Hide text
            return;
        }
        //Show icon
        if (data.id != 0)
        {
            icon.enabled = true;
            icon.sprite = icons[data.id];
        }
        //Check if bp
        if (data.blueprint)
        {
            durability.gameObject.SetActive(false);//No durability
            text.text = ""; //No Text
            bp.SetActive(true); //Show bp
            return;
        }
        else
        {
            bp.SetActive(false); //Hide bp
        }
        //Decide text value
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
        //Check if has durability
        if (FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxDurability > 0)
        {
            durability.gameObject.SetActive(true);
            durability.maxValue = FindObjectOfType<itemDictionary>().GetDataFromItemID(data.id).maxDurability;
            durability.value = data.durability;
            //Debug.Log("[UI] Current: " + durability.value + "Max: " + durability.maxValue);
        }
        else
        {
            durability.gameObject.SetActive(false);
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