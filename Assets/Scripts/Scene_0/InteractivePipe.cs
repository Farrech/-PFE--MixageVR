using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractivePipe : MonoBehaviour {

    public float currentRadius;
    public BezierSpline bezierSpline;
    public float length = 0;
    public float progress; // correspond à la valeur du panoramique
    public GameObject audioSource; // référence à la source sonore
    private GameObject pipecontainer;
    public Pipe pipe; // représentation de la courbe de bézier
    Mesh mesh;
    public bool visible; // la courbe de bézier doit-elle être visible ?
    public GameObject textInfoGo; // texte relié à l'audioSource
    public Transform frontWall; 

    // abonnement aux évenements
    public void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction += MoveTargetAction;
        RightControllerManager.OnTriggerPressAction += SelectTarget;
        LeftControllerManager.OnTeleportation += UpdateText;
        RightControllerManager.OnGripPressAction += SelectTarget2;

    }

    public void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction -= MoveTargetAction;
        RightControllerManager.OnTriggerPressAction -= SelectTarget;
        RightControllerManager.OnGripPressAction -= SelectTarget2;
        LeftControllerManager.OnTeleportation -= UpdateText;

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
    // déplacement d'une source sonore ou d'un listener
    private void MoveTargetAction(GameObject hitGo, Vector3 direction)
    {
        if (hitGo.tag == "AudioSource" && hitGo == this.audioSource) // si c'est une audioSource
        {
            if (hitGo.GetComponent<AudioSourceSript>().anchored) // si la source est ancrée à la courbe
            {
                if (direction == Vector3.up &&  audioSource.transform.position.z < frontWall.transform.position.z - 0.5f 
                    && audioSource.transform.position.x < frontWall.transform.position.z - 0.5f 
                    && audioSource.transform.position.x > -frontWall.transform.position.z + 0.5f)
                {
                    currentRadius += 2f * Time.deltaTime; // déplacement sur le long de la courbe 
                }
                else if (direction == Vector3.down && currentRadius >=1)
                {
                    currentRadius -= 2f * Time.deltaTime;
                }
                else if (direction == Vector3.right && audioSource.transform.position.z < frontWall.transform.position.z-0.5f
                    && audioSource.transform.position.x < frontWall.transform.position.z - 0.5f)
                {
                    progress += 0.5f * Time.deltaTime; // déplacement en profondeur de la source
                }
                else if (direction == Vector3.left && audioSource.transform.position.z < frontWall.transform.position.z-0.5f
                    && audioSource.transform.position.x > -frontWall.transform.position.z + 0.5f)
                {
                    progress -= 0.5f * Time.deltaTime;
                } 
                else if (audioSource.transform.position.z >= frontWall.transform.position.z - 0.5f )
                {
                    audioSource.transform.localPosition = new Vector3(audioSource.transform.position.x, audioSource.transform.position.y, audioSource.transform.position.z - 1f * Time.deltaTime);
                    currentRadius = Mathf.Sqrt(audioSource.transform.localPosition.x * audioSource.transform.localPosition.x + audioSource.transform.localPosition.z * audioSource.transform.localPosition.z);
                    CalculNewProgress();
                }

                UpdateCurve(true);// maj de la courbe
            }
            else if (!hitGo.GetComponent<AudioSourceSript>().anchored) // si la source n'est pas ancrée, déplacement "libre"
            {
                if (direction == Vector3.up && audioSource.transform.position.z < frontWall.transform.position.z - 0.5f)
                {
                    audioSource.transform.localPosition = new Vector3(audioSource.transform.position.x, audioSource.transform.position.y, audioSource.transform.position.z + 1f * Time.deltaTime);
                }
                else if (direction == Vector3.down && audioSource.transform.localPosition.z>=0)
                {
                    audioSource.transform.localPosition = new Vector3(audioSource.transform.position.x, audioSource.transform.position.y, audioSource.transform.position.z - 1f * Time.deltaTime);
                }
                else if (direction == Vector3.right && audioSource.transform.position.x < frontWall.transform.position.z *2  - 0.5f)
                {
                    audioSource.transform.localPosition = new Vector3(audioSource.transform.position.x + 1f * Time.deltaTime, audioSource.transform.position.y, audioSource.transform.position.z);
                }
                else if (direction == Vector3.left && audioSource.transform.position.x > -frontWall.transform.position.z * 2 + 0.5f)
                {
                    audioSource.transform.localPosition = new Vector3(audioSource.transform.position.x - 1f * Time.deltaTime, audioSource.transform.position.y, audioSource.transform.position.z);
                }
                currentRadius = Mathf.Sqrt(audioSource.transform.localPosition.x * audioSource.transform.localPosition.x + audioSource.transform.localPosition.z * audioSource.transform.localPosition.z);
                CalculNewProgress(); // mise à jour du progress selon la nouvelle position
                UpdateCurve(false);
            }
        }
    }

    private void SelectTarget(GameObject hitGo)
    {
        if (hitGo.tag=="AudioSource" && hitGo.GetComponent<AudioSourceSript>().index == audioSource.GetComponent<AudioSourceSript>().index && hitGo.GetComponent<AudioSourceSript>().anchored)
        {
            visible = !visible;
            UpdateCurve(visible);
        }
    }

    private void SelectTarget2(GameObject hitGo)
    {
        if (hitGo.GetComponent<AudioSourceSript>().index == audioSource.GetComponent<AudioSourceSript>().index)
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

    private void SetBezierSpline() // mis en place de la courbe de bézier
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

    public void CalculNewProgress() // mise à jour du progress (panoramique) selon la nouvelle position de la source sonore (Pythagore)
    {
        currentRadius = Mathf.Sqrt(audioSource.transform.localPosition.x * audioSource.transform.localPosition.x + audioSource.transform.localPosition.z * audioSource.transform.localPosition.z);
        length = currentRadius * 0.552284749f;
        var cos = Mathf.Acos(audioSource.transform.localPosition.x / currentRadius) * Mathf.Rad2Deg;
        progress = Interpolate(cos);
    }

    public void UpdateCurve(bool move) // Mise à jour de la courbe
    {
        length = currentRadius * 0.552284749f; // Approximation de la distance des points de contrôles pour obtenir un cercle
        bezierSpline.Reset(); // remises à zéro de la courbe de bézier
        SetBezierSpline();
        pipe.curveRadius = currentRadius;
        pipe.UpdatePipe(visible); // mise à jour de la visualisation 
        pipe.pipeMaterial.color = Color.Lerp(Color.red, Color.yellow, progress); // gradient de couleur

        if (move)
            MoveAudioSource();

    }


    public void MoveAudioSource() // mise à jour de la position de la source sonore selon sa nouvelle position
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
        audioSource.transform.localPosition = calculatePosition;
        audioSource.transform.LookAt(Vector3.zero);
    }

    private void UpdateText() // mise à jour de l'orientation du texte pour suivre le regard du joueur 
    {
        textInfoGo.transform.position = new Vector3(audioSource.transform.position.x, 2, audioSource.transform.position.z);
        
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
