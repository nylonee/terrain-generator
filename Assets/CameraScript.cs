using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public float speed = 0.1f;
    public float rotationSpeed = 0.5f;
    public float mouseSpeed = 500f;

    public float sensitivity = 10.0f;

    public float minimumY = -80f;
    public float maximumY = 80f;

    float rotationY = 0F;

    Rigidbody body;
    GameObject _cube;
    TerrainData terrainData;

    // Use this for initialization
    void Start ()
    {
        body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;

        // Get the box boundary
        _cube = GameObject.FindGameObjectWithTag("Box");


        // Get the attached terrain component
        Terrain terrain = (Terrain)(GameObject.FindGameObjectWithTag("Terrain")).GetComponent("Terrain");

        // Get a reference to the terrain data
        terrainData = terrain.terrainData;
        //gameObject.AddComponent<TerrainCollider>().terrainData = terrainData;
    }

    // Update is called once per frame
    void Update()
    {

        _cube = GameObject.FindGameObjectWithTag("Box");

        // Get the attached terrain component
        Terrain terrain = (Terrain)(GameObject.FindGameObjectWithTag("Terrain")).GetComponent("Terrain");


        // Get a reference to the terrain data
        terrainData = terrain.terrainData;
        //gameObject.AddComponent<TerrainCollider>().terrainData = terrainData;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed);
        }
        // XXX:
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
        }

        // Mouse configuration

        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;

        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, transform.localEulerAngles.z);


        // Check that the new position doesn't put the camera outside of the box
        float minX = _cube.transform.position.x - (_cube.transform.localScale.x/2);
        float maxX = _cube.transform.position.x + (_cube.transform.localScale.x/2);
        float minZ = _cube.transform.position.z - (_cube.transform.localScale.z/2);
        float maxZ = _cube.transform.position.z + (_cube.transform.localScale.z/2);

        if(transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        if (transform.position.z < minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }
        if (transform.position.z > maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }

        // Check that the camera is always above the terrain

        // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
        float height = terrainData.GetHeight((int)transform.position.x, (int)transform.position.z);
        if(transform.position.y < height + 0.5f)
        {
            transform.position = new Vector3(transform.position.x, height + 0.5f, transform.position.z);
        }
    }
}
