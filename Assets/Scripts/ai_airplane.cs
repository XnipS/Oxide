using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ai_airplane : MonoBehaviour
{
    public float m_speed;
    public GameObject m_lootPrefab;
    Vector3 dropSpot;
    bool dropped = false;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<NetworkIdentity>().isServer) {
            dropSpot = new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500));
            Vector2 calc = Random.insideUnitCircle * 1500f;
            Vector3 spawnPos = new Vector3(calc.x, 250f, calc.y);
            spawnPos += dropSpot;
            transform.position = spawnPos;
            transform.rotation = Quaternion.LookRotation(new Vector3(dropSpot.x, 250f, dropSpot.z) - transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<NetworkIdentity>().isServer) {
            transform.position += transform.forward * Time.deltaTime * m_speed;
            if(Vector3.Distance(transform.position, new Vector3(dropSpot.x,250f,dropSpot.z)) < 5f && !dropped) {
                GameObject g = Instantiate(m_lootPrefab, transform.position, Quaternion.identity);
                NetworkServer.Spawn(g);
                dropped = true;
            }
            if(Vector3.Distance(transform.position, new Vector3(dropSpot.x,250f,dropSpot.z)) > 1500f && dropped) {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
