using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class respawn_manager : NetworkBehaviour
{
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    [Command(requiresAuthority = false)]
    public void CMD_SpawnPlayer (NetworkConnectionToClient request)
	{
        GameObject ga = Instantiate(playerPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(ga, request);
        ga.GetComponent<NetworkIdentity>().AssignClientAuthority(request);

    }
   
}
