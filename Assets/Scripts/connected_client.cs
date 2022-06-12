using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class connected_client : NetworkBehaviour
{
    public playerMovement myPlayer = null;
    void Start()
    {
        if(hasAuthority)
		{
            FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(GetComponent<NetworkIdentity>().connectionToClient);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(hasAuthority)
            {
                if(myPlayer == null)
                {
                    FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(GetComponent<NetworkIdentity>().connectionToClient);
                }
            }
        }
    }
}
