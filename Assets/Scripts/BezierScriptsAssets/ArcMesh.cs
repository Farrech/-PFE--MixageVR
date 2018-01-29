using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspiré du tutoriel https://www.youtube.com/watch?v=TXHK1nPUOBE&t=194s

[RequireComponent(typeof(MeshFilter))]
public class ArcMesh: MonoBehaviour
{
    Mesh mesh;
    public float meshWidth;
    public BezierSpline bez;

    public int resolution = 20;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        MakeArcMesh(CalculateArcArray());
    }
    
    void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
            MakeArcMesh(CalculateArcArray());
    }

    void MakeArcMesh(Vector3 [] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 *2];
        for (int i = 0; i <=resolution; i++)
        {
            // Set Vertices
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);

            // Set Triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 +6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    // Create an array of Vector3 positions for arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float t = 1f/(float)resolution *i;
            arcArray[i] = CalculateArcPoint(t);
        }
        return arcArray;
    }

    // calculate height and distance of each vertex
    Vector3 CalculateArcPoint(float t)
    {
        Vector3 position = new Vector3(bez.GetPoint(t).x, bez.GetPoint(t).z, bez.GetPoint(t).y) ;
        return position;

    }
}
