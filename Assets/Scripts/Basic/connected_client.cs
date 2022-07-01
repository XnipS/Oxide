using UnityEngine;
using Mirror;

public class connected_client : NetworkBehaviour
{
    [HideInInspector]
    public GameObject myPlayer = null;
    void Start()
    {
        if (hasAuthority)
        {
            //Spawn player if connected
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
                    //Temporary respawn
                    FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(GetComponent<NetworkIdentity>().connectionToClient);
                }
            }
        }
    }
}
