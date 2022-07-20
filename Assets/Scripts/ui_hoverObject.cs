using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;

public class ui_hoverObject : MonoBehaviour
{
    ui_inventory inv;

    [HideInInspector]
    public playerInventory player = null;
    Camera mainCam;
    public TMP_Text itemText;
    public Slider healthSlider;
    public TMP_Text healthText;
    public LayerMask mask;

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
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, 2f, mask))
        {
            if (hit.collider.GetComponent<droppedItem>() != null && hit.collider.GetComponent<droppedItem>().myData != null)
            {
                //Detected bag
                hover = true;
                string str = itemDictionary.singleton.GetDataFromItemID(hit.collider.GetComponent<droppedItem>().myData.id).title + " (" + hit.collider.GetComponent<droppedItem>().myData.amount + ")";
                if (hit.collider.GetComponent<droppedItem>().myData.blueprint)
                {
                    str = "Pickup: [Blueprint] " + str;
                }
                else
                {
                    str = "Pickup: " + str;
                }
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
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (hit.collider.GetComponent<itemCooker>() != null)
                    {
                        hit.collider.GetComponent<itemCooker>().CMD_AttemptToToggle();
                    }
                }
            }
            if (hit.collider.GetComponent<pickableNode>() != null && inv.inventoryStatus == false)
            {
                //Detected hemp
                hover = true;
                string str = "";
                switch (hit.collider.GetComponent<pickableNode>().type)
                {
                    case 0:
                        str = "Pick Hemp";
                        break;
                    case 1:
                        str = "Pick Shroom";
                        break;
                }
                itemText.text = str;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    FindObjectOfType<resourceManager>().CMD_PickNode(hit.collider.GetComponent<pickableNode>().id, player.GetComponent<NetworkIdentity>());
                }
            }
            if (hit.collider.GetComponent<playerTrap>() != null)
            {
                if (hit.collider.GetComponent<playerTrap>().activated)
                {
                    //Show UI
                    hover = true;
                    switch (hit.collider.GetComponent<playerTrap>().myType)
                    {
                        case playerTrap.TrapType.landmine:
                            itemText.text = "Defuse";
                            break;
                        case playerTrap.TrapType.snaptrap:
                            itemText.text = "Reset";
                            break;
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<playerTrap>().CMD_UseTrap();
                    }
                }
            }
            if (hit.collider.GetComponent<objectHealth>() != null)
            {
                //detected deployable/object
                objectHealth ob = hit.collider.GetComponent<objectHealth>();
                healthSlider.gameObject.SetActive(true);
                healthText.enabled = true;
                healthSlider.maxValue = ob.maxHealth;
                healthSlider.value = ob.currentHealth;
                healthText.text = ob.currentHealth.ToString() + "/" + ob.maxHealth.ToString();
            }
            else if (hit.collider.GetComponent<buildingObject>() != null)
            {
                //detected building
                buildingObject ob = hit.collider.GetComponent<buildingObject>();
                healthSlider.gameObject.SetActive(true);
                healthText.enabled = true;
                healthSlider.maxValue = ob.myBuildingDontUse.maxHealth;
                healthSlider.value = ob.myBuildingDontUse.health;
                healthText.text = ob.myBuildingDontUse.health.ToString() + "/" + ob.myBuildingDontUse.maxHealth.ToString();
            }
            else
            {
                healthSlider.gameObject.SetActive(false);
                healthText.enabled = false;
            }
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
            healthText.enabled = false;
        }
        itemText.enabled = hover;
    }
}
