using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class lootDictionary : NetworkBehaviour
{
    public static lootDictionary singleton { get; private set; }
    public GameObject lootBarrel;

    [System.Serializable]
    public class LootTable
    {
        public int id;
        public int maxAmount = 1;
        public float chance;
        public bool blueprint = false;
    }
    public List<int> loot_bp_common;
    public List<int> loot_bp_uncommon;
    public List<int> loot_bp_rare;
    public List<int> loot_bp_veryrare;
    public LootTable[] loot_airDrop;
    public LootTable[] loot_barrel;

    void Start () {
        singleton = this;
    }

    public inv_item[] GetLoot(LootTable[] table, int itemsToDrop) {
        
        float maxChance = 0;
        List<inv_item> output = new List<inv_item>();
        foreach (LootTable l in table)
        {
            maxChance += l.chance;
        }
        for (int i = 0; i < itemsToDrop; i++)
        {
            float rnd = Random.Range(0, maxChance);
            float currentChance = 0;
            LootTable target = null;
            foreach (LootTable t in table)
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
                    item.durability = Random.Range(0, itemDictionary.singleton.GetDataFromItemID(item.id).maxDurability);
                    item.ammoLoaded = Random.Range(0, itemDictionary.singleton.GetDataFromItemID(item.id).maxAmmo);
                }

                output.Add(item);
            }
            else
            {
                Debug.Log("Loottable is null");
            }
        }
        return output.ToArray();
    }

    [Server]
    public void RespawnNetwork_LootBarrel (Vector3 pos, Quaternion rot, float time) {
        StartCoroutine(RespawnNetwork_LootBarrel_C(pos,rot,time));
    }
    [Server]
    IEnumerator RespawnNetwork_LootBarrel_C (Vector3 pos, Quaternion rot, float time) {
        yield return new WaitForSeconds(time);
        GameObject g = Instantiate(lootBarrel, pos, rot);
        NetworkServer.Spawn(g);
    }
}
