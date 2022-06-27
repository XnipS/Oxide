using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class playerDeployables : NetworkBehaviour
{
    [System.Serializable]
    public class deployable
    {
        public GameObject prefab_ghost;
        public GameObject prefab_network;
    }
    public deployable[] deployables;
    public GameObject effect;
    public LayerMask mask;

    GameObject currentGhost;
    int currentId;
    int realId;


    public void CancelGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
        }
        currentGhost = null;
        currentId = 0;
        realId = 0;
    }
    public void SpawnGhost(int id, int itemId)
    {
        currentGhost = Instantiate(deployables[id].prefab_ghost);
        currentId = id;
        realId = itemId;
    }

    void Update()
    {
        if (currentGhost != null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3f, mask))
            {
                //Show ghost
                currentGhost.SetActive(true);
                currentGhost.transform.position = hit.point;
                Quaternion rot = Quaternion.LookRotation(-transform.forward, hit.normal);
                currentGhost.transform.rotation = rot;
                bool can = currentGhost.GetComponent<deployGhost>().SpaceTestCanFit();
                              if (Input.GetKeyDown(KeyCode.Mouse0) && can)
                {
                    CMD_PlaceDeployable(currentId, hit.point, rot);
                    FindObjectOfType<ui_inventory>().DestroyItem(realId, 1);
                    CancelGhost();
                }
            }
            else
            {
                currentGhost.SetActive(false);
            }
        }

    }

    [Command(requiresAuthority = false)]
    public void CMD_PlaceDeployable(int id, Vector3 pos, Quaternion rot)
    {
        GameObject g = Instantiate(deployables[id].prefab_network, pos, rot);
        NetworkServer.Spawn(g);
        RPC_PlaceDeployable(pos);
    }

    [ClientRpc]
    public void RPC_PlaceDeployable(Vector3 pos)
    {
        Instantiate(effect, pos, Quaternion.identity);
    }
}
