using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class buildingManager : NetworkBehaviour
{
    public enum buildingType : int
    {
        stick, wood, stone, metal, reinforced
    }
    public enum buildingShape : int
    {
        foundation, ceiling, wall, doorway, stairs, tri_ceiling, tri_foundation, window
    }
    [System.Serializable]
    public class building
    {
        public int myId;
        public Vector3 position;
        public Quaternion rotation;
        public float health;
        public float maxHealth;
        public float integridy;
        public buildingType myType;
        public buildingShape myShape;
    }
    [System.Serializable]
    public class buildingPrefabs
    {
        public GameObject[] prefab;
    }
     [System.Serializable]
    public class BuildingPrefab {
        public GameObject prefab;
        public int id;
    }
    public buildingPrefabs[] prefabs;
    public buildingPrefabs prefabGhost;
    public List<BuildingPrefab> buildingPrefabsInWorld = new List<BuildingPrefab>();
    public List<building> buildings = new List<building>();
    GameObject currentGhost;
    int currentGhostIndex = 0;
    buildingShape currentShape;
    public LayerMask mask;
    public bool buildingPlanOut;
    //Building health update
    [Command(requiresAuthority = false)]
    public void CMD_BuildingDamage(float damage, int id)
    {
        building b = (building)buildings.First(x => x.myId == id);
        b.health -= damage;
        if (b.health <= 0)
        {
            buildings.Remove(b);
            RPC_BuildingDestroy(id);
        }
        else
        {
            RPC_BuildingDamage(id, b);
        }
    }
    //Building destroy update
    [ClientRpc]
    void RPC_BuildingDestroy(int id)
    {
        building b = (building)buildings.First(x => x.myId == id);
        buildings.Remove(b);
        Destroy(buildingPrefabsInWorld.First(x=>x.id == id).prefab);
        buildingPrefabsInWorld.Remove(buildingPrefabsInWorld.First(x=>x.id == id));
    }
    //Building health update
    [ClientRpc]
    void RPC_BuildingDamage(int id, building newBuilding)
    {
        building b = (building)buildings.First(x => x.myId == id);
        b = newBuilding;
        GameObject prefab = buildingPrefabsInWorld.First(x=>x.id == id).prefab;
        prefab.GetComponent<buildingObject>().myBuildingDontUse = newBuilding;

    }
    //Building list update
    [Command(requiresAuthority = false)]
    public void CMD_ConstructBuilding(building newBuilding)
    {
        int id = buildings.Count;
        newBuilding.myId = id;
        buildings.Add(newBuilding);
        RPC_ConstructBuilding(newBuilding);
    }
    //Building list update
    [ClientRpc]
    void RPC_ConstructBuilding(building newBuilding)
    {
        //Add building to memory
        buildings.Add(newBuilding);
        //Grab correct shape
        GameObject b = prefabs[(int)newBuilding.myType].prefab[(int)newBuilding.myShape];
        //Spawn shape
        GameObject i = Instantiate(b, newBuilding.position, newBuilding.rotation);
        //Assign id
        i.GetComponent<buildingObject>().myBuildingDontUse = newBuilding;
        //New Building prefab
        BuildingPrefab newp = new BuildingPrefab();
        newp.id = newBuilding.myId;
        newp.prefab = i;
        //Add to dictionary
        buildingPrefabsInWorld.Add(newp);
    }

    public void CancelGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
        }
        currentGhost = null;
    }
    public void SpawnGhost(buildingShape shape)
    {
        CancelGhost();
        currentGhost = Instantiate(prefabGhost.prefab[(int)shape]);
        currentShape = shape;
    }

    void Start()
    {
        SpawnGhost(buildingShape.foundation);
    }


    void Update()
    {
        //Controlls
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentGhostIndex++;
            if (currentGhostIndex > prefabGhost.prefab.Length - 1)
            {
                currentGhostIndex = 0;
            }
            SpawnGhost((buildingShape)currentGhostIndex);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentGhostIndex--;
            if (currentGhostIndex < 0)
            {
                currentGhostIndex = prefabGhost.prefab.Length - 1;
            }
            SpawnGhost((buildingShape)currentGhostIndex);
        }
        //Check if can build
        if (!buildingPlanOut) { currentGhost.SetActive(false); return; }
        //Ghost control
        if (currentGhost != null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3f, mask))
            {
                //Show ghost
                currentGhost.SetActive(true);
                bool can = false;
                int cost = 0;
                //Ghost placement
                switch (currentShape)
                {
                    case buildingShape.foundation:
                        cost = 50;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && hit.collider.GetComponent<buildingSnapPoint>().id == 0)
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position + snap.transform.forward * 1.5f;
                            currentGhost.transform.rotation = snap.transform.rotation;
                        }
                        else
                        {
                            currentGhost.transform.position = hit.point + Vector3.up * 0.5f;
                            currentGhost.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                        }
                        can = true;
                        break;
                    case buildingShape.tri_foundation:
                        cost = 25;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && hit.collider.GetComponent<buildingSnapPoint>().id == 0)
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position + snap.transform.forward * 1.5f;
                            currentGhost.transform.rotation = snap.transform.rotation;
                        }
                        else
                        {
                            currentGhost.transform.position = hit.point + Vector3.up * 0.5f;
                            currentGhost.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                        }
                        can = true;
                        break;
                    case buildingShape.wall:
                        cost = 50;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && (hit.collider.GetComponent<buildingSnapPoint>().id == 0 || hit.collider.GetComponent<buildingSnapPoint>().id == 1))
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position;
                            currentGhost.transform.rotation = snap.transform.rotation;
                            can = true;
                        }
                        break;
                    case buildingShape.doorway:
                        cost = 35;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && (hit.collider.GetComponent<buildingSnapPoint>().id == 0 || hit.collider.GetComponent<buildingSnapPoint>().id == 1))
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position;
                            currentGhost.transform.rotation = snap.transform.rotation;
                            can = true;
                        }
                        break;
                    case buildingShape.window:
                        cost = 35;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && (hit.collider.GetComponent<buildingSnapPoint>().id == 0 || hit.collider.GetComponent<buildingSnapPoint>().id == 1))
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position;
                            currentGhost.transform.rotation = snap.transform.rotation;
                            can = true;
                        }
                        break;
                    case buildingShape.stairs:
                        cost = 50;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && (hit.collider.GetComponent<buildingSnapPoint>().id == 2))
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position;
                            currentGhost.transform.rotation = Quaternion.Euler(snap.transform.rotation.x, Mathf.Round(snap.transform.rotation.y + Camera.main.transform.rotation.eulerAngles.y / 90f) * 90f, snap.transform.rotation.z);
                            can = true;
                        }
                        break;
                    case buildingShape.ceiling:
                        cost = 25;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && hit.collider.GetComponent<buildingSnapPoint>().id == 1)
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position + snap.transform.forward * 1.5f;
                            currentGhost.transform.rotation = snap.transform.rotation;
                            can = true;
                        }
                        break;
                    case buildingShape.tri_ceiling:
                        cost = 13;
                        if (hit.collider.GetComponent<buildingSnapPoint>() && hit.collider.GetComponent<buildingSnapPoint>().id == 1)
                        {
                            Transform snap = hit.collider.GetComponent<buildingSnapPoint>().transform;
                            currentGhost.transform.position = snap.transform.position + snap.transform.forward * 1.5f;
                            currentGhost.transform.rotation = snap.transform.rotation;

                            can = true;
                        }
                        break;
                }
                if (can)
                {
                    SetAllMaterialColours(currentGhost, Color.blue);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0) && can)
                {
                    //Check if has enough
                    if (FindObjectOfType<ui_inventory>().HasEnough(4, cost))
                    {
                        FindObjectOfType<ui_inventory>().DestroyItem(4, cost);
                        FindObjectOfType<ui_notifyManager>().Notify("-" + cost + " " + itemDictionary.singleton.GetDataFromItemID(4).title, ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.minus);
                        building b = new building();
                        b.myShape = currentShape;
                        b.myType = buildingType.stick;
                        b.position = currentGhost.transform.position;
                        b.rotation = currentGhost.transform.rotation;
                        b.health = 50;
                        b.maxHealth = 50;
                        FindObjectOfType<effectManager>().CMD_SpawnEffect(11, currentGhost.transform.position, currentGhost.transform.rotation);
                        CMD_ConstructBuilding(b);
                    }
                    else
                    {
                        FindObjectOfType<ui_notifyManager>().Notify("Can't afford!", ui_notification.NotifyColourType.red, ui_notification.NotifyIconType.none);
                    }
                }
            }
            else
            {
                currentGhost.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3f;
                SetAllMaterialColours(currentGhost, Color.red);
            }
        }
    }
    static void SetAllMaterialColours(GameObject obj, Color color)
    {
        foreach (MeshRenderer ren in obj.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material mat in ren.materials)
            {
                mat.color = color;
            }
        }
    }
}
