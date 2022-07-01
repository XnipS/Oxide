using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ui_slotCursor : MonoBehaviour
{
    /* if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GetComponentInParent<ui_inventory>().PickedItem(this, storage);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (lastData.pointerEnter && lastData.pointerEnter.GetComponent<ui_slot>())
                {
                    GetComponentInParent<ui_inventory>().DroppedItem(lastData.pointerEnter.GetComponent<ui_slot>(), lastData.pointerEnter.GetComponent<ui_slot>().storage);
                }
                else
                {
                    GetComponentInParent<ui_inventory>().StopDrag(true);
                }
            }*/

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponentInParent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponentInParent<EventSystem>();
    }

    void Update()
    {
        if(GetComponentInParent<ui_inventory>().inventoryStatus) {
        //PICKUP
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            ui_slot detected = null;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<ui_slot>() != null)
                {
                    detected = result.gameObject.GetComponent<ui_slot>();
                    break;
                }
            }
            //Detection
            if (detected)
            {
                detected.GetComponentInParent<ui_inventory>().PickedItem(detected, detected.storage);
            }
        }
        //DROP
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            ui_slot detected = null;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<ui_slot>() != null)
                {
                    detected = result.gameObject.GetComponent<ui_slot>();
                    break;
                }
            }
            //Detection
            if (detected)
            {
                GetComponentInParent<ui_inventory>().DroppedItem(detected, detected.storage);
            }
            else
            {
                GetComponentInParent<ui_inventory>().StopDrag(true);
            }
        }
        }
    }
}
