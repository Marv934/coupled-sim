using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class ColliderDrivingMesh : MonoBehaviour
    {

        Vector3[] newVertices;
        Vector2[] newUV;
        int[] newTriangles;

        // Start is called before the first frame update
        void Start()
        {
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            mesh.vertices = newVertices;
            mesh.uv = newUV;
            mesh.triangles = newTriangles;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}