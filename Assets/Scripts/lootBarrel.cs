using UnityEngine;
using Mirror;

public class lootBarrel : NetworkBehaviour
{
    [System.Serializable]
    public class LootTable
    {
        public int id;
        public int maxAmount = 1;
        public float chance;
        public bool blueprint = false;
    }
    public LootTable[] myTable;
    public GameObject droppedItem;

    public int maxItemsDropped;

    public void SpawnLoot()
    {
        if (!isServer) { return; }
        FindObjectOfType<lootDictionary>().RespawnNetwork_LootBarrel(transform.position, transform.rotation, 600f);
        int itemsToDrop = Random.Range(1, maxItemsDropped);
        float maxChance = 0;
        foreach (LootTable l in myTable)
        {
            maxChance += l.chance;
        }
        for (int i = 0; i < itemsToDrop; i++)
        {
            float rnd = Random.Range(0, maxChance);
            float currentChance = 0;
            LootTable target = null;
            foreach (LootTable t in myTable)
            {
                //Debug.Log("[LB] Rnd=" + rnd + "CC=" + currentChance + "LC=" + t.chance);
                if (rnd <= t.chance + currentChance)
                {
                    target = t;
                    break;
                }
                else
                {
                    currentChance += t.chance;
                }
            }
            if (target != null)
            {
                inv_item item = ScriptableObject.CreateInstance<inv_item>();
                item.id = target.id;
                if (target.blueprint)
                {
                    item.blueprint = target.blueprint;
                    item.amount = 1;
                    item.durability = 0;
                    item.ammoLoaded = 0;
                }
                else
                {
                    item.blueprint = target.blueprint;
                    item.amount = Random.Range(1, target.maxAmount);
                    item.durability = Random.Range(0, FindObjectOfType<itemDictionary>().GetDataFromItemID(item.id).maxDurability);
                    item.ammoLoaded = Random.Range(0, FindObjectOfType<itemDictionary>().GetDataFromItemID(item.id).maxAmmo);
                }

                GameObject g = Instantiate(droppedItem, transform.position + Vector3.up, Quaternion.identity);
                NetworkServer.Spawn(g);
                g.GetComponent<droppedItem>().CMD_SyncItemData(item);
            }
            else
            {
                Debug.Log("Loottable is null");
            }
        }
    }
}
