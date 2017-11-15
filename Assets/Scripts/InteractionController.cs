using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public static float tresholdTP = 1f;

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;
    private Vector3 initialPosition;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();        
    }


    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);

        if (other.CompareTag("Pylone"))
        {
            if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {
                float volumeControl = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y + 1) / 2;
                other.transform.GetChild(1).GetChild(0).localScale = new Vector3(1.05f, volumeControl, 1.05f);
                other.GetComponent<AudioSource>().volume = volumeControl;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }

    void Update()
    {
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

        if (objectInHand)
        {
            if (objectInHand.CompareTag("Sphere") && Vector3.Distance(objectInHand.transform.position, Camera.main.transform.position) < tresholdTP)
            {
                AudioSource source = objectInHand.transform.parent.GetComponent<AudioSource>();
                ReleaseObject();
                if (source)
                {
                    RoomManager.GoToRoom(source.clip);
                }
                else
                {
                    RoomManager.ReturnToDesktop();
                }
            }
        }
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;
        if (objectInHand.CompareTag("Sphere"))
        {
            initialPosition = objectInHand.transform.parent.position;
            initialPosition.y = 4f;
        }
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        objectInHand.GetComponent<Rigidbody>().isKinematic = false;
        objectInHand.GetComponent<Renderer>().material.color = new Color32(18, 88, 140, 1);
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = Mathf.Infinity;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            if (objectInHand.CompareTag("Sphere"))
            {
                objectInHand.transform.position = initialPosition;
            }
            objectInHand.GetComponent<Rigidbody>().isKinematic = true;
            objectInHand.GetComponent<Renderer>().material.color = Color.white;
        }

        objectInHand = null;
    }
}
