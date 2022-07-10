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
    public int hitEffect = 3;
    [SyncVar]
    Vector3 currentPos;
    [SyncVar]
    Vector3 currentDir;
    float selfDestruct = 60f;
    bool isDead = false;
    void Start()
    {
        currentPos = transform.position;
        currentDir = transform.forward;
    }
    void FixedUpdate()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            if (!isDead)
            {
                if (selfDestruct > 0)
                {
                    selfDestruct -= Time.fixedDeltaTime;
                }
                else
                {
                    NetworkServer.Destroy(gameObject);
                }
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
    }

    void Update()
    {
        model.transform.position = Vector3.Lerp(model.transform.position, currentPos, Time.deltaTime * 10f);
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(currentDir), Time.deltaTime * 10f);
    }

    IEnumerator DestroyDelayed()
    {
        yield return new WaitForSeconds(timePerStep);
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    void TestForImpact()
    {
        if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, distance, mask))
        {
            isDead = true;
            if (hit.collider.GetComponent<playerHitbox>())
            {
                HitPlayer(hit.collider.GetComponent<playerHitbox>().multiplier, hit.collider.GetComponent<playerHitbox>().id, hit.collider.GetComponentInParent<NetworkIdentity>(), hit.point);
            }
            else if (hit.collider.GetComponent<objectHealth>())
            {
                HitPlayer(0.1f, "", hit.collider.GetComponentInParent<NetworkIdentity>(), hit.point);
            }
            if (hit.collider.GetComponent<npcHealth>())
            {
                HitPlayer(1f, "", hit.collider.GetComponentInParent<NetworkIdentity>(), hit.point);
            }
            else
            {
                FindObjectOfType<effectManager>().CMD_SpawnEffect(hitEffect, hit.point, Quaternion.identity);
            }
            currentPos = hit.point;
            StartCoroutine(DestroyDelayed());
        }
    }
    [Server]
    void HitPlayer(float multiplier, string str, NetworkIdentity target, Vector3 pos)
    {
        if (target != owner)
        {
            if (target.GetComponentInParent<playerHealth>())
            {
                target.GetComponentInParent<playerHealth>().CMD_TakeDamage(damage * multiplier, owner);
                FindObjectOfType<effectManager>().CMD_SpawnEffect(2, pos, Quaternion.identity);
                FindObjectOfType<effectManager>().CMD_SpawnEffect(hitEffect, pos, Quaternion.identity);
            }
            if (target.GetComponentInParent<objectHealth>())
            {
                target.GetComponentInParent<objectHealth>().CMD_TakeDamage(damage * multiplier, owner);
                FindObjectOfType<effectManager>().CMD_SpawnEffect(hitEffect, pos, Quaternion.identity);
            }
            if (target.GetComponentInParent<npcHealth>())
            {
                target.GetComponentInParent<npcHealth>().CMD_TakeDamage(damage * multiplier, owner);
                FindObjectOfType<effectManager>().CMD_SpawnEffect(hitEffect, pos, Quaternion.identity);
            }
        }
    }
}
