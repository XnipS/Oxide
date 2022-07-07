using UnityEngine;
using Mirror;
using Cinemachine;

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
    inv_item myItem;
    ui_inventory myInv;
    float cooldown;
    public LayerMask mask;
    float viewmodelFov;
    public smoothMouseLook vert1;
    public smoothMouseLook vert2;
    public int currentBeltSlot;
    [SyncVar]
    string current_aimAnim = "";
    void Start()
    {
        anim = GetComponent<Animation>();
        myInv = FindObjectOfType<ui_inventory>();
        viewmodelFov = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
        UpdateViewmodels(0);
    }
    void Update()
    {
        //Animation
        if (current_aimAnim != "")
        {
            anim[current_aimAnim].layer = 9;
            anim.CrossFade(current_aimAnim, 0.1f);
        }
        //Check if mine and inv is closed
        if (!hasAuthority) { return; }
        if (myInv.inventoryStatus) { return; }
        //Viewmodel Camera Fov
        GetComponent<playerMovement>().viewmodelCam.fieldOfView = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;
        //Reload
        if (Input.GetKeyDown(KeyCode.R) && currentData.maxAmmo > 0 && myInv.HowMuch(currentData.ammo) > 0)
        {
            if (currentData.anim_reload != null)
            {
                CMD_PlayWeaponAnimation(currentData.anim_reload.name, currentData.weaponId, false, false);
            }
            else
            {
                //ANIM_ReloadComplete();
            }
        }
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
        //Aim Fov
        CinemachineVirtualCamera cam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        if (isAiming)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, viewmodelFov + currentData.deltaFov, Time.deltaTime * 5f);
        }
        else
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, viewmodelFov, Time.deltaTime * 5f);
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
                                CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true, true);
                                isAiming = true;
                            }
                        }
                        else
                        {

                            CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true, true);
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
                CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true, false);
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false, false);
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
        //Final check 
        if(currentData == null){return;}
        //Check durability
        if (currentData.maxDurability > 0)
        {
            if (myItem.durability > 0)
            {
                myItem.durability -= currentData.durabilityOnUse;
                myInv.UpdateBelt();
            }
            else
            {
                return;
            }
        }
        //Check aim required eg bow
        if (currentData.aimRequiredToShoot)
        {
            if (current_aimAnim == "")
            {
                return;
            }
        }
        //Check ammo
        if (currentData.ammo != 0)
        {
            if (myItem.ammoLoaded > 0)
            {
                myItem.ammoLoaded--;
                myInv.UpdateBelt();
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
        //Apply cooldown
        cooldown = currentData.cooldown;
        //Use aim fire animation or not
        if (currentData.anim_aim_fire != null && isAiming)
        {
            CMD_PlayWeaponAnimation(currentData.anim_aim_fire.name, currentData.weaponId, false, false);
        }
        else
        {
            CMD_PlayWeaponAnimation(currentData.anim_attack.name, currentData.weaponId, false, false);
        }
        //Stop aiming after use eg bow
        if (currentData.aimRequiredToShoot)
        {
            isAiming = false;
            CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true, false);
        }
        //Attack
        if (currentData.anim_attack_hit == null)
        {
            switch (currentData.fireType)
            {
                case (int)FireType.raycast:
                    if (currentData.anim_attack_hit == null)
                    {
                        RaycastAttack();
                    }
                    break;
                case (int)FireType.projectile:
                    //Apply recoil
                    GetComponent<smoothMouseLook>().rotationX += Random.Range(-currentData.recoil_std.x, currentData.recoil_std.x) + Random.Range(-currentData.recoil_rnd.x, currentData.recoil_rnd.x);
                    vert1.rotationY += currentData.recoil_std.y + Random.Range(0, currentData.recoil_rnd.y);
                    vert2.rotationY = vert1.rotationY;
                    CMD_SpawnProjectile(currentData.id, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.LookRotation(Camera.main.transform.forward));
                    break;
            }
        }
    }

    void RaycastAttack()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 2f, mask);


        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<harvestableNode>())
            {
                //Animation confirm
                if (currentData.anim_attack_hit != null)
                {
                    CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId, false, false);
                }
                //Hit rock node
                FindObjectOfType<effectManager>().CMD_SpawnEffect(0, hit.point, Quaternion.LookRotation(hit.normal));
                FindObjectOfType<resourceManager>().CMD_HitNode(hit.collider.GetComponent<harvestableNode>().id, GetComponent<NetworkIdentity>(), currentData.weaponId);
                break;
            }
            if (hit.collider.GetComponent<Terrain>())
            {
                //Animation confirm
                if (currentData.anim_attack_hit != null)
                {
                    CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId, false, false);
                }
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
                    //Animation confirm
                    if (currentData.anim_attack_hit != null)
                    {
                        CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId, false, false);
                    }
                    hit.collider.GetComponentInParent<NetworkIdentity>().GetComponentInParent<playerHealth>().CMD_TakeDamage(currentData.ray_damage * hit.collider.GetComponent<playerHitbox>().multiplier, GetComponent<NetworkIdentity>());
                    FindObjectOfType<effectManager>().CMD_SpawnEffect(2, hit.point, Quaternion.identity);
                    break;
                }
            }
            if (hit.collider.GetComponent<objectHealth>())
            {
                //Animation confirm
                FindObjectOfType<effectManager>().CMD_SpawnEffect(hit.collider.GetComponent<objectHealth>().hitEffect, hit.point, Quaternion.identity);
                if (currentData.anim_attack_hit != null)
                {
                    CMD_PlayWeaponAnimation(currentData.anim_attack_hit.name, currentData.weaponId, false, false);
                }
                hit.collider.GetComponent<objectHealth>().CMD_TakeDamage(currentData.ray_damage, GetComponent<NetworkIdentity>());
                break;

            }
        }
    }
    public void ANIM_ReloadComplete()
    {
        if (!hasAuthority) { return; }
        if (currentData.maxAmmo > 0)
        {
            int hm = myInv.HowMuch(currentData.ammo);
            if (hm > 0)
            {
                int take = Mathf.Min(hm, currentData.maxAmmo - myItem.ammoLoaded);
                myInv.DestroyItem(currentData.ammo, take);
                myItem.ammoLoaded += take;
                myInv.UpdateBelt();
            }
        }
    }

    public void ANIM_AttackHit()
    {
        if (!hasAuthority) { return; }
        if (currentData.anim_attack_hit != null && currentData.fireType == (int)FireType.raycast)
        {
            //Apply recoil
            GetComponent<smoothMouseLook>().rotationX += Random.Range(-currentData.recoil_std.x, currentData.recoil_std.x) + Random.Range(-currentData.recoil_rnd.x, currentData.recoil_rnd.x);
            vert1.rotationY += currentData.recoil_std.y + Random.Range(0, currentData.recoil_rnd.y);
            vert2.rotationY = vert1.rotationY;
            RaycastAttack();
        }
    }
    public void EquipSlot(int id)
    {
        //Update slot
        currentBeltSlot = id;
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
        //Stop aiming
        if (current_aimAnim != "" && occupied.slot != myItem.slot)
        {
            CMD_PlayWeaponAnimation(currentData.anim_aim.name, currentData.weaponId, true, false);
            isAiming = false;
        }
        //Update Viewmodels
        if (occupied == null || occupied.blueprint || (occupied.durability == 0 && FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id).maxDurability > 0))
        {
            currentData = null;
            CMD_PlayWeaponAnimation("hands", 0, false, false);
            return;
        }
        if (currentData == null)
        {
            currentData = FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id);
            myItem = occupied;
            if (currentData.weaponId != 0)
            {
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false, false);
            }
            else if (currentData.placeId != 0)
            {
                GetComponent<playerDeployables>().SpawnGhost(currentData.placeId, currentData.id);
                CMD_PlayWeaponAnimation("hands", 0, false, false);
            }
            else
            {
                CMD_PlayWeaponAnimation("hands", 0, false, false);
            }
        }
        else
        if (currentData.id != occupied.id || (currentData.id == occupied.id && FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id).placeId != 0))
        {
            currentData = FindObjectOfType<itemDictionary>().GetDataFromItemID(occupied.id);
            myItem = occupied;
            if (currentData.weaponId != 0)
            {
                CMD_PlayWeaponAnimation(currentData.anim_equip.name, currentData.weaponId, false, false);
            }
            else if (currentData.placeId != 0)
            {
                GetComponent<playerDeployables>().SpawnGhost(currentData.placeId, currentData.id);
                CMD_PlayWeaponAnimation("hands", 0, false, false);
            }
            else
            {
                CMD_PlayWeaponAnimation("hands", 0, false, false);
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
    public void ANIM_SpawnMuzzleEffect(int id)
    {
        snap_muzzle muz = null;
        foreach (GameObject g in weaponObjects[currentWeapon].viewmodels)
        {
            if (g.GetComponentInChildren<snap_muzzle>() != null)
            {
                muz = g.GetComponentInChildren<snap_muzzle>();
            }
        }
        if (muz == null)
        {
            Debug.LogError("No muzzle found!");
        }
        else
        {
            FindObjectOfType<effectManager>().CMD_SpawnEffect(id, muz.transform.position, muz.transform.rotation);
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
    public void CMD_PlayWeaponAnimation(string animation, int viewmodel, bool aimAnim, bool aimState)
    {
        RPC_PlayWeaponAnimation(animation, viewmodel, aimAnim, aimState);
    }
    [ClientRpc]
    public void RPC_PlayWeaponAnimation(string animation, int viewmodel, bool aimAnim, bool aimState)
    {
        UpdateViewmodels(viewmodel);
        if (aimAnim)
        {
            if (aimState)
            {
                current_aimAnim = animation;
            }
            else
            {
                anim.Stop(animation);
                current_aimAnim = "";
            }
        }
        else
        {
            anim[animation].layer = 10;
            anim.Stop(animation);
            anim.CrossFade(animation, 0.1f, PlayMode.StopSameLayer);
        }
    }
}
