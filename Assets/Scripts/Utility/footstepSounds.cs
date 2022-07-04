using UnityEngine;

public class footstepSounds : MonoBehaviour
{
    [System.Serializable]
    public class footstepMaterial
    {
        public string name;
        public AudioClip[] hard;
        public AudioClip[] soft;
        public GameObject effect;
    }
    public footstepMaterial[] footsteps;
    /*
	0=Sand
	1=Dirt
	2=Grass
	3=Forest
	4=Concrete
	5=Snow
	6=Metal
	*/
    //sand
    //dirt
    //stone
    //grass
    //snow
    //wetsand
    public void ANIM_FootstepHard()
    {
        Footstep(true);
    }
    public void ANIM_Footstep()
    {
        Footstep(false);
    }

    void Footstep(bool hard)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.15f))
        {
            if (hit.collider.GetComponent<Terrain>() != null)
            {
                Step(GetTerrainTexture(hit.collider.GetComponent<Terrain>()), hard, hit.point);
            }
            else
            {
                if (hit.collider.sharedMaterial == null)
                {
                    Step(4, hard, hit.point);
                }
                else
                {
                    if (hit.collider.sharedMaterial.name == "Concrete")
                    {
                        Step(4, hard, hit.point);
                    }
                    if (hit.collider.sharedMaterial.name == "Dirt")
                    {
                        Step(1, hard, hit.point);
                    }
                }
            }
        }
    }

    void Step(int index, bool hard, Vector3 pos)
    {
        GameObject gam = Instantiate(footsteps[index].effect, pos, Quaternion.identity);
        gam.transform.up = Vector3.up;
        if (hard)
        {
            GetComponent<reloadAudio>().PlayRandomReloadAudio(footsteps[index].hard);
        }
        else
        {
            GetComponent<reloadAudio>().PlayRandomReloadAudio(footsteps[index].soft);
        }
    }

    int GetTerrainTexture(Terrain ter)
    {
        Vector2 lpos = ConvertPosition(transform.position, ter);
        return CheckTexture(ter, lpos);
    }

    Vector2 ConvertPosition(Vector3 playerPosition, Terrain t)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / t.terrainData.size.x, 0,
        terrainPosition.z / t.terrainData.size.z);

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        Vector2 vec = Vector2.zero;
        vec.x = xCoord;
        vec.y = zCoord;
        return vec;
    }

    int CheckTexture(Terrain ter, Vector2 convertedPos)
    {
        float[,,] aMap = ter.terrainData.GetAlphamaps((int)convertedPos.x, (int)convertedPos.y, 1, 1);
        if (aMap[0, 0, 6] > 0)
        {
            return 4;

        }
        else
        {
            if (aMap[0, 0, 5] > 0)
            {
                return 0;

            }
            else
            {
                if (aMap[0, 0, 4] > 0)
                {
                    return 5;

                }
                else
                {
                    if (aMap[0, 0, 3] > 0)
                    {
                        return 2;
                    }
                    else
                    {
                        if (aMap[0, 0, 2] > 0)
                        {
                            return 4;
                        }
                        else
                        {
                            if (aMap[0, 0, 1] > 0)
                            {
                                return 1;
                            }
                            else
                            {
                                if (aMap[0, 0, 0] > 0)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}