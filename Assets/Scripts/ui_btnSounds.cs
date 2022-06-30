using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ui_btnSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hover;
    public AudioClip click;
    AudioSource soc;
    void Start()
    {
        soc = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hover != null)
        {
            soc.PlayOneShot(hover);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (click != null)
        {
            soc.PlayOneShot(click);
        }
    }
}
