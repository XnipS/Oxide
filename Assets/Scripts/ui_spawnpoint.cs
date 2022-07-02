using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ui_spawnpoint : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text bigText;
    public TMP_Text smallText;
    public Vector3 pos;
    public bool rnd;

    public void UpdateUI(string big, string small, bool r, Vector3 poi)
    {
        bigText.text = big;
        smallText.text = small;
        rnd = r;
        pos = poi;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<ui_spawnManager>().Hide(rnd, pos);
    }
}
