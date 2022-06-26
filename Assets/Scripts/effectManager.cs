using UnityEngine;
using Mirror;
public class effectManager : NetworkBehaviour
{
    public GameObject[] effects;
    
    [Command(requiresAuthority = false)]
    public void CMD_SpawnEffect(int id, Vector3 pos, Quaternion rotation)
    {
       RPC_SpawnEffect(id,pos,rotation);
    }
    [ClientRpc]
    public void RPC_SpawnEffect(int id, Vector3 pos, Quaternion rotation)
    {
        Instantiate(effects[id], pos, rotation);
    }
}
