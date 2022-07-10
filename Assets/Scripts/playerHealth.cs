using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class playerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;
    [SyncVar]
    public float currentHealth = 100f;
    public int gibs;
    public float maxHunger = 500f;
    public float maxWater = 500f;
    float currentHunger = 150f;
    float currentWater = 150f;

    void Start()
    {
        StartCoroutine(StatsThink());
    }

    IEnumerator StatsThink()
    {
        while (true)
        {
            if (currentHunger == 0)
            {
                CMD_TakeDamage(1f, GetComponent<NetworkIdentity>());
            }
            else
            {
                currentHunger -= .01f;
            }
            if (currentWater == 0)
            {
                CMD_TakeDamage(1f, GetComponent<NetworkIdentity>());
            }
            else
            {
                currentWater -= .01f;
            }
            FindObjectOfType<ui_manager>().UpdateStatus(ui_hud.statusType.hunger, currentHunger);
            FindObjectOfType<ui_manager>().UpdateStatus(ui_hud.statusType.water, currentWater);
            yield return new WaitForSeconds(1f);
        }
    }

    [Command(requiresAuthority = false)]
    public void CMD_TakeDamage(float damage, NetworkIdentity damageDealer)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        RPC_UpdateHealth(currentHealth, -damage, damageDealer);
        if (currentHealth == 0)
        {
            FindObjectOfType<effectManager>().CMD_SpawnEffect(gibs, transform.position, transform.rotation);
            foreach (connected_client client in FindObjectsOfType<connected_client>())
            {
                if (client.GetComponent<NetworkIdentity>().connectionToServer == GetComponent<NetworkIdentity>().connectionToServer)
                {
                    client.RPC_PlayerDeath(transform.position);
                }
            }
            //GetComponent<playerMovement>().myClient.RPC_PlayerDeath( GetComponent<playerMovement>().myClient.GetComponent<NetworkIdentity>(),transform.position);
            NetworkServer.Destroy(this.gameObject);
        }
    }

    [ClientRpc]
    public void RPC_UpdateHealth(float hp, float delta, NetworkIdentity damageDealer)
    {
        currentHealth = hp;
        if (hasAuthority)
        {
            FindObjectOfType<ui_hud>().UpdateStatusHud(ui_hud.statusType.health, currentHealth);
            Debug.Log(hp);
        }

    }
}
