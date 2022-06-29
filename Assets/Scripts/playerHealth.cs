using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class playerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;
    [SyncVar]
    public float currentHealth = 100f;

    public connected_client myClient;

    [Command(requiresAuthority = false)]
    public void CMD_TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);
        RPC_UpdateHealth(currentHealth, -damage);
        if (currentHealth == 0)
        {
            FindObjectOfType<effectManager>().CMD_SpawnEffect(4, transform.position, transform.rotation);
            NetworkServer.Destroy(this.gameObject);
        }
    }
    [ClientRpc]
    public void RPC_UpdateHealth(float hp, float delta)
    {
        currentHealth = hp;
    }

}
