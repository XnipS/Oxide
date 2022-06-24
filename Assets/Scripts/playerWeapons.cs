using UnityEngine;
using Mirror;

public class playerWeapons : NetworkBehaviour
{
    [System.Serializable]
    public class weaponObject
    {
        public GameObject[] viewmodels;
    }
    public weaponObject[] weaponObjects;
    Animation anim;
    public int currentWeapon;
    inv_item_data currentData;
    ui_inventory myInv;
    float cooldown;
    public LayerMask mask;
    void Start()
    {
        anim = GetComponent<Animation>();
        myInv = FindObjectOfType<ui_inventory>();
        UpdateViewmodels(0);
    }
    void Update()
    {
        if(!hasAuthority) {return;}
        if(myInv.inventoryStatus) {return;}
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipSlot(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EquipSlot(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EquipSlot(6);
        }
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        if (cooldown <= 0 && currentWeapon != 0)
        {
            if (currentData.automatic)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    cooldown = currentData.cooldown;
                    CMD_PlayWeaponAnimation(currentData.anim_attack.name, currentData.weaponId);

                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    cooldown = currentData.cooldown;
                    CMD_PlayWeaponAnimation(currentData.anim_attack.name, currentData.weaponId);

                }
            }
        }

    }
    public void ANIM_AttackHit()
    {
        if(!hasAuthority) {return;}
        if (currentData.anim_attack_hit != null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2f, mask))
            {
                CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId);
                if(hit.collider.GetComponent<harvestableNode>()) {
                    FindObjectOfType<effectManager>().CMD_SpawnEffect(0, hit.point, Quaternion.LookRotation(hit.normal));
                    FindObjectOfType<resourceManager>().CMD_HitNode(hit.collider.GetComponent<harvestableNode>().id, GetComponent<NetworkIdentity>(), currentData.weaponId);
                }
            }

        }

    }
    void EquipSlot(int id)
    {
        //Check if slot populated
        inv_item occupied = null;
        foreach (inv_item it in myInv.invent)
        {
            if (it.slot == id + 23)
            {
                occupied = it;
            }
        }
        //Update Viewmodels
        if (occupied == null) { UpdateViewmodels(0); return; }
        if(currentData != FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id)) {
            currentData = FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id);
            CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId);
        }
    }
    void UpdateViewmodels(int id)
    {
        currentWeapon = id;
        for (int i = 0; i < weaponObjects.Length; i++)
        {
            foreach (GameObject ob in weaponObjects[i].viewmodels)
            {
                ob.SetActive(false);
            }
        }
        foreach (GameObject ob in weaponObjects[id].viewmodels)
        {
            ob.SetActive(true);
        }
    }
    [Command]
    public void CMD_PlayWeaponAnimation(string animation, int viewmodel)
    {
        RPC_PlayWeaponAnimation(animation, viewmodel);
    }
    [ClientRpc]
    public void RPC_PlayWeaponAnimation(string animation, int viewmodel)
    {
        UpdateViewmodels(viewmodel);
        anim[animation].layer = 10;
        anim.Stop(animation);
        anim.CrossFade(animation,0.1f,PlayMode.StopSameLayer);
    }
}
