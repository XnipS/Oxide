using UnityEngine;
using Mirror;

public class respawn_manager : NetworkBehaviour
{
    public GameObject playerPrefab;

    [Command(requiresAuthority = false)]
    public void CMD_SpawnPlayer( Vector3 pos, bool random,NetworkConnectionToClient request)
    {
        if (random)
        {
            Transform spawnPoint = transform.GetChild(Random.Range(0, transform.childCount));
            GameObject ga = Instantiate(playerPrefab, spawnPoint.position + Vector3.up, spawnPoint.rotation);
            NetworkServer.Spawn(ga, request);
            ga.GetComponent<NetworkIdentity>().AssignClientAuthority(request);
        }else {
            GameObject ga = Instantiate(playerPrefab, pos + Vector3.up, Quaternion.identity);
            NetworkServer.Spawn(ga, request);
            ga.GetComponent<NetworkIdentity>().AssignClientAuthority(request);
        }
    }
}
