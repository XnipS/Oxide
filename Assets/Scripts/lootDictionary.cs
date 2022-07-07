using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class lootDictionary : NetworkBehaviour
{
    public GameObject lootBarrel;
    [Server]
    public void RespawnNetwork_LootBarrel (Vector3 pos, Quaternion rot, float time) {
        StartCoroutine(RespawnNetwork_LootBarrel_C(pos,rot,time));
    }
    [Server]
    IEnumerator RespawnNetwork_LootBarrel_C (Vector3 pos, Quaternion rot, float time) {
        Debug.Log("respawning" + time);
        yield return new WaitForSeconds(time);
        Debug.Log("respawned");
        GameObject g = Instantiate(lootBarrel, pos, rot);
        NetworkServer.Spawn(g);
    }
}
