using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightControllerManager : MonoBehaviour
{

    public delegate void TriggerTouchAction(GameObject hit);
    public static event TriggerTouchAction OnTriggerTouchAction;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Update()
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject hitGo = this.GetComponentInParent<LaserPointer>().hitGo;
            if(hitGo && OnTriggerTouchAction != null)
            {
                OnTriggerTouchAction(hitGo);
            }
        }

    }

}
