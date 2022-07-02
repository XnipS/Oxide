using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ui_craftingSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    Vector2 startScale;
    public Image icon;
    [HideInInspector]
    public Sprite[] icons;
    public TMP_Text bigText;
    public TMP_Text smallText;
    [HideInInspector]
    public int slot;
    public inv_recipe myRec;
    void Start()
    {
        startScale = gameObject.transform.localScale;
        gameObject.transform.localScale = startScale * 0.95f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<ui_crafting>().Craft(myRec);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = startScale * 0.95f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}