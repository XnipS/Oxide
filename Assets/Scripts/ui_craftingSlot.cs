using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ui_craftingSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 startScale;
    public Image icon;
    [HideInInspector]
    public Sprite[] icons;
    public Text bigText;
    public Text smallText;
    [HideInInspector]
    public int slot;
    void Start () {
        startScale = gameObject.transform.localScale;
        gameObject.transform.localScale = startScale * 0.95f;
    }
    public void UpdateRecipeData (inv_recipe data) {
        
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
        
    }

    public void OnPointerDown(PointerEventData eventData)
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

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}