using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBezier : MonoBehaviour
{

    public float currentRadius;
    public BezierSpline bezierSpline;
    public float length = 0;
    private Color defaultArcMeshColor;
    public float progress;
    private Material arcMeshMaterial;
    public int duration;
    public GameObject audioGo;

    private ArcMesh arcMesh;
    Mesh mesh;
    public float meshWidth;
    public int resolution = 20;

    public void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction += MoveTargetAction;
    }

    public void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction -= MoveTargetAction;
    }

    void Awake()
    {
        bezierSpline = this.GetComponent<BezierSpline>();
        bezierSpline.Loop = false;
        mesh = GetComponent<MeshFilter>().mesh;
        //SetBezierSpline();
        //MakeArcMesh(CalculateArcArray());
        arcMeshMaterial = bezierSpline.GetComponent<Renderer>().material;
        defaultArcMeshColor = arcMeshMaterial.color;
        duration = 5;
        //progress = 0.75f;
        arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
        //audioGo.transform.localPosition = bezierSpline.GetPoint(0.75f);
       // audioGo.transform.localPosition = new Vector3(10,0.5f,10);
        //currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        //progress = CalculNewProgress();
        //UpdateBezierSpline(false);
    }

    private void Update()
    {
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    Debug.Log("down");
        //    currentRadius -= 1f * Time.deltaTime;
        //    UpdateBezierSpline(true);
        //}
        //else if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    Debug.Log("up");
        //    currentRadius += 1f * Time.deltaTime;
        //    UpdateBezierSpline(true);
        //}
        //else if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    progress += 1f * Time.deltaTime;
        //    UpdateBezierSpline(true);
        //}
        //else if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    progress -= 1f * Time.deltaTime;
        //    UpdateBezierSpline(true);
        //}
        //else if (Input.GetKey(KeyCode.Z))
        //{
        //    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z + 1f * Time.deltaTime);
        //    currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        //    progress = CalculNewProgress();
        //    UpdateBezierSpline(false);
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z - 1f * Time.deltaTime);
        //    currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        //    progress = CalculNewProgress();
        //    UpdateBezierSpline(false);
        //}
        //else if (Input.GetKey(KeyCode.Q))
        //{
        //    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x - 1f * Time.deltaTime, audioGo.transform.position.y, audioGo.transform.position.z);
        //    currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        //    progress = CalculNewProgress();
        //    UpdateBezierSpline(false);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x + 1f * Time.deltaTime, audioGo.transform.position.y, audioGo.transform.position.z);
        //    currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        //    progress = CalculNewProgress();
        //    UpdateBezierSpline(false);
        //}


    }


    private void MoveTargetAction(GameObject hitGo, Vector3 direction)
    {
        if (hitGo.tag == "AudioSource" && hitGo==this.audioGo)
        {
            if (hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up)
                {
                    currentRadius += 1f * Time.deltaTime;
                }
                else if (direction == Vector3.down)
                {
                    currentRadius -= 1f * Time.deltaTime;
                }
                else if (direction == Vector3.right)
                {
                    progress += 1f * Time.deltaTime;
                }
                else if (direction == Vector3.left)
                {
                    progress -= 1f * Time.deltaTime;
                }
                UpdateBezierSpline(true);
            }
            else if (!hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z + 1f * Time.deltaTime);
                }
                else if (direction == Vector3.down)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z - 1f * Time.deltaTime);
                }
                else if (direction == Vector3.right)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x + 1f * Time.deltaTime, audioGo.transform.position.y, audioGo.transform.position.z);
                }
                else if (direction == Vector3.left)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x - 1f * Time.deltaTime, audioGo.transform.position.y, audioGo.transform.position.z);
                }
                currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
                CalculNewProgress();
                UpdateBezierSpline(false);
            }
        }
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


    public void CalculNewProgress()
    {
        currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
        length = currentRadius * 0.552284749f;
        var cos = Mathf.Acos(audioGo.transform.localPosition.x / currentRadius) * Mathf.Rad2Deg;
        //Debug.Log("calculatePrgress= " + Interpolate(cos) + " - true = " + progress);
        progress= Interpolate(cos);
    }

    public void UpdateBezierSpline(bool move)
    {
        length = currentRadius * 0.552284749f;
        bezierSpline.Reset();
        SetBezierSpline();
        arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
        MakeArcMesh(CalculateArcArray());
        if (move)
            MoveAudioSource();
        // MoveAudioSource();
    }


    public void MoveAudioSource()
    {
        Vector3 calculatePosition = bezierSpline.GetPoint(progress);
        calculatePosition = new Vector3(calculatePosition.x, 0.5f, calculatePosition.z);
        if (progress < 0)
        {
            progress = 0f;
        }
        else if (progress > 1)
        {
            progress = 1f;
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
            vertices[i * 2] = new Vector3(arcVerts[i].x, meshWidth * 0.5f, arcVerts[i].y);
            vertices[i * 2 + 1] = new Vector3(arcVerts[i].x, meshWidth * -0.5f, arcVerts[i].y);
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


    private float Interpolate(float f)
    {
        float xa = 180f;
        float xb = 0f;
        float ya = 0f;
        float yb = 1f;

        return ya + ((float)f - xa) * (yb - ya) / (xb - xa);
    }

}
