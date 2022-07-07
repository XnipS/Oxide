using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ai_huey : NetworkBehaviour
{
    public GameObject m_bullet;
    public GameObject m_rocket;
    public int m_bulletEffect;
    public GameObject m_turret;
    [SyncVar]
    Quaternion turret_target;
    [SyncVar]
    Vector3 target_pos;
    [SyncVar]
    Quaternion target_rot;
    public LayerMask m_floorMask;
    public Vector3 m_mapCentre;
    public Vector3 m_mapSize;
    float floorLevel = 0;
    public float m_speed;
    NetworkIdentity currentTarget = null;
    Vector3 waypoint = Vector3.zero;
    float rocketBurstCooldown;
    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) { return; }
        StartCoroutine(HeliThink());
        StartCoroutine(TurretThink());
    }

    void Update()
    {
        m_turret.transform.rotation = Quaternion.Slerp(m_turret.transform.rotation, turret_target, Time.deltaTime * 10f);
        transform.rotation = Quaternion.LookRotation(new Vector3(target_pos.x, 0, target_pos.z) - new Vector3(transform.position.x, 0, transform.position.z));
        transform.position = Vector3.Lerp(transform.position, target_pos, Time.deltaTime * .1f);
        if (!isServer) { return; }
        //Lerp model

        //m_heliModel.transform.rotation = Quaternion.Slerp(m_heliModel.transform.rotation, target_rot, Time.deltaTime * .1f);
        //Calculate floor
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000f, m_floorMask))
        {
            floorLevel = hit.point.y + 50f;
        }
        //Calculate pos
        target_pos.y = floorLevel;
    }

    public void TakeDamageHook(NetworkIdentity damager)
    {
        if (damager == null) { Debug.Log("[AI] Unkown damager networkIdentity"); return; }
        currentTarget = damager;
        waypoint.x = currentTarget.transform.position.x;
        waypoint.z = currentTarget.transform.position.z;
        waypoint.y = 0;
    }

    IEnumerator TurretThink()
    {

        while (true)
        {
            if (currentTarget == null)
            {

            }
            else
            {
                if (Vector3.Distance(currentTarget.transform.position, transform.position) > 250f)
                {
                    currentTarget = null;
                }
                else
                {
                    if (rocketBurstCooldown > 0)
                    {
                        rocketBurstCooldown -= Time.deltaTime;
                    }
                    turret_target = Quaternion.LookRotation((currentTarget.transform.position + Vector3.up) - m_turret.transform.position);
                    if (Physics.Raycast(m_turret.transform.position + m_turret.transform.forward, turret_target * Vector3.forward, out RaycastHit hit, 1000f))
                    {
                        if (hit.collider.GetComponent<NetworkIdentity>() && hit.collider.GetComponent<NetworkIdentity>() == currentTarget)
                        {
                            if (rocketBurstCooldown <= 0)
                            {
                                rocketBurstCooldown = 10f;
                                for (int i = 0; i < 6; i++)
                                {
                                    GameObject x = Instantiate(m_rocket, m_turret.transform.position, turret_target);
                                    NetworkServer.Spawn(x);
                                    FindObjectOfType<effectManager>().CMD_SpawnEffect(9, m_turret.transform.position, turret_target);
                                    yield return new WaitForSeconds(.25f);
                                }
                            }
                            yield return new WaitForSeconds(.1f);
                            GameObject g = Instantiate(m_bullet, m_turret.transform.position, turret_target);
                            NetworkServer.Spawn(g);
                            FindObjectOfType<effectManager>().CMD_SpawnEffect(m_bulletEffect, m_turret.transform.position, turret_target);
                        }
                    }

                }
            }
            yield return null;
        }
    }

    IEnumerator HeliThink()
    {
        while (true)
        {
            //Set waypoint
            waypoint.x = m_mapCentre.x + Random.Range(-m_mapSize.x, m_mapSize.x);
            waypoint.z = m_mapCentre.z + Random.Range(-m_mapSize.z, m_mapSize.z);
            waypoint.y = 0;
            Vector3 startPos = transform.position;
            //Go waypoint
            while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint) > 5f)
            {
                //While moving attack enemies
                target_pos = Vector3.Lerp(target_pos, waypoint, Time.deltaTime * m_speed * (1f / Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint)));
                yield return null;
            }
            yield return null;
        }
    }
}
