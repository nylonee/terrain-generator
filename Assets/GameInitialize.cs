using UnityEngine;
using System.Collections;

public class GameInitialize : MonoBehaviour {
    
    public int mapSize = 10;
    public float magnitude = 0.5f;
    public Material waterMaterial;

    public Shader shader;
    public Shader waterShader;
    public PointLight sun;

    float[,] mapArray;
    GameObject _terrain;
    TerrainData _terrainData;
    GameObject _camera;

    // Use this for initialization
    void Start () {

        // Set the random seed to system time
        Random.seed = (int)System.DateTime.Now.Ticks;

        // Generate the terrain using diamond square algorithm
        DiamondSquareAlgorithm();

        // Generate water by creating a plane
        Mesh m = new Mesh();
        m.name = "Water";
        m.vertices = new[] {
            new Vector3(-1.0f, 0f, -1.0f),
            new Vector3(-1.0f, 0f, 1.0f),
            new Vector3(1.0f, 0f, 1.0f),
            new Vector3(-1.0f, 0f, -1.0f),
            new Vector3(1.0f, 0f, 1.0f),
            new Vector3(1.0f, 0f, -1.0f)
        };

        // Vertex normals
        Vector3 topNormal = (new Vector3(0f, 1.0f, 0f));
        m.normals = new[]
        {
            topNormal,
            topNormal,
            topNormal,
            topNormal,
            topNormal,
            topNormal
        };

        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
            triangles[i] = i;
        m.triangles = triangles;
        
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Plane);

        MeshRenderer renderer = water.GetComponent<MeshRenderer>();
        renderer.material.SetFloat("_Mode", 3.0f);
        renderer.material.color = new Color(0.0f, 0.0f, 1.0f, 0.3f);

        MeshFilter waterMesh = water.GetComponent<MeshFilter>();
        waterMesh.mesh = m;

        water.transform.position = new Vector3(_terrain.transform.position.x + (Mathf.Pow(2, mapSize) + 1) / 2, Mathf.Pow(_terrainData.GetHeight((int)transform.position.x, (int)transform.position.z), 0.5f), _terrain.transform.position.z + (Mathf.Pow(2, mapSize) + 1) / 2);
        water.transform.localScale = new Vector3(_terrainData.size.x/2, 0, _terrainData.size.z/2);


        // Set the camera to around the center of the generated terrain
        SetCameraPosition();

        // Create a 'box' around the terrain
        GameObject _boundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _boundary.transform.position = new Vector3(_terrain.transform.position.x + (Mathf.Pow(2, mapSize)+1)/2, _terrain.transform.position.y, _terrain.transform.position.z + (Mathf.Pow(2, mapSize) + 1) / 2);
        _boundary.transform.localScale = new Vector3(_terrainData.size.x, _terrainData.heightmapHeight, _terrainData.size.z);
        _boundary.GetComponent<MeshRenderer>().enabled = false;
        _boundary.tag = "Box";
    }

    void SetCameraPosition()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");

        float x = _terrain.transform.position.x + (Mathf.Pow(2, mapSize) + 1) / 2;
        float z = _terrain.transform.position.z + (Mathf.Pow(2, mapSize) + 1) / 2;
        _camera.transform.position = new Vector3(x, _terrainData.GetHeight((int)transform.position.x, (int)transform.position.z) + 1, z);


    }

    void DiamondSquareAlgorithm()
    {
        // Using 2^n-1 for size will ensure that we'll always be able to find midpoints during the algorithm
        int fullLength = (int)(Mathf.Pow(2, mapSize) + 1.0f);
        // Initialize the map array
        mapArray = new float[fullLength, fullLength];

        // Set the 4 corners to random starting points
        mapArray[0, 0] = Random.value - 0.5f;
        mapArray[0, fullLength - 1] = Random.value - 0.5f;
        mapArray[fullLength - 1, 0] = Random.value - 0.5f;
        mapArray[fullLength - 1, fullLength - 1] = Random.value - 0.5f;
        
        float multiplier = magnitude;

        // Start at the full length of the array, alternating between diamond/square
        // note that for a 2^n+1 size array, we have to iterate n times
        for (int sideLength = fullLength - 1; sideLength >= 2; sideLength /= 2)
        {
            int halfLength = sideLength / 2;

            // Start with square
            for (int x = 0; x < fullLength - 1; x += sideLength)
            {
                for (int y = 0; y < fullLength - 1; y += sideLength)
                {
                    float val = mapArray[x, y];
                    val += mapArray[x + sideLength, y];
                    val += mapArray[x, y + sideLength];
                    val += mapArray[x + sideLength, y + sideLength];

                    val /= 4;

                    // Add a random value to the average
                    val += (Random.value - 0.5f) * multiplier;

                    mapArray[x + halfLength, y + halfLength] = val;
                }
            }

            // Now do diamonds
            for (int x = 0; x < fullLength - 1; x += halfLength)
            {
                for (int y = (x + halfLength) % sideLength; y < fullLength - 1; y += sideLength)
                {
                    float val = mapArray[(x - halfLength + fullLength - 1) % (fullLength - 1), y];
                    val += mapArray[(x + halfLength) % (fullLength - 1), y];
                    val += mapArray[x, (y + halfLength) % (fullLength - 1)];
                    val += mapArray[x, (y - halfLength + fullLength - 1) % (fullLength - 1)];

                    val /= 4;

                    // Add randomness
                    val += (Random.value - 0.5f) * multiplier;

                    mapArray[x, y] = val;

                    // Fringe case - wrap values on the edges
                    if (x == 0) mapArray[fullLength - 1, y] = val;
                    if (y == 0) mapArray[x, fullLength - 1] = val;
                }
            }

            multiplier /= 2;
        }

        float min = 0f;
        float max = 1f;

        for (int i = 0; i < fullLength; i++)
        {
            for (int j = 0; j < fullLength; j++)
            {
                if(mapArray[i, j] < min)
                {
                    min = mapArray[i, j];
                } else if(mapArray[i, j] > max)
                {
                    max = mapArray[i, j];
                }
            }
        }

        for (int i = 0; i < fullLength; i++)
        {
            for (int j = 0; j < fullLength; j++)
            {
                mapArray[i, j] = (mapArray[i, j] - min) / (max - min);
            }
        }

        GenerateTerrain();

    }

    void GenerateTerrain()
    {

        //create terrain data
        _terrainData = new TerrainData();

        _terrainData.heightmapResolution = (int)(Mathf.Pow(2, mapSize) + 1.0f);
        _terrainData.size = new Vector3((int)(Mathf.Pow(2, mapSize) + 1.0f), 20, (int)(Mathf.Pow(2, mapSize) + 1.0f));
        _terrainData.SetHeights(0, 0, mapArray);

        //Create a terrain with the set terrain data
        _terrain = Terrain.CreateTerrainGameObject(_terrainData);
        _terrain.tag = "Terrain";

        SplatPrototype[] terrainTexture = new SplatPrototype[4];
        terrainTexture[0] = new SplatPrototype();
        byte[] fileData = System.IO.File.ReadAllBytes("Assets/terraingrass.jpg");
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        terrainTexture[0].texture = tex;
        terrainTexture[1] = new SplatPrototype();
        fileData = System.IO.File.ReadAllBytes("Assets/sand.jpg");
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        terrainTexture[1].texture = tex;
        terrainTexture[2] = new SplatPrototype();
        fileData = System.IO.File.ReadAllBytes("Assets/flower.jpg");
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        terrainTexture[2].texture = tex;
        terrainTexture[3] = new SplatPrototype();
        fileData = System.IO.File.ReadAllBytes("Assets/snow.jpg");
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        terrainTexture[3].texture = tex;

        _terrainData.splatPrototypes = terrainTexture;

        // Attach a splat component for texture
        _terrain.AddComponent(typeof(AssignSplatMap));

        MeshRenderer renderer = _terrain.AddComponent<MeshRenderer>();
        renderer.material.shader = shader;


        // Generate water for terrain that's below 0.2?


    }

    // Update is called once per frame
    void Update()
    {
        // Pass updated light positions to shader
        MeshRenderer renderer = _terrain.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_PointLightColor", sun.color);
        renderer.material.SetVector("_PointLightPosition", sun.GetWorldPosition());
    }
}
