using UnityEngine;

public class footstepSounds : MonoBehaviour
{
	public AudioClip[] sand;
	public AudioClip[] dirt;
	public AudioClip[] grass;
	public AudioClip[] forest;
	public AudioClip[] concrete;
	public AudioClip[] snow;
	public AudioClip[] metal;

	public void ANIM_Footstep()
	{
		Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

		if(Physics.Raycast(ray, out RaycastHit hit, 1.15f))
		{
			if(hit.collider.GetComponent<Terrain>() != null)
			{
				Step(GetTerrainTexture(hit.collider.GetComponent<Terrain>()));
			}
			else
			{
				if(hit.collider.sharedMaterial == null)
				{
					GetComponent<reloadAudio>().PlayRandomReloadAudio(concrete);
				}
				else
				{
					if(hit.collider.sharedMaterial.name == "Concrete")
					{
						Step(4);
					}
					if(hit.collider.sharedMaterial.name == "Dirt")
					{
						Step(1);
					}
				}
			}
		}
	}

	void Step(int index)
	{
		// GameObject gam = Instantiate(surfaces[index].stepEffect, transform.position, transform.rotation);
		// gam.transform.forward = Vector3.up;
		switch(index)
		{
			case 0:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(sand);
			break;
			case 1:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(dirt);
			break;
			case 2:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(grass);
			break;
			case 3:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(forest);
			break;
			case 4:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(concrete);
			break;
			case 5:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(snow);
			break;
			case 6:
			GetComponent<reloadAudio>().PlayRandomReloadAudio(metal);
			break;
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
		if(aMap[0, 0, 5] > 0)
		{
			return 5;

		}
		else
		{
			if(aMap[0, 0, 4] > 0)
			{
				return 4;

			}
			else
			{
				if(aMap[0, 0, 3] > 0)
				{
					return 3;
				}
				else
				{
					if(aMap[0, 0, 2] > 0)
					{
						return 2;
					}
					else
					{
						if(aMap[0, 0, 1] > 0)
						{
							return 1;
						}
						else
						{
							if(aMap[0, 0, 0] > 0)
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