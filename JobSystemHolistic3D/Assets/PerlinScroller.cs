using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.Jobs;
using  Unity.Jobs;


public class PerlinScroller : MonoBehaviour
{
    int cubeCount;
    public int width = 500;
    public int height = 500;
    public int layer = 3;

    private GameObject[] cubes;
    
    // adding for job system
    private Transform[] cubeTransforms;
    private TransformAccessArray cubeTransformAccessArray;
    private PositionUpdateJob cubeJob;
    private JobHandle cubePositionJobHandle;

    private void Awake()
    {
        cubeCount = (int) (width * height * layer);
        cubes = new GameObject[cubeCount];
        // adding for job system
        cubeTransforms = new Transform[cubeCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        cubes = CreateCubes(cubeCount);
        // adding for job system
        for (int i = 0; i < cubeCount; i++)
        {
            cubeTransforms[i] = cubes[i].transform;
        }
        cubeTransformAccessArray = new TransformAccessArray(cubeTransforms);
    }

    private GameObject[] CreateCubes(int count)
    {
        var cubes = new GameObject[count];
        var cubeToCopy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = cubeToCopy.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        cubeToCopy.GetComponent<Collider>().enabled =false;

        for (int i = 0; i < count; i++)
        {
            var cube = GameObject.Instantiate(cubeToCopy);
            int x = i / (width * layer);
            cube.transform.position = new Vector3(x,0,(i-x*height*layer)/layer);
            cubes[i] = cube;
        }
        GameObject.Destroy(cubeToCopy);
        return cubes;
    }
    
    
    struct PositionUpdateJob: IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
           
        }
    }

    // Update is called once per frame
    private int xoffset;
    void Update()
    {
        //int xoffset = (int) (this.transform.position.x - width / 2.0f);
        xoffset++;
        int zoffset = (int) (this.transform.position.z - height / 2.0f);

        for (int i = 0; i < cubeCount; i++)
        {
            int x = i / (width * layer);
            int z = (i - x * height * layer) / layer;
            int yoffset = i - x * width * layer - z * layer;
            cubes[i].transform.position = new Vector3(x,
                GeneratePerlinHeight(x+xoffset,z+zoffset)+yoffset,
                z+zoffset
                );
        }

        if (Input.GetKey(KeyCode.UpArrow))
            this.transform.Translate(0,0,2);
        else if (Input.GetKey(KeyCode.DownArrow))
        
            this.transform.Translate(0,0,-2);
         else if (Input.GetKey(KeyCode.LeftArrow))
        
            this.transform.Translate(-2,0,0);
        else if (Input.GetKey(KeyCode.RightArrow))
        
            this.transform.Translate(2,0,0);
        
    }

    private float GeneratePerlinHeight(float posx, float posz)
    {
        float smooth = 0.03f;
        float heightMult = 5;
        float height = (Mathf.PerlinNoise(posx * smooth, posz * smooth * 2) * heightMult +
                        Mathf.PerlinNoise(posx * smooth, posz * smooth * 2) * heightMult) / 2.0f;

        return height * 10f;
    }
}
