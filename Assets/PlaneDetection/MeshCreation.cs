using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreation : MonoBehaviour
{
    [SerializeField] Transform[] newVerticeObjs;
    [SerializeField] Vector3[] newVertices;
    int[] newTriangles;

    void Start()
    {
        newVertices = new Vector3[4];
        for(int i = 0; i < newVerticeObjs.Length; i++)
        {
            newVertices[i] = newVerticeObjs[i].transform.position;
        }

        Vector2[] uv = new[] {
            // generate uv for corresponding vertices also in form of square
    
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1),
        };

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = uv;
        mesh.triangles = newTriangles;
    }
}
