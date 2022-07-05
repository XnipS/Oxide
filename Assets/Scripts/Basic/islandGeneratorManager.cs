using System.Collections.Generic;
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
    [Header("Generation Settings")]
    public float noiseScale;
    public float noiseVerticalScale;
    public float pass1Weight;
    public float pass2Weight;
    public float pass3Weight;
    public float maxHeight_wetbeach;
    public float maxHeight_drybeach;
    public float maxHeight_topstone;
    public float maxHeight_snow;
    [Tooltip("Y scale of height map.")]
    public float yScale = 1f;//y height of heightmap
    [Tooltip("Heightmap resolution.")]
    public HeightmapResolution resolution;
    public GameObject[] foliage;
    public TerrainLayer[] layers;
    public GameObject[] rockNodes;
    public GameObject hempNode;
    public GameObject shroomNode;

    public void NoiseAndSplat(float amt)
    {
        TerrainData data = GetComponent<Terrain>().terrainData;
        float[,] inputData = data.GetHeights(0, 0, islandSize, islandSize);
        float[,] outputData;
        GenerateHeightmap(out outputData);
        for (int x = 0; x < islandSize; x++)
        {
            for (int y = 0; y < islandSize; y++)
            {
                inputData[x, y] += outputData[x, y] * (amt / data.heightmapResolution);
            }
        }

        data.SetHeights(0, 0, inputData);
    }

    void GenerateNoiseOnly(out float[,] raw)
    {
        float[,] rawData = new float[islandSize, islandSize];
        for (int x = 0; x < islandSize; x++)
        {
            for (int y = 0; y < islandSize; y++)
            {
                //First Pass - randomness
                float xCoord = ((float)x / (float)islandSize * (float)noiseScale);//+ (float)transform.position.x;
                float yCoord = ((float)y / (float)islandSize * (float)noiseScale);//+ (float)transform.position.z;
                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                noise *= pass1Weight;
                //Apply
                rawData[x, y] = noise;
            }
        }
        raw = rawData;
    }

    public void GenerateIsland()
    {
        TerrainData data = new TerrainData();
        GetComponent<Terrain>().materialTemplate = terrainMaterial;
        GetComponent<Terrain>().terrainData = data;
        GetComponent<TerrainCollider>().terrainData = data;
        data.heightmapResolution = (int)resolution;
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

    void GenerateHeightmap(out float[,] raw)
    {
        float[,] rawData = new float[islandSize, islandSize];
        for (int x = 0; x < islandSize; x++)
        {
            for (int y = 0; y < islandSize; y++)
            {
                float total = 0;
                //First Pass - randomness
                float xCoord = ((float)x / (float)islandSize * (float)noiseScale);//+ (float)transform.position.x;
                float yCoord = ((float)y / (float)islandSize * (float)noiseScale);//+ (float)transform.position.z;

                total += Unity_Voronoi_float(new Vector2(xCoord * 0.5f, yCoord * 0.5f), 30f, 1f) * pass3Weight;

                //Noise 1
                float noise = Mathf.PerlinNoise(xCoord * 0.5f, yCoord * 0.5f);
                noise *= (1 / yScale) * (pass1Weight * 1f);
                total += noise;
                //Noise 2
                noise = Mathf.PerlinNoise(xCoord * 1, yCoord * 1);
                noise *= (1 / yScale) * (pass1Weight * .5f);
                total -= noise;
                //Noise 3
                noise = Mathf.PerlinNoise(xCoord * 10, yCoord * 10);
                noise *= (1 / yScale) * (pass1Weight * .3f);
                total += noise;
                //Noise 3
                noise = Mathf.PerlinNoise(xCoord * 20, yCoord * 20);
                noise *= (1 / yScale) * (pass1Weight * .05f);
                total += noise;

                //Second Pass - island formation
                //float centreDistance = Vector2.Distance(new Vector2(x, y), new Vector2(islandSize / 2, islandSize / 2));
                float centreDistance = Vector2.Distance(new Vector2(x, y), new Vector2(islandSize, islandSize));
                centreDistance *= 1.25f;
                centreDistance /= islandSize;
                centreDistance = (centreDistance * -1) + 1;
                centreDistance *= pass2Weight;
                total += centreDistance;

                //Apply
                rawData[x, y] = total;
            }
        }
        raw = rawData;
    }

    void AddTerrainLayers(TerrainData data)
    {
        data.treeInstances = new TreeInstance[0];
        data.terrainLayers = layers;
        List<TreePrototype> trees = new List<TreePrototype>();
        foreach (GameObject tree in foliage)
        {
            var x = new TreePrototype();
            x.prefab = tree;
            trees.Add(x);
        }
        data.treePrototypes = trees.ToArray();
        data.RefreshPrototypes();
    }

    public void InspectorGenerateSplat()
    {
        float[,,] splatData;
        TerrainData data = GetComponent<Terrain>().terrainData;
        AddTerrainLayers(data);
        GenerateSplatmap(out splatData, data);
        data.SetAlphamaps(0, 0, splatData);
        GenerateTerrainStuffs();
    }

    float GetTerrainNoise(float scale, float x, float y)
    {
        float xCoord = ((float)x / (float)islandSize * (float)scale);
        float yCoord = ((float)y / (float)islandSize * (float)scale);
        float noise = Mathf.PerlinNoise(xCoord, yCoord);
        return noise;
    }

    void SpawnTree(float x, float y, int type, bool normalSnap = false)
    {
        TerrainData data = GetComponent<Terrain>().terrainData;
        TreeInstance tree = new TreeInstance();
        float xx = (1.0f / data.alphamapResolution) * y;
        float yy = (1.0f / data.alphamapResolution) * x;
        float zz = (1.0f / data.heightmapResolution) * data.GetInterpolatedHeight(x, y);
        tree.position = new Vector3(xx, zz, yy);
        tree.prototypeIndex = type;
        tree.widthScale = 1;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
       // tree.rotation = 0;
        tree.heightScale = 1;
        //data.treePrototypes[type].prefab.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        tree.rotation = Random.Range(0,2 * Mathf.PI);
        GetComponent<Terrain>().AddTreeInstance(tree);
    }

    public void GenerateGrass(float x, float y, int splat)
    {
        TerrainData data = GetComponent<Terrain>().terrainData;
        if (splat == 3)
        {
            //Grass
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.98f)
            {
                //Bush
                SpawnTree(x, y, 3);
            }
            else
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.7f)
            {

                //Shrub
                SpawnTree(x, y, 4);
            }
            else if (Random.Range(0f, 1f) > 0.5f)
            {
                //Grass_1
                SpawnTree(x, y, 0);
            }
        }
        if (splat == 1)
        {
            //Dirt
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.98f)
            {
                //Bush_1
                SpawnTree(x, y, 8);
            }
            else
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.98f)
            {
                //Bush_1
                SpawnTree(x, y, 2);
            }
            else
            if (GetTerrainNoise(125, x, y) > 0.50f && Random.Range(0f, 1f) > 0.7f)
            {

                //Shrub_2
                SpawnTree(x, y, 5);
            }
            else
            if (GetTerrainNoise(90, x, y) > 0.50f && Random.Range(0f, 1f) > 0.7f)
            {

                //Shrub_3
                SpawnTree(x, y, 6);
            }
            else
            if (GetTerrainNoise(75, x, y) > 0.50f && Random.Range(0f, 1f) > 0.7f)
            {

                //Shrub_4
                SpawnTree(x, y, 7);
            }
            else if (Random.Range(0f, 1f) > 0.5f)
            {
                //Grass_2
                SpawnTree(x, y, 1);
            }
        }
        if (splat == 2)
        {
            //Stone Rocks
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.98f)
            {
                //Bush_1
                SpawnTree(x, y, 14);
            }
            else
            if (GetTerrainNoise(100, x, y) > 0.50f && Random.Range(0f, 1f) > 0.98f)
            {
                //Bush_1
                SpawnTree(x, y, 13);
            }
            else
            if (GetTerrainNoise(125, x, y) > 0.50f && Random.Range(0f, 1f) > 0.99f)
            {

                //Shrub_2
                SpawnTree(x, y, 12);
            }
            else
            if (GetTerrainNoise(90, x, y) > 0.50f && Random.Range(0f, 1f) > 0.99f)
            {

                //Shrub_3
                SpawnTree(x, y, 11);
            }
            else
            if (GetTerrainNoise(75, x, y) > 0.50f && Random.Range(0f, 1f) > 0.99f)
            {

                //Shrub_4
                SpawnTree(x, y, 10);
            }
            else if (Random.Range(0f, 1f) > 0.99f)
            {
                //Grass_2
                SpawnTree(x, y, 9);
            }
        }
       GetComponent<Terrain>().Flush();
    }

    void SpawnHemp(float x, float y, TerrainData data, int surface)
    {
        if (Random.Range(0f, 1f) > 0.999f)
        {
            float xx = transform.position.z + y;
            float yy = transform.position.x + x;
            float zz = data.GetHeight((int)y, (int)x) + transform.position.y;//data.GetHeight((int)(x * data.alphamapResolution), (int)(y * data.alphamapResolution));
            GameObject g = Instantiate(hempNode, new Vector3(xx, zz, yy), Quaternion.Euler(0, Random.Range(0, 360), 0));
            g.transform.up = data.GetInterpolatedNormal(y * (1f / data.heightmapResolution), x * (1f / data.heightmapResolution));
            g.transform.SetParent(this.transform);
        }
    }

    void SpawnShroom(float x, float y, TerrainData data, int surface)
    {
        if (Random.Range(0f, 1f) > 0.997f)
        {
            float xx = transform.position.z + y;
            float yy = transform.position.x + x;
            float zz = data.GetHeight((int)y, (int)x) + transform.position.y;//data.GetHeight((int)(x * data.alphamapResolution), (int)(y * data.alphamapResolution));
            GameObject g = Instantiate(shroomNode, new Vector3(xx, zz, yy), Quaternion.Euler(0, Random.Range(0, 360), 0));
            g.transform.up = data.GetInterpolatedNormal(y * (1f / data.heightmapResolution), x * (1f / data.heightmapResolution));
            g.transform.SetParent(this.transform);
        }
    }

    void SpawnHarvestableRock(float x, float y, TerrainData data, int surface)
    {
        //Debug.Log(data.heightmapScale.y);
        if (surface == 2) //stone
        {
            if (Random.Range(0f, 1f) > 0.999f)
            {

                float xx = transform.position.z + y;
                float yy = transform.position.x + x;
                float zz = data.GetHeight((int)y, (int)x) + transform.position.y;//data.GetHeight((int)(x * data.alphamapResolution), (int)(y * data.alphamapResolution));
                GameObject g = Instantiate(rockNodes[Random.Range(0, 3)], new Vector3(xx, zz, yy), Quaternion.Euler(0, Random.Range(0, 360), 0));
                g.transform.up = data.GetInterpolatedNormal(y * (1f / data.heightmapResolution), x * (1f / data.heightmapResolution));
                g.transform.SetParent(this.transform);
            }
        }
        if (surface == 1) //dirt
        {
            if (Random.Range(0f, 1f) > 0.999f)
            {

                float xx = transform.position.z + y;
                float yy = transform.position.x + x;
                float zz = data.GetHeight((int)y, (int)x) + transform.position.y;//data.GetHeight((int)(x * data.alphamapResolution), (int)(y * data.alphamapResolution));
                GameObject g = Instantiate(rockNodes[Random.Range(0, 3)], new Vector3(xx, zz, yy), Quaternion.Euler(0, Random.Range(0, 360), 0));
                g.transform.up = data.GetInterpolatedNormal(y * (1f / data.heightmapResolution), x * (1f / data.heightmapResolution));
                g.transform.SetParent(this.transform);
            }
        }
    }

    public void GenerateTerrainStuffs()
    {
        TerrainData terrainData = GetComponent<Terrain>().terrainData;
        for (int i = this.transform.childCount; i > 0; --i)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
        terrainData.SetTreeInstances(new TreeInstance[0], true);
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));
                float[,,] aMap = terrainData.GetAlphamaps(y, x, 1, 1);
                if (aMap[0, 0, 6] > 0)
                {
                    //Debug.Log("ESS");
                }
                else
                {
                    if (height > maxHeight_wetbeach + GetTerrainNoise(100, x, y))
                    {
                        if (height > maxHeight_drybeach + GetTerrainNoise(100, x, y))
                        {
                            if (height > maxHeight_topstone + GetTerrainNoise(100, x, y))
                            {
                                if (height > maxHeight_snow + GetTerrainNoise(100, x, y))
                                {
                                    //snow
                                }
                                else
                                {
                                    //	stone
                                    SpawnHarvestableRock(x, y, terrainData, 2);
                                    GenerateGrass(x, y, 2);
                                }
                            }
                            else
                            {
                                if (GetTerrainNoise(20, x, y) > 0.7f)
                                {
                                    //stone
                                    SpawnHarvestableRock(x, y, terrainData, 2);
                                    GenerateGrass(x, y, 2);
                                }
                                else
                                {
                                    //grass
                                    if (GetTerrainNoise(15, x, y) > 0.4f)
                                    {
                                        //grass
                                        if (steepness < 25f)
                                        {
                                            GenerateGrass(x, y, 3);
                                            SpawnHemp(x, y, terrainData, 3);
                                        }
                                    }
                                    else
                                    {
                                        //dirt
                                        if (steepness < 25f)
                                        {
                                            GenerateGrass(x, y, 1);
                                        }
                                        SpawnShroom(x, y, terrainData, 3);
                                        SpawnHarvestableRock(x, y, terrainData, 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //drysand
                        }
                    }
                    else
                    {
                        //wetsand
                    }

                    if (steepness > 25f)
                    {
                        //stone
                        SpawnHarvestableRock(x, y, terrainData, 2);
                        GenerateGrass(x, y, 2);
                    }
                }
            }
        }
    }

    void GenerateSplatmap(out float[,,] raw, TerrainData terrainData)
    {
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
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

                splatWeights[0] = 0f;//sand
                splatWeights[1] = 0f;//dirt
                splatWeights[2] = 0f;//stone
                splatWeights[3] = 0f;//grass
                splatWeights[4] = 0f;//snow
                splatWeights[5] = 0f;//wetsand

                if (height > maxHeight_wetbeach + GetTerrainNoise(100, x, y))
                {
                    if (height > maxHeight_drybeach + GetTerrainNoise(100, x, y))
                    {
                        if (height > maxHeight_topstone + GetTerrainNoise(100, x, y))
                        {
                            if (height > maxHeight_snow + GetTerrainNoise(100, x, y))
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
                        }
                        else
                        {
                            //grass
                            if (GetTerrainNoise(20, x, y) > 0.7f)
                            {
                                //stone
                                splatWeights[0] = 0f;//sand
                                splatWeights[1] = 0f;//dirt
                                splatWeights[2] = 0f;//stone
                                splatWeights[3] = 0f;//grass
                                splatWeights[4] = 0f;//snow
                                splatWeights[5] = 0f;//wetsand
                                splatWeights[2] = 1f;
                            }
                            else
                            {
                                if (GetTerrainNoise(15, x, y) > 0.4f)
                                {
                                    //grass
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
                        }
                    }
                    else
                    {
                        //drysand
                        splatWeights[0] = 1f;//sand
                        splatWeights[1] = 0f;//dirt
                        splatWeights[2] = 0f;//stone
                        splatWeights[3] = 0f;//grass
                        splatWeights[4] = 0f;//snow
                        splatWeights[5] = 0f;//wetsand
                    }
                }
                else
                {
                    //wetsand
                    splatWeights[0] = 0f;//sand
                    splatWeights[1] = 0f;//dirt
                    splatWeights[2] = 0f;//stone
                    splatWeights[3] = 0f;//grass
                    splatWeights[4] = 0f;//snow
                    splatWeights[5] = 1f;//wetsand
                }

                if (steepness > 25f)
                {
                    //stone
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
                for (int i = 0; i < terrainData.alphamapLayers; i++)
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
    }
    float Unity_Voronoi_float(Vector2 UV, float AngleOffset, float CellDensity)
    {
        float Out = 0;
        // Cells = 0;
        Vector2 g = new Vector2(Mathf.Floor(UV.x * CellDensity), Mathf.Floor(UV.y * CellDensity));
        Vector2 f = new Vector2((UV.x * CellDensity) % 1f, (UV.y * CellDensity) % 1f);
        float t = 8.0f;
        Vector3 res = new Vector3(8.0f, 0.0f, 0.0f);

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2 lattice = new Vector2(x, y);
                Vector2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                float d = Vector2.Distance(lattice + offset, f);
                if (d < res.x)
                {
                    res = new Vector3(d, offset.x, offset.y);
                    Out = res.x;
                    // Cells = res.y;
                }
            }
        }
        return Out;
    }
    Vector2 unity_voronoi_noise_randomVector(Vector2 UV, float offset)
    {
        Vector4 m = new Vector4(15.27f, 47.63f, 99.41f, 89.98f);
        Vector4 x = new Vector4(UV.x, UV.y, 0, 0);
        //UV = (Mathf.Sin(Vector4.Dot (x, m)) * 46839.32) % 1f;
        return new Vector2(Mathf.Sin(UV.y * +offset) * 0.5f + 0.5f, Mathf.Cos(UV.x * offset) * 0.5f + 0.5f);
    }
}

[CustomEditor(typeof(islandGeneratorManager))]
public class islandGeneratorEditor : Editor
{
    string input_id = "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        islandGeneratorManager man = (islandGeneratorManager)target;
        if (GUILayout.Button("Apply Foliage ONLY"))
        {
            man.GenerateTerrainStuffs();

        }
        if (GUILayout.Button("Apply Splat and Foliage"))
        {
            man.InspectorGenerateSplat();

        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Noise Scale To Apply");
        input_id = EditorGUILayout.TextField("", input_id);
        EditorGUILayout.Separator();
        if (GUILayout.Button("Apply Noise"))
        {

            man.NoiseAndSplat(float.Parse(input_id));
            //man.GenerateFoliage();
            //man.GenerateGrass(man.islandSize, man.islandSize);
        }
        if (GUILayout.Button("Generate New Island"))
        {
            man.GenerateIsland();
        }
    }


}

public enum HeightmapResolution
{
    //33 65 129 257 513 1025 2049 4097
    _33x33 = 33, _65x65 = 65, _129x129 = 129, _257x257 = 257, _513x513 = 513, _1025x1025 = 1025, _2049x2049 = 2049, _4097x4097 = 4097
}
#endif