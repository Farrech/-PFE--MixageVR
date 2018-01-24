using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightControllerManager : MonoBehaviour
{

    public delegate void TouchpadPressAction(GameObject hit, Vector3 dir);
    public static event TouchpadPressAction OnTouchpadPressAction;

    public delegate void TriggerPressAction(GameObject hitGo); // show 
    public static event TriggerPressAction OnTriggerPressAction;


    private GameObject localHitGo;
    public Transform bezierSplinesContainer;
    bool showAll;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    //todo = virer toute logique d'ici
    private void Update()
    {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            var hitGo = this.GetComponentInParent<LaserPointer>().hitGo;
            if (hitGo.tag == "AudioSource")
            {
                if (localHitGo == null)
                {
                    localHitGo = hitGo;
                    localHitGo.GetComponent<Renderer>().material.color = Color.red;
                    if (!showAll)
                        OnTriggerPressAction(localHitGo);
                }
                else if( localHitGo!=null && hitGo.GetComponent<AudioSourceSript>().index== localHitGo.GetComponent<AudioSourceSript>().index)
                {
                    localHitGo.GetComponent<Renderer>().material.color = Color.black;
                    if (!showAll)
                        OnTriggerPressAction(localHitGo);
                    localHitGo = null;
                }else if (localHitGo != null && hitGo.GetComponent<AudioSourceSript>().index != localHitGo.GetComponent<AudioSourceSript>().index)
                {
                    localHitGo.GetComponent<Renderer>().material.color = Color.black;
                    if (!showAll)
                        OnTriggerPressAction(localHitGo);
                    localHitGo = hitGo;
                    localHitGo.GetComponent<Renderer>().material.color = Color.red;
                    if (!showAll)
                        OnTriggerPressAction(localHitGo);
                }

            }
        }
        else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (localHitGo && OnTouchpadPressAction != null)
            {
                Vector2 touchpad = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                if (touchpad.y > 0.7f)
                {
                    OnTouchpadPressAction(localHitGo, Vector3.up);
                }
                else if (touchpad.y < -0.7f)
                {
                    OnTouchpadPressAction(localHitGo, Vector3.down);
                }
                if (touchpad.x > 0.7f)
                {
                    OnTouchpadPressAction(localHitGo, Vector3.right);
                }
                else if (touchpad.x < -0.7f)
                {
                    OnTouchpadPressAction(localHitGo, Vector3.left);
                }
            }
        }
        else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (localHitGo)
            {
                localHitGo.GetComponent<AudioSourceSript>().anchored = !localHitGo.GetComponent<AudioSourceSript>().anchored;
            }
        }
        else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            showAll = !showAll;
            foreach(Transform t in bezierSplinesContainer)
            {
                t.GetComponent<InteractivePipe>().visible = showAll ? true : false;
                if (this.localHitGo != null && this.localHitGo.GetComponent<AudioSourceSript>().index == t.GetComponent<InteractivePipe>().audioGo.GetComponent<AudioSourceSript>().index)
                    t.GetComponent<InteractivePipe>().visible = true;
                t.GetComponent<InteractivePipe>().UpdateCurve(false);
            }
        }
    }
}
