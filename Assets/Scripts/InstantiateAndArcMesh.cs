using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAndArcMesh : MonoBehaviour
{

    public float radius;
    public float meshWidth;
    public int resolution;
    public int duration;
    public MixerController masterMixer;
    public GameObject audioSource;

    private BezierSpline bez;
    private Mesh mesh;
    private Material arcMeshMaterial;
    private Color defaultArcMeshColor;
    private float progress;



    void Awake()
    {
        bez = this.GetComponentInChildren<BezierSpline>();
        SetBezierSpline();
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        MakeArcMesh(CalculateArcArray());
        arcMeshMaterial = GetComponentInChildren<Renderer>().material;
        defaultArcMeshColor = arcMeshMaterial.color;
        duration = 5;
        progress = 0.5f;
        audioSource.transform.localPosition = bez.GetPoint(0.5f);
    }



    void Update()
    {
        if (Input.anyKey && !Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKey(KeyCode.RightArrow) && progress >= 0)
            {
                progress += 0.1f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && progress <= 1)
            {
                progress -= 0.1f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                radius += 0.1f;
                masterMixer.SetMusicVol(masterMixer.volume + 0.1f);
                mesh.Clear();
                ResetBezierSpline();
                MakeArcMesh(CalculateArcArray());
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                radius -= 0.1f;
                masterMixer.SetMusicVol(masterMixer.volume - 0.1f);
                mesh.Clear();
                ResetBezierSpline();
                MakeArcMesh(CalculateArcArray());
            }
            arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
            MoveAudioSource();
            audioSource.GetComponent<AudioSource>().panStereo = Interpolate(progress);
        }
    }




    public void MoveAudioSource()
    {
        Vector3 calculatePosition = bez.GetPoint(progress);
        audioSource.transform.localPosition = calculatePosition;
        audioSource.transform.LookAt(calculatePosition + bez.GetDirection(progress));
        if (progress < 0.01)
        {
            progress = 0.01f;
        }
        else if (progress > 0.99)
        {
            progress = 0.99f;
        }
    }

    private float Interpolate(float f)
    {
        float xa = 0.01f;
        float xb = 0.99f;
        float ya = -1f;
        float yb = 1f;

        return ya + ((float)f - xa) * (yb - ya) / (xb - xa);
    }

    void SetBezierSpline()
    {
        float length = radius * 0.552284749f;
        bez.Loop = false;
        Vector3 coord1 = new Vector3(-radius, 0, 0);
        Vector3 coord2 = new Vector3(0, 0, radius);
        Vector3 coord3 = new Vector3(radius, 0, 0);
        bez.SetControlPoint(0, coord1);
        bez.SetControlPoint(1, TangenteOfPoint(coord1, length));
        bez.SetControlPoint(3, coord2);
        bez.SetControlPoint(2, TangenteOfPoint(coord2, -length));
        bez.AddCurve();
        bez.SetControlPoint(4, TangenteOfPoint(coord2, length));
        bez.SetControlPoint(6, coord3);
        bez.SetControlPoint(5, TangenteOfPoint(coord3, length));
    }

    void ResetBezierSpline()
    {
        bez.Reset();
        SetBezierSpline();
    }

    private Vector3 TangenteOfPoint(Vector3 point, float length)
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

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];
        for (int i = 0; i <= resolution; i++)
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
        Vector3 position = new Vector3(bez.GetPoint(t).x, bez.GetPoint(t).z, bez.GetPoint(t).y);
        return position;

    }
}