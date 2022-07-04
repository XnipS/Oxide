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
            FindObjectOfType<ui_spawnManager>().myClient = this;
        }
    }

    void Update()
    {
        if (hasAuthority && myPlayer == null && !FindObjectOfType<ui_spawnManager>().showing)
        {
            OnDeath();
        }
        if (hasAuthority && myPlayer != null)
        {
            FindObjectOfType<ui_spawnManager>().deathScreen.SetActive(false);
            FindObjectOfType<ui_spawnManager>().showing = false;
        }
    }

    public void OnDeath()
    {
        FindObjectOfType<ui_spawnManager>().ShowRespawnPoints();

    }

    public void Respawn(bool rnd, Vector3 pos)
    {
        if (hasAuthority)
        {
            if (myPlayer == null)
            {
                //Respawn
                //Debug.Log(rnd + " - " + pos + " - " + GetComponent<NetworkIdentity>().connectionToClient);
                FindObjectOfType<respawn_manager>().CMD_SpawnPlayer(pos, rnd, GetComponent<NetworkIdentity>().connectionToClient);
            }
        }
    }
}
