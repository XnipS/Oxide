using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ui_craftingSlot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    Vector2 startScale;
    public Image icon;
    [HideInInspector]
    public Sprite[] icons;
    public Text bigText;
    public Text smallText;
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
}