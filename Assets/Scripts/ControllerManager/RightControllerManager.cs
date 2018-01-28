using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightControllerManager : MonoBehaviour
{

    public delegate void TouchpadPressAction(GameObject hit, Vector3 dir);
    public static event TouchpadPressAction OnTouchpadPressAction;

    public delegate void TouchpadPressAction2(bool up);
    public static event TouchpadPressAction2 OnTouchpadPressAction2;

    public delegate void TriggerPressAction(GameObject hitGo); 
    public static event TriggerPressAction OnTriggerPressAction;

    public delegate void GripPressAction(GameObject hitGo);
    public static event GripPressAction OnGripPressAction;

    public delegate void TouchPadAndTriggerPress(bool dir);
    public static event TouchPadAndTriggerPress OnTouchPadAndTriggerPress;

    public GameObject menuViewGo;
    private MenuView menuView;


    private GameObject lastHitGo;
    public Transform bezierSplinesContainer;
    bool showAll;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        menuView = menuViewGo.GetComponent<PFEView>().menu;
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    //todo = virer toute logique d'ici
    private void Update()
    {
        if(!menuView.menuState)
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
                if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) && lastHitGo.tag == "AudioListener")
                {
                    var touchpad = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                        if (touchpad.x > 0.2f)
                        {
                        OnTouchPadAndTriggerPress(true);
                        }
                        else if (touchpad.x < -0.2f)
                        {
                        OnTouchPadAndTriggerPress(false);
                        }
                }
                else
                {
                    var hitGo = this.GetComponentInParent<LaserPointer>().hitGo;
                    if (hitGo && hitGo.tag == "AudioSource")
                    {

                        if (lastHitGo == null)
                        {
                            lastHitGo = hitGo;
                            lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                            if (!showAll)
                                OnTriggerPressAction(lastHitGo);
                        }
                        else if (lastHitGo != null && lastHitGo.tag != "AudioListener" && hitGo.GetComponent<AudioSourceSript>().index == lastHitGo.GetComponent<AudioSourceSript>().index)
                        {
                            lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                            if (!showAll)
                                OnTriggerPressAction(lastHitGo);
                            lastHitGo = null;
                        }
                        else if (lastHitGo != null && lastHitGo.tag != "AudioListener" && hitGo.GetComponent<AudioSourceSript>().index != lastHitGo.GetComponent<AudioSourceSript>().index)
                        {
                            lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                            if (!showAll)
                                OnTriggerPressAction(lastHitGo);
                            lastHitGo = hitGo;
                            lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                            if (!showAll)
                                OnTriggerPressAction(lastHitGo);
                        }
                        else if (lastHitGo != null && lastHitGo.tag == "AudioListener")
                        {
                            lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                            lastHitGo = hitGo;
                            lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                            if (!showAll)
                                OnTriggerPressAction(lastHitGo);
                        }
                    }
                    else if (hitGo && hitGo.tag == "AudioListener" && lastHitGo == null)
                    {
                        lastHitGo = hitGo;
                        lastHitGo.GetComponent<Renderer>().material.color = Color.green;
                    }
                    else if (hitGo && hitGo.tag == "AudioListener" && lastHitGo.tag == "AudioSource")
                    {
                        lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                        if (!showAll)
                            OnTriggerPressAction(lastHitGo);
                        lastHitGo = hitGo;
                        lastHitGo.GetComponent<Renderer>().material.color = Color.green;
                    }
                    else if (hitGo && hitGo.tag == "AudioListener" && hitGo == lastHitGo)
                    {
                        lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                        lastHitGo = null;
                    }
                }
        }
        else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (lastHitGo && OnTouchpadPressAction != null)
            {
                var offSetTouchPad = 0.0;
                if (lastHitGo.tag == "AudioSource")
                    offSetTouchPad = lastHitGo.GetComponent<AudioSourceSript>().anchored ? 0.7f : 0.2f;
                else
                    offSetTouchPad = 0.2f;
                var touchpad = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                if (touchpad.y > offSetTouchPad)
                {
                    OnTouchpadPressAction(lastHitGo, Vector3.up);
                }
                else if (touchpad.y < -offSetTouchPad)
                {
                    OnTouchpadPressAction(lastHitGo, Vector3.down);
                }
                if (touchpad.x > offSetTouchPad)
                {
                    OnTouchpadPressAction(lastHitGo, Vector3.right);
                }
                else if (touchpad.x < -offSetTouchPad)
                {
                    OnTouchpadPressAction(lastHitGo, Vector3.left);
                }
            }
            else
            {
                var touchpad = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                if (lastHitGo== null && OnTouchpadPressAction != null)
                {
                    if (touchpad.y > 0.7f)
                    {
                        OnTouchpadPressAction2(true);
                    }
                    else if (touchpad.y < -0.7f)
                    {
                        OnTouchpadPressAction2(false);
                    }
                }
            }

        }
        else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (lastHitGo && OnGripPressAction!=null)
            {
                lastHitGo.GetComponent<AudioSourceSript>().anchored = !lastHitGo.GetComponent<AudioSourceSript>().anchored;
                if(!showAll)
                OnGripPressAction(lastHitGo);
            }
        }
        else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            showAll = !showAll;
            foreach(Transform t in bezierSplinesContainer)
            {
                t.GetComponent<InteractivePipe>().visible = showAll ? true : false;
                if (this.lastHitGo != null && this.lastHitGo.GetComponent<AudioSourceSript>().index == t.GetComponent<InteractivePipe>().audioGo.GetComponent<AudioSourceSript>().index && lastHitGo.GetComponent<AudioSourceSript>().anchored)
                    t.GetComponent<InteractivePipe>().visible = true;
                t.GetComponent<InteractivePipe>().UpdateCurve(false);
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("AudioSource"))
        {
            if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {
                float volumeControl = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y + 1) / 2;
                other.transform.GetChild(0).GetChild(0).localScale = new Vector3(1.05f, volumeControl, 1.05f);
                other.GetComponent<AudioSource>().volume = volumeControl;
            }
        }
    }
}
