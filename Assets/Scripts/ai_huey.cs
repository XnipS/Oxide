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
    float heliBoredom = 600;
    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) { return; }
        turret_target = m_turret.transform.rotation;
        StartCoroutine(HeliThink());
        StartCoroutine(TurretThink());
    }

    void Update()
    {
        //Lerp model
        m_turret.transform.rotation = Quaternion.Slerp(m_turret.transform.rotation, turret_target, Time.deltaTime * 10f);
        transform.rotation = Quaternion.LookRotation(new Vector3(target_pos.x, 0, target_pos.z) - new Vector3(transform.position.x, 0, transform.position.z));
        transform.position = Vector3.Lerp(transform.position, target_pos, Time.deltaTime * .1f);
        if (!isServer) { return; }
        if (rocketBurstCooldown > 0)
        {
            rocketBurstCooldown -= Time.deltaTime;
        }
        if(heliBoredom > 0) {
            heliBoredom -= Time.deltaTime;
        }
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
            if (heliBoredom <= 0)
            {
            }
            else
            {
                if (currentTarget == null)
                {
                    heliBoredom -= Time.deltaTime;
                }
                else
                {
                    if (Vector3.Distance(currentTarget.transform.position, transform.position) > 250f)
                    {
                        currentTarget = null;
                    }
                    else
                    {
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
                                        Vector2 rocket_cone = Random.insideUnitCircle * Random.Range(-1f, 1f);
                                        Quaternion rocket_error = Quaternion.Euler(rocket_cone.x, rocket_cone.y, 0);
                                        GameObject x = Instantiate(m_rocket, m_turret.transform.position, turret_target * rocket_error);
                                        NetworkServer.Spawn(x);
                                        FindObjectOfType<effectManager>().CMD_SpawnEffect(9, m_turret.transform.position, turret_target);
                                        yield return new WaitForSeconds(.05f);
                                    }
                                    yield return new WaitForSeconds(3f);
                                }
                                yield return new WaitForSeconds(.1f);
                                Vector2 bullet_cone = Random.insideUnitCircle * Random.Range(-.5f, .5f);
                                Quaternion bullet_error = Quaternion.Euler(bullet_cone.x, bullet_cone.y, 0);
                                GameObject g = Instantiate(m_bullet, m_turret.transform.position, turret_target * bullet_error);
                                NetworkServer.Spawn(g);
                                FindObjectOfType<effectManager>().CMD_SpawnEffect(m_bulletEffect, m_turret.transform.position, turret_target);
                            }
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
            if (heliBoredom > 0)
            {
                //Set waypoint
                waypoint.x = m_mapCentre.x + Random.Range(-m_mapSize.x, m_mapSize.x);
                waypoint.z = m_mapCentre.z + Random.Range(-m_mapSize.z, m_mapSize.z);
                waypoint.y = 0;
                Vector3 startPos = transform.position;
                //Go waypoint
                while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint) > 10f)
                {
                    //While moving attack enemies
                    target_pos = Vector3.Lerp(target_pos, waypoint, Time.deltaTime * m_speed * (1f / Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint)));
                    yield return null;
                }
            }
            else
            {
                //Set waypoint
                waypoint.x = -1000;
                waypoint.z = -1000;
                waypoint.y = 0;
                //Go waypoint
                while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint) > 10f)
                {
                    //While moving attack enemies
                    target_pos = Vector3.Lerp(target_pos, waypoint, Time.deltaTime * m_speed * (1f / Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), waypoint)));
                    yield return null;
                }
                NetworkServer.Destroy(gameObject);
            }
            yield return null;
        }
    }
}
