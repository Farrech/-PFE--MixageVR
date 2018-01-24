using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePipe : MonoBehaviour {

    public float currentRadius;
    public BezierSpline bezierSpline;
    public float length = 0;
    public float progress;
    public GameObject audioGo;
    private GameObject pipecontainer;
    public Pipe pipe;
    Mesh mesh;
    public bool visible;


    public void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction += MoveTargetAction;
        RightControllerManager.OnTriggerPressAction += SelectTarget;
    }

    public void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction -= MoveTargetAction;
        RightControllerManager.OnTriggerPressAction -= SelectTarget;
    }

    void Awake()
    {
        bezierSpline = this.GetComponent<BezierSpline>();
        bezierSpline.Loop = false;
        mesh = GetComponent<MeshFilter>().mesh;
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

    // TODO : limité les actions dans les dimensions de la salle
    private void MoveTargetAction(GameObject hitGo, Vector3 direction)
    {
        if (hitGo.tag == "AudioSource" && hitGo == this.audioGo)
        {
            if (hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up)
                {
                    currentRadius += 1f * Time.deltaTime;
                }
                else if (direction == Vector3.down && currentRadius >=1)
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
                UpdateCurve(true);
            }
            else if (!hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z + 1f * Time.deltaTime);
                }
                else if (direction == Vector3.down && audioGo.transform.localPosition.z>=0)
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
                UpdateCurve(false);
            }
        }
    }

    private void SelectTarget(GameObject hitGo)
    {
        if (hitGo.GetComponent<AudioSourceSript>().index == audioGo.GetComponent<AudioSourceSript>().index)
        {
            visible = !visible;
            UpdateCurve(visible);
        }
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
        progress = Interpolate(cos);
    }

    public void UpdateCurve(bool move)
    {
        length = currentRadius * 0.552284749f;
        bezierSpline.Reset();
        SetBezierSpline();
        pipe.curveRadius = currentRadius;
        Debug.Log("update, visible = " + visible);
        pipe.UpdatePipe(visible);
        pipe.pipeMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);

        if (move)
            MoveAudioSource();
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


    private float Interpolate(float f)
    {
        float xa = 180f;
        float xb = 0f;
        float ya = 0f;
        float yb = 1f;

        return ya + ((float)f - xa) * (yb - ya) / (xb - xa);
    }
}
