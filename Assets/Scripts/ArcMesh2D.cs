using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArcMesh2D : MonoBehaviour
{

    LineRenderer lr;

    public float velocity;
    public float angle;
    public int resolution = 10;

    float gravity;
    float radianAngle;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        gravity = Mathf.Abs(Physics2D.gravity.y); // gravité vers le haut pris en compte seulement
    }
    // Use this for initialization
    void Start()
    {
        RenderArc();
    }
    void OnValidate()
    {
        if (lr != null && Application.isPlaying)
            RenderArc();
    }

    // populating th lr with the appropriate settinfs
    void RenderArc()
    {
        lr.SetVertexCount(resolution + 1);
        lr.SetPositions(CalculateArcArray());
    }

    // Create an array of Vector3 positions for arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;
        for (int i = 0; i <=resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t,maxDistance);
        }
        return arcArray;
    }

    // calculate height and distance of each vertex
    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float z= x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        float y = 0;
        return new Vector3(x, y,z);
    }
}
