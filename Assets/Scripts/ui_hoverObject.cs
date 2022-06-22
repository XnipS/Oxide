using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class ui_hoverObject : MonoBehaviour
{
    ui_inventory inv;
    [HideInInspector]
    public playerInventory player = null;
    Camera mainCam;
    public Text itemText;

    void Start()
    {
        inv = FindObjectOfType<ui_inventory>();
        mainCam = Camera.main;
    }
    void Update()
    {
        //Pickup Items
        if (!player) { itemText.enabled = false; return; }
        bool hover = false;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.collider.GetComponent<droppedItem>() && hit.collider.GetComponent<droppedItem>().myData != null)
            {
                hover = true;
                string str = "Pickup: " + FindObjectOfType<itemDictionary>().GetDataFromItemID(hit.collider.GetComponent<droppedItem>().myData.id).title + " (" + hit.collider.GetComponent<droppedItem>().myData.amount + ")";
                itemText.text = str;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<droppedItem>().CMD_Pickup(player.GetComponent<NetworkIdentity>());
                }
            }
        }
        itemText.enabled = hover;
    }
}
