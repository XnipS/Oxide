using UnityEngine;
using Mirror;
using static lootDictionary;

public class lootBarrel : NetworkBehaviour
{

    public lootDictionary.LootTable[] myTable;
    public GameObject droppedItem;

    public int maxItemsDropped;

    public void SpawnLoot()
    {
        if (!isServer) { return; }
        FindObjectOfType<lootDictionary>().RespawnNetwork_LootBarrel(transform.position, transform.rotation, 600f);
        int itemsToDrop = Random.Range(1, maxItemsDropped);
        inv_item[] data = lootDictionary.singleton.GetLoot(lootDictionary.singleton.loot_barrel, itemsToDrop);
        foreach (inv_item da in data)
        {
            GameObject g = Instantiate(droppedItem, transform.position + Vector3.up, Quaternion.identity);
            NetworkServer.Spawn(g);
            g.GetComponent<droppedItem>().CMD_SyncItemData(da);
        }
    }
}
