using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class playerTrap : NetworkBehaviour
{
    public enum TrapType
    {
        landmine, snaptrap
    }
    public TrapType myType;
    public LayerMask m_mask;
    public AudioClip activate;
    public AudioClip reset;
    public float initialDamage;
    public int effectId;
    public GameObject[] models; //0=ARmed 1=ACtivated
    [SyncVar]
    public bool activated = false;
    NetworkIdentity target = null;

    [Command(requiresAuthority = false)]
    public void CMD_UseTrap()
    {
        //Update model
        RPC_UseTrap();
        if (myType == TrapType.landmine)
        {
            if (activated)
            {
                NetworkServer.Destroy(gameObject);
                return;
            }
        }
        activated = false;
    }
    [ClientRpc]
    public void RPC_UseTrap()
    {
        //Change state
        activated = false;
        switch (myType)
        {
            case TrapType.snaptrap:

                //Reset model
                models[0].SetActive(true);
                models[1].SetActive(false);
                //Play reset sound
                GetComponent<reloadAudio>().PlayReloadAudio(reset);
                break;
        }
    }
    [ClientRpc]
    public void RPC_ActivateTrap()
    {
        //Active
        activated = true;
        //Play sound
        GetComponent<reloadAudio>().PlayReloadAudio(activate);
        //Swap model
        if (myType == TrapType.snaptrap)
        {
            models[0].SetActive(false);
            models[1].SetActive(true);
        }
    }

    void Update()
    {
        //Only on server
        if (!isServer) { return; }

        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(.5f, .5f, .5f), transform.rotation, m_mask);
        bool hasPlayer = false;
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<playerMovement>() != null)
            {
                hasPlayer = true;
                if (target == null)
                {
                    //Only activate when ready
                    if (activated == true) { return; }
                    //Active
                    activated = true;
                    //Sync
                    RPC_ActivateTrap();
                    //Remeber target
                    target = col.GetComponent<NetworkIdentity>();
                    //Depends on type
                    if (myType == TrapType.snaptrap)
                    {
                        //Do damage
                        target.GetComponent<playerHealth>().CMD_TakeDamage(initialDamage, GetComponent<NetworkIdentity>());
                    }
                }
            }
        }
        //On player leave
        if (!hasPlayer && activated)
        {
            if (target != null && myType == TrapType.landmine)
            {
                //Do damage
                target.GetComponent<playerHealth>().CMD_TakeDamage(initialDamage, GetComponent<NetworkIdentity>());
                FindObjectOfType<effectManager>().CMD_SpawnEffect(effectId, transform.position, Quaternion.identity);
                NetworkServer.Destroy(gameObject);
            }
            target = null;
        }

    }
}
