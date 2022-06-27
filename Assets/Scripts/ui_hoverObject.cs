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
        //Test for bag or crate
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.collider.GetComponent<droppedItem>() != null && hit.collider.GetComponent<droppedItem>().myData != null)
            {
                //Detected bag
                hover = true;
                string str = "Pickup: " + FindObjectOfType<itemDictionary>().GetDataFromItemID(hit.collider.GetComponent<droppedItem>().myData.id).title + " (" + hit.collider.GetComponent<droppedItem>().myData.amount + ")";
                itemText.text = str;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.GetComponent<droppedItem>().CMD_Pickup(player.GetComponent<NetworkIdentity>());
                }
            }
            if (hit.collider.GetComponent<itemStorage>() != null && inv.inventoryStatus == false)
            {
                //Detected crate
                hover = true;
                string str = "Open ";
                str += hit.collider.GetComponent<itemStorage>().label;
                itemText.text = str;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    inv.OpenInventory();
                    inv.OpenStorage(hit.collider.GetComponent<itemStorage>().storage, hit.collider.GetComponent<itemStorage>().slots, hit.collider.GetComponent<itemStorage>());
                }
            }
            if (hit.collider.GetComponent<pickableNode>() != null && inv.inventoryStatus == false)
            {
                //Detected hemp
                hover = true;
                string str = "Pick Hemp";
                itemText.text = str;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    FindObjectOfType<resourceManager>().CMD_PickNode(hit.collider.GetComponent<pickableNode>().id, player.GetComponent<NetworkIdentity>());
                }
            }
        }
        itemText.enabled = hover;
    }
}
