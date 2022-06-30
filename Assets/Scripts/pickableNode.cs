using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickableNode : MonoBehaviour
{
    public GameObject model;
    public GameObject pickEffect;
    public int id;
    public int type;
    public void UpdateNode(bool enabled)
    {
        if (!enabled)
        {
            Instantiate(pickEffect, transform.position, Quaternion.identity);
        }
        GetComponent<MeshRenderer>().enabled = enabled;
        GetComponent<Collider>().enabled = enabled;

    }
}
