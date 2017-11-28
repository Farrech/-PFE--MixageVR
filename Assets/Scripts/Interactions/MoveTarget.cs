using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour {

    public int duration;
    public GameObject bezierSplineContainer;
    public GameObject arcMeshContainer;
    public MixerController masterMixer;

    private Color defaultArcMeshColor;
    private float progress;
    private BezierSpline bezierSpline;
    private ArcMesh arcMesh;
    private int currentSpline;
    private Material arcMeshMaterial;


    public void OnEnable()
    {
        RightControllerManager.OnTriggerTouchAction += MoveTargetAction;
    }

    public void OnDisable()
    {
        RightControllerManager.OnTriggerTouchAction -= MoveTargetAction;
    }

    public void Start()
    {
        bezierSpline = bezierSplineContainer.transform.GetChild(currentSpline).GetComponent<BezierSpline>();
        arcMesh = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<ArcMesh>();
        arcMeshMaterial = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<Renderer>().material;
        defaultArcMeshColor = arcMeshMaterial.color;
        this.transform.localPosition = bezierSpline.GetPoint(0.5f);
        duration = 5;
        progress = 0.5f;
    }

 

    private void MoveTargetAction(GameObject hitGo)
    {
        if (hitGo.tag == "AudioSource")
        {
            RaycastHit hit =this.GetComponentInParent<LaserPointer>().hit;
            var forward = this.transform.position - hitGo.transform.position;
            var direction = this.transform.position - hit.point;
            var leftOrRight = CrossProduct(forward, direction, Vector3.up);
            if (leftOrRight == 1)
            {
                progress += 1f * Time.deltaTime;
               // print("moving right");
            }
            else if (leftOrRight == -1)
            {
             //   print("moving left");
                progress -= 1f* Time.deltaTime;
            }


            arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
            MoveAudioSource(hitGo);
            hitGo.GetComponent<AudioSource>().panStereo = Interpolate(progress);
        }
    }

    public void MoveAudioSource(GameObject hitGo)
    {
        Vector3 calculatePosition = bezierSpline.GetPoint(progress);
        calculatePosition = new Vector3(calculatePosition.x, 0.5f, calculatePosition.z);
        hitGo.transform.localPosition = calculatePosition;
        hitGo.transform.LookAt(calculatePosition + bezierSpline.GetDirection(progress));
        if (progress < 0.01)
        {
            progress = 0.01f;
        }
        else if (progress > 0.99)
        {
            progress = 0.99f;
        }
    }

    int CrossProduct(Vector3 forward, Vector3 direction, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(forward, direction);
        float dir = Vector3.Dot(perp, up);
        if (dir > 0.8)
        {
            return 1;
        }
        else if (dir < -0.8f)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    private void ChangeSpline()
    {
        arcMeshMaterial.color = defaultArcMeshColor;
        bezierSpline = bezierSplineContainer.transform.GetChild(currentSpline).GetComponent<BezierSpline>();
        arcMesh = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<ArcMesh>();
        arcMeshMaterial = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<Renderer>().material;
    }

    private float Interpolate(float f)
    {
        float xa = 0.01f;
        float xb = 0.99f;
        float ya = -1f;
        float yb = 1f;

        return ya + ((float)f - xa) * (yb - ya) / (xb - xa);
    }
}
