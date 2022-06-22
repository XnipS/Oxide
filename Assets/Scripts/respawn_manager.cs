using UnityEngine;
using Mirror;

public class respawn_manager : NetworkBehaviour
{
    public GameObject playerPrefab;

    [Command(requiresAuthority = false)]
    public void CMD_SpawnPlayer(NetworkConnectionToClient request)
    {
        GameObject ga = Instantiate(playerPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(ga, request);
        ga.GetComponent<NetworkIdentity>().AssignClientAuthority(request);

    }
}
