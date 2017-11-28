using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FollowSpline : MonoBehaviour
{

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


    // Use this for initialization
    void Start()
    {
        bezierSpline = bezierSplineContainer.transform.GetChild(currentSpline).GetComponent<BezierSpline>();
        arcMesh = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<ArcMesh>();
        arcMeshMaterial= arcMeshContainer.transform.GetChild(currentSpline).GetComponent<Renderer>().material;
        defaultArcMeshColor = arcMeshMaterial.color;
        this.transform.localPosition = bezierSpline.GetPoint(0.5f);
        duration = 5;
        progress = 0.5f;
    }

    // Update is called once per frame
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
            else if (Input.GetKeyDown(KeyCode.UpArrow) && currentSpline < bezierSplineContainer.transform.childCount)
            {
                masterMixer.SetMusicVol(masterMixer.volume + 5);
                currentSpline++;
                ChangeSpline();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentSpline > 0)
            {
                masterMixer.SetMusicVol(masterMixer.volume - 5);
                currentSpline--;
                ChangeSpline();
            }
            arcMeshMaterial.color = Color.Lerp(Color.red, Color.yellow, progress);
            MoveAudioSource();
            this.GetComponent<AudioSource>().panStereo = Interpolate(progress);
            //print(progress + " - " + Interpolate(progress));
        }
        else if (Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && masterMixer.reverbDecay < 20)
            {
                masterMixer.SetReverbDecayTime(masterMixer.reverbDecay + 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && masterMixer.reverbDecay > 0) 
            {
                masterMixer.SetReverbDecayTime(masterMixer.reverbDecay - 1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && masterMixer.reverbRoom<0)
            {
                masterMixer.SetReverbRoom(masterMixer.reverbRoom + 100);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && masterMixer.reverbRoom > -5000)
            {
                masterMixer.SetReverbRoom(masterMixer.reverbRoom - 100);
            }
        }
    }

    public void ChangeSpline()
    {
        arcMeshMaterial.color = defaultArcMeshColor;
        bezierSpline = bezierSplineContainer.transform.GetChild(currentSpline).GetComponent<BezierSpline>();
        arcMesh = arcMeshContainer.transform.GetChild(currentSpline).GetComponent<ArcMesh>();
        arcMeshMaterial= arcMeshContainer.transform.GetChild(currentSpline).GetComponent<Renderer>().material;
    }
    public void MoveAudioSource()
    {
        Vector3 calculatePosition = bezierSpline.GetPoint(progress);
        transform.localPosition = calculatePosition;
        transform.LookAt(calculatePosition + bezierSpline.GetDirection(progress));
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
}
