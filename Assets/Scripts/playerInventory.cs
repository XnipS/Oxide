using UnityEngine;
using Mirror;

public class playerInventory : NetworkBehaviour
{
    public GameObject prefab_droppedItem;
    ui_inventory myInv;
    public AnimationClip anim_pickup;
    public AnimationClip anim_drop;
    void Start()
    {
        if (!hasAuthority) { return; }
        myInv = FindObjectOfType<ui_inventory>();
        myInv.player = this;
        FindObjectOfType<ui_hoverObject>().player = this;
    }

    [Command(requiresAuthority = false)]
    public void CMD_SpawnDroppedItem(inv_item data, Vector3 pos)
    {
        GameObject ga = Instantiate(prefab_droppedItem, pos, Quaternion.identity);
        NetworkServer.Spawn(ga);
        ga.GetComponent<droppedItem>().RPC_SyncItemData(data);
    }

    [ClientRpc]
    public void RPC_GiveItem(inv_item data)
    {
        if (hasAuthority)
        {
            myInv.GiveItem(data);
            GetComponent<playerWeapons>().CMD_PlayWeaponAnimation(anim_pickup.name);
        }
    }
    public void DropItemAnimation()
    {
        GetComponent<playerWeapons>().CMD_PlayWeaponAnimation(anim_drop.name);
    }
}
