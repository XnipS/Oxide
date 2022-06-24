using UnityEngine;
using Mirror;

public class droppedItem : NetworkBehaviour
{
    [HideInInspector]
    [SyncVar]
    public inv_item myData;

    [Command(requiresAuthority = false)]
    public void CMD_SyncItemData(inv_item data)
    {
        myData = data;
        if (isServer)
        {
            RPC_SyncItemData(myData);
        }
    }

    [Command(requiresAuthority = false)]
    public void CMD_Pickup(NetworkIdentity target)
    {
        if (isServer)
        {
            if (target != null)
            {
                if (target.GetComponent<playerInventory>())
                {
                    target.GetComponent<playerInventory>().RPC_GiveItem(myData, true);
                    NetworkServer.Destroy(this.gameObject);
                }
                else
                {
                    Debug.LogError("NO INVENTORUY");
                }
            }
        }
    }

    [ClientRpc]
    public void RPC_SyncItemData(inv_item data)
    {
        myData = data;
    }
}
