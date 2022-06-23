using UnityEngine;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
public class islandGeneratorManager : MonoBehaviour
{
	[Header("Island Settings")]
	[Tooltip("Island real size in metres.")]
	public int islandSize;
	[Tooltip("The terrain material.")]
	public Material terrainMaterial;
	//public float islandHeightmapResolutionScale = 1.0f;
	[Header("Generation Settings")]
	public float noiseScale;
	public float noiseVerticalScale;
	public float pass1Weight;
	public float pass2Weight;
	[Tooltip("Y scale of height map.")]
	public float yScale = 1f;//y height of heightmap
	[Tooltip("Heightmap resolution.")]
	public HeightmapResolution resolution;
	public GameObject[] foliage;
	public TerrainLayer[] layers;

	public void GenerateIsland ()
	{
		TerrainData data = new TerrainData();
		GetComponent<Terrain>().materialTemplate = terrainMaterial;
		GetComponent<Terrain>().terrainData = data;
		GetComponent<TerrainCollider>().terrainData = data;
		data.heightmapResolution =(int)resolution;
		data.alphamapResolution = (int)resolution;
		float[,] rawData;
		float[,,] splatData;
		data.size = new Vector3(islandSize, yScale, islandSize);
		AddTerrainLayers(data);
		GenerateHeightmap(out rawData);		
		data.SetHeights(0, 0, rawData);

		GenerateSplatmap(out splatData, data);
		data.SetAlphamaps(0, 0, splatData);
	}

	void GenerateHeightmap (out float[,] raw)
	{
		float[,] rawData = new float[islandSize,islandSize];
		for(int x = 0; x < islandSize; x++)
		{
			for(int y = 0; y < islandSize; y++)
			{
				//First Pass - randomness
				float xCoord = ((float)x / (float)islandSize * (float)noiseScale);//+ (float)transform.position.x;
				float yCoord = ((float)y / (float)islandSize * (float)noiseScale);//+ (float)transform.position.z;
				float noise = Mathf.PerlinNoise(xCoord, yCoord);
				noise *= pass1Weight;

				//Second Pass - island formation
				float centreDistance = Vector2.Distance(new Vector2(x, y), new Vector2(islandSize / 2, islandSize / 2));
				centreDistance /= islandSize;						
				centreDistance = (centreDistance * -1)+1;
				centreDistance *= pass2Weight;
				noise += centreDistance;	
				
				//Apply
				rawData[x, y] = noise;
			}
		}
		raw = rawData;
	}

	void AddTerrainLayers (TerrainData data)
	{
		data.terrainLayers = layers;
	}

	public void InspectorGenerateSplat ()
	{
		float[,,] splatData;
		TerrainData data = GetComponent<Terrain>().terrainData;
		GenerateSplatmap(out splatData, data);
		data.SetAlphamaps(0, 0, splatData);
	}

	float GetTerrainNoise (float scale, float x, float y)
	{
		float xCoord = ((float)x / (float)islandSize * (float)scale);//+ (float)transform.position.x;
		float yCoord = ((float)y / (float)islandSize * (float)scale);//+ (float)transform.position.z;
		float noise = Mathf.PerlinNoise(xCoord, yCoord);
		return noise;
	}

	//public void GenerateFoliage ()
	//{
	//	TerrainData data = GetComponent<Terrain>().terrainData;


	//	List<TreeInstance> trees = new List<TreeInstance>();

	//		for(int y = 0; y < data.alphamapHeight; y++)
	//		{
	//			for(int x = 0; x < data.alphamapWidth; x++)
	//			{
	//			if(GetTerrainNoise(50, x, y) > 0.3f)
	//			{
					
	//			}
	//			}

	//		}
	//	//data.SetTreeInstances(trees.ToArray(), true);
		
	//}

	void GenerateGrass (float x, float y)
	{
		TerrainData data = GetComponent<Terrain>().terrainData;
		if(GetTerrainNoise(25, x, y) > 0.75f)
		{
			if(Random.Range(0f, 1f) > 0.5f)

			{
				TreeInstance tree = new TreeInstance();
				float xx = (1.0f / data.alphamapResolution) * x;
				float yy = (1.0f / data.alphamapResolution) * y;
				tree.position = new Vector3(xx, (1.0f / data.heightmapResolution) * data.GetInterpolatedHeight(x, y), yy);
				tree.prototypeIndex = 0;
				tree.widthScale = 1;
				tree.heightScale = 1;
				data.treePrototypes[0].prefab.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
				//Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
				//tree.prototypeIndex = 0;
				//inst.position = new Vector3(x, height, y);
				tree.rotation = Random.Range(0, 360);
				//trees.Add(tree);
				GetComponent<Terrain>().AddTreeInstance(tree);
				//insts.Add(inst);
			}
		}
		GetComponent<Terrain>().Flush();
	}

	void GenerateSplatmap(out float[,,] raw, TerrainData terrainData)
	{
		float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

		for(int y = 0; y < terrainData.alphamapHeight; y++)
		{
			for(int x = 0; x < terrainData.alphamapWidth; x++)
			{
				// Normalise x/y coordinates to range 0-1 
				float y_01 = (float)y / (float)terrainData.alphamapHeight;
				float x_01 = (float)x / (float)terrainData.alphamapWidth;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));

				// Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
				Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

				// Calculate the steepness of the terrain
				float steepness = terrainData.GetSteepness(y_01, x_01);

				// Setup an array to record the mix of texture weights at this point
				float[] splatWeights = new float[terrainData.alphamapLayers];

				// CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

				///////////////////////////////////////////////////////////////////////// Examples
				// Texture[0] has constant influence
				////splatWeights[0] = 0.5f;

				// Texture[1] is stronger at lower altitudes
				////splatWeights[1] = Mathf.Clamp01((terrainData.heightmapResolution - height));

				// Texture[2] stronger on flatter terrain
				// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
				// Subtract result from 1.0 to give greater weighting to flat surfaces
				////splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapResolution / 5.0f));

				// Texture[3] increases with height but only on surfaces facing positive Z axis 
				////splatWeights[3] = height * Mathf.Clamp01(normal.z);
				/////////////////////////////////////////////////////////////////////////

				splatWeights[0] = 0f;//sand
				splatWeights[1] = 0f;//dirt
				splatWeights[2] = 0f;//stone
				splatWeights[3] = 0f;//grass
				splatWeights[4] = 0f;//snow
				splatWeights[5] = 0f;//wetsand
									 //Mathf.Clamp01((terrainData.heightmapResolution - height));

				
				//0Sand = base layer
				//splatWeights[0] = 0.5f;
				//1Dirt = from level 80 - max
				//Debug.Log(height);
				//splatWeights[1] = Mathf.Clamp01((terrainData.heightmapResolution - height));

				if(height > 76f + GetTerrainNoise(100, x, y))
				{
					if(height > 80f + GetTerrainNoise(100, x, y))
					{
						if(height >120f + GetTerrainNoise(100, x, y))
						{
							if(height > 125f + GetTerrainNoise(100, x, y))
							{
								//snow
								splatWeights[0] = 0f;//sand
								splatWeights[1] = 0f;//dirt
								splatWeights[2] = 0f;//stone
								splatWeights[3] = 0f;//grass
								splatWeights[4] = 1f;//snow
								splatWeights[5] = 0f;//wetsand
							}
							else
							{
								//	stone
								splatWeights[0] = 0f;//sand
								splatWeights[1] = 0f;//dirt
								splatWeights[2] = 1f;//stone
								splatWeights[3] = 0f;//grass
								splatWeights[4] = 0f;//snow
								splatWeights[5] = 0f;//wetsand
							}
						}else
						{
							//grass
							
							if(GetTerrainNoise(50, x, y) > 0.3f)
							{
								//grass
								//GenerateGrass(x, y);
								splatWeights[0] = 0f;//sand
								splatWeights[1] = 0f;//dirt
								splatWeights[2] = 0f;//stone
								splatWeights[3] = 1f;//grass
								splatWeights[4] = 0f;//snow
								splatWeights[5] = 0f;//wetsand
							}
							else
							{
								//dirt
								splatWeights[0] = 0f;//sand
								splatWeights[1] = 1f;//dirt
								splatWeights[2] = 0f;//stone
								splatWeights[3] = 0f;//grass
								splatWeights[4] = 0f;//snow
								splatWeights[5] = 0f;//wetsand
							}
						}
					}else
					{
						//drysand
						splatWeights[0] = 1f;//sand
						splatWeights[1] = 0f;//dirt
						splatWeights[2] = 0f;//stone
						splatWeights[3] = 0f;//grass
						splatWeights[4] = 0f;//snow
						splatWeights[5] = 0f;//wetsand
					}
				}else
				{
					//wetsand
					splatWeights[0] = 0f;//sand
					splatWeights[1] = 0f;//dirt
					splatWeights[2] = 0f;//stone
					splatWeights[3] = 0f;//grass
					splatWeights[4] = 0f;//snow
					splatWeights[5] = 1f;//wetsand
				}

				if(steepness > 25f)
				{
					splatWeights[0] = 0f;//sand
					splatWeights[1] = 0f;//dirt
					splatWeights[2] = 0f;//stone
					splatWeights[3] = 0f;//grass
					splatWeights[4] = 0f;//snow
					splatWeights[5] = 0f;//wetsand
					splatWeights[2] = 1f;
				}

				//2Stone = only on slopes

				////3Grass = only on flat areas
				//splatWeights[3] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
				float z = splatWeights.Sum();

				// Loop through each terrain texture
				for(int i = 0; i < terrainData.alphamapLayers; i++)
				{

					// Normalize so that sum of all texture weights = 1
					splatWeights[i] /= z;

					// Assign this point to the splatmap array
					splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}

		// Finally assign the new splatmap to the terrainData:
		raw = splatmapData;
		//terrainData.SetAlphamaps(0, 0, splatmapData);
	}
}

[CustomEditor(typeof(islandGeneratorManager))]
public class islandGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		islandGeneratorManager man = (islandGeneratorManager)target;
		if(GUILayout.Button("Apply Alpha"))
		{
			man.InspectorGenerateSplat();

		}
		
		if(GUILayout.Button("Apply Foliage"))
		{
			//man.GenerateFoliage();
		}
		if(GUILayout.Button("Generate Island from scratch"))
		{
			//man.InspectorGenerateSplat();
			man.GenerateIsland();
		}
	}
}

public enum HeightmapResolution
{
	//33 65 129 257 513 1025 2049 4097
	_33x33 = 33, _65x65 = 65, _129x129 = 129,_257x257 = 257, _513x513 = 513, _1025x1025 = 1025, _2049x2049 = 2049, _4097x4097 = 4097
}
#endif