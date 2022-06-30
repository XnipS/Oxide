using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class projectile : NetworkBehaviour
{
    public float damage;
    public float timePerStep;
    public float distance;
    public float dropAngle;
    float cooldown;
    public NetworkIdentity owner;
    public GameObject model;
    public LayerMask mask;
    [SyncVar]
    Vector3 currentPos;
    [SyncVar]
    Vector3 currentDir;
    void Start()
    {

        currentPos = transform.position;
        currentDir = transform.forward;

    }
    void FixedUpdate()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            if (cooldown > 0)
            {
                cooldown -= Time.fixedDeltaTime;
            }
            else
            {
                cooldown = timePerStep;
                TestForImpact();
                currentPos += currentDir * distance;
                currentDir = Quaternion.AngleAxis(dropAngle, Vector3.Cross(currentDir, Vector3.up)) * currentDir;


            }
        }
    }

    void Update()
    {
        model.transform.position = Vector3.Lerp(model.transform.position, currentPos, Time.deltaTime * 10f);
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(currentDir), Time.deltaTime * 10f);
    }



    [Server]
    void TestForImpact()
    {
        if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, distance, mask))
        {
            if (hit.collider.GetComponent<playerHitbox>())
            {
                HitPlayer(hit.collider.GetComponent<playerHitbox>().multiplier, hit.collider.GetComponent<playerHitbox>().id, hit.collider.GetComponentInParent<NetworkIdentity>(), hit.point);
            }
            else
            {
                FindObjectOfType<effectManager>().CMD_SpawnEffect(3, hit.point, Quaternion.identity);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
    [Server]
    void HitPlayer(float multiplier, string str, NetworkIdentity target, Vector3 pos)
    {
        if (target != owner)
        {
            target.GetComponentInParent<playerHealth>().CMD_TakeDamage(damage * multiplier);
            FindObjectOfType<effectManager>().CMD_SpawnEffect(2, model.transform.position, Quaternion.identity);
            NetworkServer.Destroy(gameObject);
        }
    }

}
