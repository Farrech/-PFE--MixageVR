using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject textInfoGo;
    public Transform frontWall; 

    public void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction += MoveTargetAction;
        RightControllerManager.OnTriggerPressAction += SelectTarget;
        LeftControllerTelepotation.OnTeleportation += UpdateText;
        RightControllerManager.OnGripPressAction += SelectTarget2;

    }

    public void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction -= MoveTargetAction;
        RightControllerManager.OnTriggerPressAction -= SelectTarget;
        RightControllerManager.OnGripPressAction -= SelectTarget2;
        LeftControllerTelepotation.OnTeleportation -= UpdateText;

    }

    void Awake()
    {
        bezierSpline = this.GetComponent<BezierSpline>();
        bezierSpline.Loop = false;
        mesh = GetComponent<MeshFilter>().mesh;

      }

    private void Update()
    {
        if(Camera.main)
            UpdateText();
    }

    // TODO : limité les actions dans les dimensions de la salle
    private void MoveTargetAction(GameObject hitGo, Vector3 direction)
    {
        if (hitGo.tag == "AudioSource" && hitGo == this.audioGo)
        {
            if (hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up &&  audioGo.transform.position.z < frontWall.transform.position.z - 0.5f 
                    && audioGo.transform.position.x < frontWall.transform.position.z - 0.5f 
                    && audioGo.transform.position.x > -frontWall.transform.position.z + 0.5f)
                {
                    currentRadius += 2f * Time.deltaTime;
                }
                else if (direction == Vector3.down && currentRadius >=1)
                {
                    currentRadius -= 2f * Time.deltaTime;
                }
                else if (direction == Vector3.right && audioGo.transform.position.z < frontWall.transform.position.z-0.5f
                    && audioGo.transform.position.x < frontWall.transform.position.z - 0.5f)
                {
                    progress += 0.5f * Time.deltaTime;
                }
                else if (direction == Vector3.left && audioGo.transform.position.z < frontWall.transform.position.z-0.5f
                    && audioGo.transform.position.x > -frontWall.transform.position.z + 0.5f)
                {
                    progress -= 0.5f * Time.deltaTime;
                } 
                else if (audioGo.transform.position.z >= frontWall.transform.position.z - 0.5f )
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z - 1f * Time.deltaTime);
                    currentRadius = Mathf.Sqrt(audioGo.transform.localPosition.x * audioGo.transform.localPosition.x + audioGo.transform.localPosition.z * audioGo.transform.localPosition.z);
                    CalculNewProgress();
                }

                UpdateCurve(true);
            }
            else if (!hitGo.GetComponent<AudioSourceSript>().anchored)
            {
                if (direction == Vector3.up && audioGo.transform.position.z < frontWall.transform.position.z - 0.5f)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z + 1f * Time.deltaTime);
                }
                else if (direction == Vector3.down && audioGo.transform.localPosition.z>=0)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x, audioGo.transform.position.y, audioGo.transform.position.z - 1f * Time.deltaTime);
                }
                else if (direction == Vector3.right && audioGo.transform.position.x < frontWall.transform.position.z *2  - 0.5f)
                {
                    audioGo.transform.localPosition = new Vector3(audioGo.transform.position.x + 1f * Time.deltaTime, audioGo.transform.position.y, audioGo.transform.position.z);
                }
                else if (direction == Vector3.left && audioGo.transform.position.x > -frontWall.transform.position.z * 2 + 0.5f)
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
        if (hitGo.tag=="AudioSource" && hitGo.GetComponent<AudioSourceSript>().index == audioGo.GetComponent<AudioSourceSript>().index && hitGo.GetComponent<AudioSourceSript>().anchored)
        {
            visible = !visible;
            UpdateCurve(visible);
        }
    }

    private void SelectTarget2(GameObject hitGo)
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
        audioGo.transform.LookAt(Vector3.zero);
        //textInfoGo.transform.localRotation = Quaternion.Euler(textInfoGo.transform.localRotation.x, textInfoGo.transform.localRotation.y - 180f, textInfoGo.transform.localRotation.z);
        //textInfoGo.transform.eulerAngles = new Vector3(textInfoGo.transform.rotation.x, textInfoGo.transform.rotation.y - 180f, textInfoGo.transform.rotation.z);
    }

    private void UpdateText()
    {
        textInfoGo.transform.position = new Vector3(audioGo.transform.position.x, 2, audioGo.transform.position.z);
        
        textInfoGo.transform.LookAt(Camera.main.transform);
        textInfoGo.transform.Rotate(0, -180, 0);
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
