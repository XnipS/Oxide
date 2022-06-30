using UnityEngine;
using Mirror;

public class playerWeapons : NetworkBehaviour
{

    public enum FireType
    {
        raycast, projectile
    }
    [System.Serializable]
    public class weaponObject
    {
        public GameObject[] viewmodels;
    }
    public weaponObject[] weaponObjects;
    Animation anim;
    public bool isAiming;
    public int currentWeapon;
    inv_item_data currentData;
    ui_inventory myInv;
    float cooldown;
    public LayerMask mask;
    [SyncVar]
    string current_aimAnim = "";
    void Start()
    {
        anim = GetComponent<Animation>();
        myInv = FindObjectOfType<ui_inventory>();
        UpdateViewmodels(0);
    }
    void Update()
    {
        //Animation
        if (current_aimAnim != "")
        {
            anim[current_aimAnim].layer = 10;
            anim.CrossFade(current_aimAnim, 0.1f);
        }
        //Check if mine and inv is closed
        if (!hasAuthority) { return; }
        if (myInv.inventoryStatus) { return; }
        //Get slot number input
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

        //Decrease weapon cooldown
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        //Past here only if weapon valid
        if (currentData == null || currentData.weaponId == 0) { return; }
        if (currentData.anim_aim != null)
        {
            //Begin aim
            if (Input.GetKey(KeyCode.Mouse1) && current_aimAnim != currentData.anim_aim.name)
            {
                if (currentData.aimRequiredToShoot && cooldown > 0)
                {

                }
                else
                {
                    if (currentData.ammo != 0)
                    {
                        if (currentData.aimRequiredToShoot)
                        {
                            if (myInv.HasEnough(currentData.ammo, 1))
                            {
                                CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true);
                                isAiming = true;
                            }
                        }
                        else
                        {

                            CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true);
                            isAiming = true;

                        }
                    }
                }
            }

            //Stop aim
            if (currentData.aimRequiredToShoot && cooldown > 0 && current_aimAnim != "")
            {
                //CMD_PlayWeaponAnimation("", currentData.weaponId, true);
                // CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false);
            }
            if ((Input.GetKeyUp(KeyCode.Mouse1) && current_aimAnim != ""))
            {
                isAiming = false;
                CMD_PlayWeaponAnimation("", currentData.weaponId, true);
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false);
            }
        }
        //Fire mechanics
        if (cooldown <= 0 && currentData != null && currentData.weaponId != 0)
        {
            if (currentData.automatic)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    DoFireChecks();

                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    DoFireChecks();

                }
            }
        }
    }

    void DoFireChecks()
    {
        if (currentData.aimRequiredToShoot)
        {
            if (current_aimAnim == "")
            {
                return;
            }
        }
        if (currentData.ammo != 0)
        {
            if (myInv.HasEnough(currentData.ammo, 1))
            {
                myInv.DestroyItem(currentData.ammo, 1);
                Fire();
            }
        }
        else
        {

            Fire();

        }
    }

    void Fire()
    {
        cooldown = currentData.cooldown;
        CMD_PlayWeaponAnimation(currentData.anim_attack.name, currentData.weaponId, false);
        if (currentData.aimRequiredToShoot)
        {
            isAiming = false;
            CMD_PlayWeaponAnimation("", currentData.weaponId, true);
        }
        if (currentData.anim_attack_hit == null)
        {
            switch (currentData.fireType)
            {
                case FireType.raycast:
                    if (currentData.anim_attack_hit == null)
                    {
                        RaycastAttack();
                    }
                    break;
                case FireType.projectile:
                    CMD_SpawnProjectile(currentData.id, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.LookRotation(Camera.main.transform.forward));
                    break;
            }
        }
    }

    void RaycastAttack()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position + Camera.main.transform.forward * 0.5f, Camera.main.transform.forward, 2f, mask);


        foreach (RaycastHit hit in hits)
        {
            //Debug.Log("HIT = " + hit.collider);
            if (currentData.anim_attack_hit != null)
            {
                CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId, false);
            }
            if (hit.collider.GetComponent<harvestableNode>())
            {
                //Hit rock node
                FindObjectOfType<effectManager>().CMD_SpawnEffect(0, hit.point, Quaternion.LookRotation(hit.normal));
                FindObjectOfType<resourceManager>().CMD_HitNode(hit.collider.GetComponent<harvestableNode>().id, GetComponent<NetworkIdentity>(), currentData.weaponId);
                break;
            }
            if (hit.collider.GetComponent<Terrain>())
            {
                //Hit terrain & possible tree
                resourceManager.harvestableTree tar = FindObjectOfType<resourceManager>().GetTreeWithPOS(hit.point);
                if (tar != null)
                {
                    FindObjectOfType<effectManager>().CMD_SpawnEffect(1, hit.point, Quaternion.LookRotation(hit.normal));
                    FindObjectOfType<resourceManager>().CMD_HitTree(tar.myId, GetComponent<NetworkIdentity>(), currentData.weaponId);
                }
                break;
            }
            if (hit.collider.GetComponent<playerHitbox>())
            {
                if (GetComponent<NetworkIdentity>() != hit.collider.GetComponentInParent<NetworkIdentity>())
                {
                    hit.collider.GetComponentInParent<NetworkIdentity>().GetComponentInParent<playerHealth>().CMD_TakeDamage(currentData.ray_damage * hit.collider.GetComponent<playerHitbox>().multiplier);
                    FindObjectOfType<effectManager>().CMD_SpawnEffect(2, hit.point, Quaternion.identity);
                    break;
                }
                
            }
        }



    }

    public void ANIM_AttackHit()
    {
        if (!hasAuthority) { return; }
        if (currentData.anim_attack_hit != null && currentData.fireType == FireType.raycast)
        {
            RaycastAttack();

        }

    }
    void EquipSlot(int id)
    {
        //Delete
        GetComponent<playerDeployables>().CancelGhost();
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
        if (occupied == null) { currentData = null; CMD_PlayWeaponAnimation("hands", 0, false); return; }
        if (currentData == null)
        {
            currentData = FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id);
            if (currentData.weaponId != 0)
            {
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false);
            }
            else if (currentData.placeId != 0)
            {
                GetComponent<playerDeployables>().SpawnGhost(currentData.placeId, currentData.id);
                CMD_PlayWeaponAnimation("hands", 0, false);
            }
            else
            {
                CMD_PlayWeaponAnimation("hands", 0, false);
            }
        }
        else
        if (currentData.id != occupied.id)
        {
            currentData = FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id);
            if (currentData.weaponId != 0)
            {
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false);
            }
            else if (currentData.placeId != 0)
            {
                GetComponent<playerDeployables>().SpawnGhost(currentData.placeId, currentData.id);
                CMD_PlayWeaponAnimation("hands", 0, false);
            }
            else
            {
                CMD_PlayWeaponAnimation("hands", 0, false);
            }
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
    public void CMD_SpawnProjectile(int type, Vector3 pos, Quaternion rot)
    {
        GameObject g = Instantiate(FindObjectOfType<itemDictionary>().GetDataFromItemID(type).projectile, pos, rot);
        g.GetComponent<projectile>().owner = GetComponent<NetworkIdentity>();
        NetworkServer.Spawn(g);
    }
    [Command]
    public void CMD_PlayWeaponAnimation(string animation, int viewmodel, bool aimAnim)
    {
        RPC_PlayWeaponAnimation(animation, viewmodel, aimAnim);
    }
    [ClientRpc]
    public void RPC_PlayWeaponAnimation(string animation, int viewmodel, bool aimAnim)
    {
        UpdateViewmodels(viewmodel);
        if (aimAnim)
        {
            current_aimAnim = animation;
        }
        else
        {
            anim[animation].layer = 10;
            anim.Stop(animation);
            anim.CrossFade(animation, 0.1f, PlayMode.StopSameLayer);
        }
    }
}
