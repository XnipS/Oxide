using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class resourceManager : NetworkBehaviour
{
    [HideInInspector]
    public harvestableNode[] nodes;
    //[HideInInspector]
    public class harvestableTree
    {
        public int myId;
        public TreeInstance instance;
        public int terrainId;
        public int treeGibId;
        public Vector3 pos;
        public int maxHealth = 100;
        public int health = 100;
    }
    public harvestableTree[] trees;
    Terrain[] terrains;
    [System.Serializable]
    public class harvestYield {
        public int resource_id;
        public int resource_totalAmount;
    }
    public harvestYield tree;
    public GameObject[] treeGibs;
   

    void Start()
    {
        terrains = FindObjectsOfType<Terrain>();
        List<harvestableTree> newTrees = new List<harvestableTree>();
        for (int x = 0; x < terrains.Length; x++)
        {
            TreeInstance[] targets = terrains[x].terrainData.treeInstances.Where(x => x.prototypeIndex == 8).ToArray();
            for (int i = 0; i < targets.Length; i++)
            {
                harvestableTree terra = new harvestableTree();
                terra.myId = i;
                terra.treeGibId = 0;
                terra.terrainId = x;
                terra.instance = targets[i];
                terra.maxHealth = 100;
                terra.health = 100;
                terra.pos = new Vector3((targets[i].position.x * terrains[x].terrainData.alphamapResolution) + terrains[x].transform.position.x, (targets[i].position.y * terrains[x].terrainData.heightmapScale.y) + terrains[x].transform.position.y, (targets[i].position.z * terrains[x].terrainData.alphamapResolution) + terrains[x].transform.position.z);
                newTrees.Add(terra);
            }
        }
        Debug.Log(newTrees[5].pos);

        trees = newTrees.ToArray();

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
    harvestableTree GetTreeWithID(int id)
    {
        foreach (harvestableTree n in trees)
        {
            if (n.myId == id)
            {
                return n;
            }
        }
        Debug.LogError("Tree with id:" + id + " not found.");
        return null;
    }
    public harvestableTree GetTreeWithPOS(Vector3 pos)
    {
        pos = new Vector3(pos.x, 0, pos.z);
        foreach (harvestableTree n in trees)
        {
            Vector3 p = n.pos;
            p.y = 0;
            if (Vector3.Distance(pos, p) < 1f)
            {
                return n;
            }
        }
        return null;
    }

    void HarvestTree(int damage, int bonus, NetworkIdentity human, harvestableTree target)
    {
        RPC_TreeUpdate(target.myId, Mathf.Clamp(target.health - damage, 0, target.maxHealth));
        inv_item item = inv_item.CreateInstance<inv_item>();
        item.id = tree.resource_id;
        item.amount = ((tree.resource_totalAmount / target.maxHealth) * damage) + Random.Range(-bonus, bonus);
        human.GetComponent<playerInventory>().RPC_GiveItem(item, false);
    }
    void HarvestNode(int damage, int bonus, NetworkIdentity human, harvestableNode target)
    {
        RPC_NodeUpdate(target.id, Mathf.Clamp(target.health - 4, 0, 100));
        inv_item item = inv_item.CreateInstance<inv_item>();
        item.id = target.resource_id;
        item.amount = ((target.resource_totalAmount / target.maxHealth) * 4) + Random.Range(-5, 5);
        human.GetComponent<playerInventory>().RPC_GiveItem(item, false);
    }


    [Command(requiresAuthority = false)]
    public void CMD_HitTree(int id, NetworkIdentity human, int weaponId)
    {
        harvestableTree target = GetTreeWithID(id);
        if (target.health > 0)
        {
            Debug.Log("harvest");
            switch (weaponId)
            {
                case 2://Stone pick 4 damage, 5 bonus
                    HarvestTree(10, 5, human, target);
                    break;
                default:
                    //Invalid tool
                    break;
            }
        }

    }
    [ClientRpc]
    public void RPC_TreeUpdate(int id, int newHp)
    {
        harvestableTree target = GetTreeWithID(id);
        target.health = newHp;
        if (target.health == 0)
        {
            List<TreeInstance> instances = new List<TreeInstance>(terrains[target.terrainId].terrainData.treeInstances);
            instances.Remove(target.instance);
            TerrainCollider terrainCollider = terrains[target.terrainId].GetComponent<TerrainCollider>();
            terrainCollider.enabled = false;
            terrains[target.terrainId].terrainData.SetTreeInstances(instances.ToArray(), true);
            terrainCollider.enabled = true;
            GameObject g = Instantiate(treeGibs[target.treeGibId], target.pos, Quaternion.identity);
            g.GetComponent<Rigidbody>().AddForce(g.transform.forward *5f, ForceMode.VelocityChange);
        }
        //Debug.Log("Harvested " + id);
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
                    HarvestNode(4, 5, human, target);
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
