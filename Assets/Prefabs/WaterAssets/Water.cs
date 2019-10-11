// 2017 Contrail

using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour
{

    //Our renderer that'll make the top of the water visible
    LineRenderer Body;

    //Our physics arrays
    float[] xpositions;
    float[] zpositions;

    //Our meshes and colliders
    GameObject[] meshobjects;
    Mesh[] meshes;

    //The material we're using for the top of the water
    public Material mat;

    //The GameObject we're using for a mesh
    public GameObject watermesh;

    //Where to draw waves
    //Vector3 TopPos;
    public float length;

    //The properties of our water
    float bottom;
    public float initialHeight;

    public int edgecount = 10;

    public float Amplitude = 1.0f;
    public float enchanceFreq = 1.0f;
    public float delay = 0.0f;
    public float bubblesEnableMin = 0.5f;

    [System.NonSerialized]
    public static float height = 0.0f;

    float waterMaxHeight;
    float waterSurfPos = 0.0f;

    public GameObject ArrowButtonRight;
    public GameObject ArrowButtonDown;
    public GameObject[] waterBubbles;

    void Start()
    {
        height = 0f;
        waterMaxHeight = ArrowButtonRight.transform.lossyScale.z;
        //Spawning our water
        SpawnWater();
        //Disable the water bubbles
        for (int i = 0; i < 2; i++)
        {
            ParticleSystem.EmissionModule em = waterBubbles[i].GetComponent<ParticleSystem>().emission;
            em.enabled = false;
        }
    }

    public void SpawnWater()
    {

        //Calculating the number of edges and nodes we have
        int nodecount = edgecount + 1;

        //Add our line renderer and set it up:
        Body = gameObject.AddComponent<LineRenderer>();
        Body.material = mat;
        Body.positionCount = nodecount;
        Body.startWidth = 0.1f;
        Body.endWidth = 0.1f;
        Body.useWorldSpace = true;

        //Declare our physics arrays
        xpositions = new float[nodecount];
        zpositions = new float[nodecount];

        //Declare our mesh arrays
        meshobjects = new GameObject[edgecount];
        meshes = new Mesh[edgecount];

        //Set our variables
        Transform dwnButtonTrans = ArrowButtonDown.transform;
        bottom = dwnButtonTrans.position.z + dwnButtonTrans.lossyScale.z / 2;
        waterSurfPos = bottom;

        //Use waterWave position as first water surface position
        Vector3 TopPos = gameObject.transform.position;

        //For each node, set the line renderer and our physics arrays
        for (int i = 0; i < nodecount; i++)
        {
            //Wave surface height pos
            zpositions[i] = bottom + initialHeight;
            //Wave surface nodes' x pos
            xpositions[i] = Mathf.Lerp(TopPos.x - length / 2, TopPos.x + length / 2, i / (float)edgecount);
            Body.SetPosition(i, new Vector3(xpositions[i], TopPos.y, zpositions[i]));
        }

        //Setting the meshes now:
        for (int i = 0; i < edgecount; i++)
        {
            //Make the mesh
            meshes[i] = new Mesh();

            //Create the corners of the mesh
            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(xpositions[i], TopPos.y, zpositions[i]);
            Vertices[1] = new Vector3(xpositions[i + 1], TopPos.y, zpositions[i + 1]);
            Vertices[2] = new Vector3(xpositions[i], TopPos.y, bottom);
            Vertices[3] = new Vector3(xpositions[i + 1], TopPos.y, bottom);

            //Set the UVs of the texture
            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            //Set where the triangles should be.
            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            //Add all this data to the mesh.
            meshes[i].vertices = Vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            //Create a holder for the mesh, set it to be the manager's child
            meshobjects[i] = Instantiate(watermesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshobjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshobjects[i].transform.parent = transform;

        }
    }

    //Same as the code from in the meshes before, set the new mesh positions
    void UpdateMeshes()
    {
        float yPos = gameObject.transform.localPosition.y;
        for (int i = 0; i < meshes.Length; i++)
        {
            float fixedZPos_0 = transform.InverseTransformPoint(new Vector3(0, 0, zpositions[i])).z;
            float fixedZPos_1 = transform.InverseTransformPoint(new Vector3(0, 0, zpositions[i + 1])).z;
            float fixedZPos_Bottom = transform.InverseTransformPoint(new Vector3(0, 0, bottom)).z;
            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(xpositions[i], yPos, fixedZPos_0);
            Vertices[1] = new Vector3(xpositions[i + 1], yPos, fixedZPos_1);
            Vertices[2] = new Vector3(xpositions[i], yPos, fixedZPos_Bottom);
            Vertices[3] = new Vector3(xpositions[i + 1], yPos, fixedZPos_Bottom);

            meshes[i].vertices = Vertices;
        }
    }

    //old water height stored for the use of not lacking performance
    float _waterHeight = 0.0f;
    void FixedUpdate()
    {
        float waterHeight = (float)((StoreScore.accomplishedPercent / 100.0f) * waterMaxHeight);
        if (waterHeight > _waterHeight)
        {
            _waterHeight = waterHeight;
            waterSurfPos = bottom + waterHeight;

            for (int i = 0; i < 2; i++)
            {
                Vector3 _bubblePos = waterBubbles[i].transform.position;
                _bubblePos.z = waterSurfPos - 3.5f;
                waterBubbles[i].transform.position = _bubblePos;
            }
        }

        for (int i = 0; i < edgecount + 1; i++)
        {
            zpositions[i] = Amplitude * Mathf.Sin(enchanceFreq * (xpositions[i] + Time.realtimeSinceStartup + (1 / Mathf.PI) * delay)) + waterSurfPos;
            Body.SetPosition(i, new Vector3(xpositions[i], gameObject.transform.position.y, zpositions[i]));
        }

        UpdateBubbles(waterHeight);

        //Finally we update the meshes to reflect this
        UpdateMeshes();
    }

    void UpdateBubbles(float _presentWaterHeight)
    {
        //when bubbles are to be shown
        if (_presentWaterHeight > bubblesEnableMin)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject bubbleObj = waterBubbles[i];
                ParticleSystem.EmissionModule em = bubbleObj.GetComponent<ParticleSystem>().emission;
                em.enabled = true;
            }
        }
    }


}
