using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class resourceManager : NetworkBehaviour
{
    [HideInInspector]
    public harvestableNode[] nodes;
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
    public class harvestYield
    {
        public int resource_id;
        public int resource_totalAmount;
    }
    public harvestYield tree;
    public GameObject[] treeGibs;


    void Start()
    {
        //Get terrain
        terrains = FindObjectsOfType<Terrain>();
        //Fill array with trees
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
        trees = newTrees.ToArray();
        //Fill array with nodes
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
        //Update tree array
        RPC_TreeUpdate(target.myId, Mathf.Clamp(target.health - damage, 0, target.maxHealth));
        //Add item to inventory
        inv_item item = inv_item.CreateInstance<inv_item>();
        item.id = tree.resource_id;
        item.amount = ((tree.resource_totalAmount / target.maxHealth) * damage) + Random.Range(0, bonus);
        human.GetComponent<playerInventory>().RPC_GiveItem(item, false);
    }
    void HarvestNode(int damage, int bonus, NetworkIdentity human, harvestableNode target)
    {
        //Update node array
        RPC_NodeUpdate(target.id, Mathf.Clamp(target.health - 4, 0, 100));
        //Add item to inventory
        inv_item item = inv_item.CreateInstance<inv_item>();
        switch (Random.Range(0, 3))
        {
            case 0:
                item.id = 5;
                break;
            case 1:
                item.id = 8;
                break;
            case 2:
                item.id = 9;
                break;
        }
        item.amount = ((target.resource_totalAmount / target.maxHealth) * damage) + Random.Range(0, 5);
        human.GetComponent<playerInventory>().RPC_GiveItem(item, false);
    }

    [Command(requiresAuthority = false)]
    public void CMD_HitTree(int id, NetworkIdentity human, int weaponId)
    {
        harvestableTree target = GetTreeWithID(id);
        if (target.health > 0)
        {
            switch (weaponId)
            {
                case 4://Stone hatchet 
                    HarvestTree(4, 5, human, target);
                    break;
                case 5://metal hatchet
                    HarvestTree(6, 10, human, target);
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
        //Get tree
        harvestableTree target = GetTreeWithID(id);
        target.health = newHp;
        if (target.health == 0)
        {
            //Get and remove instance from array
            List<TreeInstance> instances = new List<TreeInstance>(terrains[target.terrainId].terrainData.treeInstances);
            instances.Remove(target.instance);
            //Refresh collider
            TerrainCollider terrainCollider = terrains[target.terrainId].GetComponent<TerrainCollider>();
            terrainCollider.enabled = false;
            //Assign edited array
            terrains[target.terrainId].terrainData.SetTreeInstances(instances.ToArray(), true);
            terrainCollider.enabled = true;
            //Spawn gibs
            GameObject g = Instantiate(treeGibs[target.treeGibId], target.pos, Quaternion.identity);
            g.GetComponent<Rigidbody>().AddForce(g.transform.forward * 5f, ForceMode.VelocityChange);
        }
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
                case 3://Metal pick
                    HarvestNode(10, 10, human, target);
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
