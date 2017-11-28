using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {


    public GameObject laserPrefab;
    private SteamVR_TrackedObject trackedObj;
    private GameObject laser;
    private Transform laserTransform;
    public LayerMask teleportMask;
    [HideInInspector]
    public GameObject hitGo;
    public RaycastHit hit;
   



    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }


    void Update()
    {
        // controller is targetting an object through the laser
        if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                this.hitGo = hit.collider.gameObject;
                if (!this.transform.GetComponent<FixedJoint>())
                    ShowLaser(hit);
                else
                    ResetLaser();
            }
        }
        else // Hide the laser when the player released the touchpad.
        {
            ResetLaser();
        }
    }

    public void ResetLaser()
    {
        laser.SetActive(false);
        hitGo = null;
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hit.point, .5f);
        laserTransform.LookAt(hit.point);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

}
