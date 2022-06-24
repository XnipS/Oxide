using UnityEngine;
using Mirror;

public class resourceManager : NetworkBehaviour
{
    [HideInInspector]
    public harvestableNode[] nodes;

    void Start()
    {
        nodes = FindObjectsOfType<harvestableNode>();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].id = i;
        }
    }

    harvestableNode GetNodeWithID(int id)
    {
        foreach (harvestableNode n in nodes)
        {
            if (n.id == id)
            {
                return n;
            }
        }
        Debug.LogError("Node with id:" + id + " not found.");
        return null;
    }

    [Command(requiresAuthority = false)]
    public void CMD_HitNode(int id, NetworkIdentity human, int weaponId)
    {
        harvestableNode target = GetNodeWithID(id);
        if (target.health > 0)
        {
            switch (weaponId)
            {
                case 2://Stone pick
                    RPC_NodeUpdate(target.id, Mathf.Clamp(target.health - 4, 0, 100));
                    inv_item item = new inv_item();
                    item.id = target.resource_id;
                    item.amount = ((target.resource_totalAmount / target.maxHealth)* 4) ;//+ Random.Range(-3, 3);
                    human.GetComponent<playerInventory>().RPC_GiveItem(item, false);
                    break;
                default:
                    //Invalid tool
                    break;
            }
        }

    }
    [ClientRpc]
    public void RPC_NodeUpdate(int id, int newHp)
    {
        harvestableNode target = GetNodeWithID(id);
        target.UpdateNode(newHp);
    }

}
