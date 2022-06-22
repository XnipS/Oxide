using UnityEngine;
using Mirror;

public class connected_client : NetworkBehaviour
{
    [HideInInspector]
    public playerMovement myPlayer = null;
    void Start()
    {
        if (hasAuthority)
        {
            FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (hasAuthority)
            {
                if (myPlayer == null)
                {
                    FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(GetComponent<NetworkIdentity>().connectionToClient);
                }
            }
        }
    }
}
