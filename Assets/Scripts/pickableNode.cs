using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickableNode : MonoBehaviour
{
    public GameObject model;
    public GameObject pickEffect;
    public int id;
    public void UpdateNode(bool enabled)
    {
        if (!enabled)
        {
            Instantiate(pickEffect, transform.position, Quaternion.identity);
        }
        GetComponent<MeshRenderer>().enabled = enabled;
        GetComponent<Collider>().enabled = enabled;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
