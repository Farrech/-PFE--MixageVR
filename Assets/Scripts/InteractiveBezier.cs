using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBezier : MonoBehaviour {

    public float currentRadius;
    private BezierSpline bezierSpline;
    float length = 0;
    private Color defaultArcMeshColor;
    private float progress;
    private Material arcMeshMaterial;
    public int duration;
    public GameObject audioGo;

    private ArcMesh arcMesh;
    Mesh mesh;
    public float meshWidth;
    public int resolution = 20;

    void Awake()
    {
        bezierSpline = this.GetComponent<BezierSpline>();
        bezierSpline.Loop = false;
        length = currentRadius *0.552284749f;
        mesh = GetComponent<MeshFilter>().mesh;
        SetBezierSpline();
        MakeArcMesh(CalculateArcArray());
        arcMeshMaterial = bezierSpline.GetComponent<Renderer>().material;
        defaultArcMeshColor = arcMeshMaterial.color;
        duration = 5;
        progress = 0.75f;
        arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
        audioGo.transform.localPosition = bezierSpline.GetPoint(0.75f);
        CalculNewProgress();
    }

    void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
            MakeArcMesh(CalculateArcArray());
    }

    private Vector3 CalculTang(Vector3 point, float length)
    {
        float x, z;
        float pointToLookAtY = 0;
        x = z = 0;

        x = point.x;
        z = point.z;

        float c = point.x * point.x + point.z * point.z;
        if (z != 0)
        {
            pointToLookAtY = (x * length - c) / -z;
            return new Vector3(length, 0, pointToLookAtY);
        }
        else
        {
            pointToLookAtY = (-c / -x);
            return new Vector3(pointToLookAtY, 0, length);
        }
    }

    private void SetBezierSpline()
    {
        Vector3 coord1 = new Vector3(-currentRadius, 0, 0);
        Vector3 coord2 = new Vector3(0, 0, currentRadius);
        Vector3 coord3 = new Vector3(currentRadius, 0, 0);
        bezierSpline.SetControlPoint(0, coord1);
        bezierSpline.SetControlPoint(1, CalculTang(coord1, length));
        bezierSpline.SetControlPoint(3, coord2);
        bezierSpline.SetControlPoint(2, CalculTang(coord2, -length));
        bezierSpline.AddCurve();
        bezierSpline.SetControlPoint(4, CalculTang(coord2, length));
        bezierSpline.SetControlPoint(6, coord3);
        bezierSpline.SetControlPoint(5, CalculTang(coord3, length));
    }

    private void ResetBezierSpline()
    {
        bezierSpline.Reset(); // checker si reset bien
    }
    private float CalculNewProgress()
    {
        var cos = Mathf.Cos(audioGo.transform.localPosition.z / currentRadius);
        Debug.Log("cos = " + cos + " prog = " + Mathf.Lerp(0, Mathf.PI / 2, cos));
        return Mathf.Lerp(0, Mathf.PI , cos);
    }
    private void Update()
    {
        if(Input.anyKey)
            CalculNewProgress();

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("down");
            currentRadius -= 1f * Time.deltaTime;
            length = currentRadius * 0.552284749f;
            ResetBezierSpline();
            SetBezierSpline();
            arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
            MakeArcMesh(CalculateArcArray());
            MoveAudioSource();
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("up");
            currentRadius += 1f * Time.deltaTime;
            length = currentRadius * 0.552284749f;
            ResetBezierSpline();
            SetBezierSpline();
            arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
            MakeArcMesh(CalculateArcArray());
            MoveAudioSource();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            progress += 1f * Time.deltaTime;
            MoveAudioSource();

        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            progress -= 1f * Time.deltaTime;
            MoveAudioSource();
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z+1f);
            currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
            progress = CalculNewProgress();
            Debug.Log("radius = " + currentRadius + " progress= " + progress);
            MakeArcMesh(CalculateArcArray());
            MoveAudioSource();
        }
        else if (Input.GetKey(KeyCode.S))
        {

        }
    }

    public void MoveAudioSource()
    {
        Vector3 calculatePosition = bezierSpline.GetPoint(progress);
        calculatePosition = new Vector3(calculatePosition.x, 0.5f, calculatePosition.z);
        if (progress < 0.01)
        {
            progress = 0.01f;
        }
        else if (progress > 0.99)
        {
            progress = 0.99f;
        }
        audioGo.transform.localPosition = calculatePosition;
        audioGo.transform.LookAt(calculatePosition + bezierSpline.GetDirection(progress));
    }

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];
        for (int i = 0; i <= resolution; i++)
        {
            // Set Vertices
            vertices[i * 2] = new Vector3( arcVerts[i].x, meshWidth * 0.5f, arcVerts[i].y);
            vertices[i * 2 + 1] = new Vector3( arcVerts[i].x, meshWidth * -0.5f, arcVerts[i].y);
           // vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].x, arcVerts[i].y);
           // vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].x, arcVerts[i].y);

            // Set Triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
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
            float t = 1f / (float)resolution * i;
            arcArray[i] = CalculateArcPoint(t);
        }
        return arcArray;
    }

    // calculate height and distance of each vertex
    Vector3 CalculateArcPoint(float t)
    {
        Vector3 position = new Vector3(bezierSpline.GetPoint(t).x, bezierSpline.GetPoint(t).z, bezierSpline.GetPoint(t).y);
        return position;

    }


}
