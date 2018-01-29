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
        if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad) && ! Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) // Si le touchPad est appuyé mais pas préssé
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask)) // si une collision est détectée
            {
                this.hitGo = hit.collider.gameObject; // on garde l'objet en mémoire
                if (!this.transform.GetComponent<FixedJoint>())
                    ShowLaser(hit);
                else
                    ResetLaser();
            }
        }
        else 
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
