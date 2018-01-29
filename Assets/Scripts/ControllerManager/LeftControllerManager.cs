using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftControllerManager : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public GameObject hitGo;
    public GameObject plateform;

    public Transform cameraRigTransform;

    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;
    private bool up;

    public delegate void TeleportAction();
    public static event TeleportAction OnTeleportation;

    public delegate void TriggerPressAction(); // show 
    public static event TriggerPressAction OnTriggerPressAction;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        plateform.SetActive(false);
    }
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
    hit.distance);
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        if (Controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                this.hitGo = hit.collider.gameObject;
                hitPoint = hit.point; 
                ShowLaser(hit);
                if (hitGo.name == "Floor") // Si le laser touche le sol, on fait apparaitre une cible, l'utilisateur peut se téléporter
                {
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                    shouldTeleport = true;
                }
                else
                {
                    reticle.SetActive(false);
                    shouldTeleport = false;
                }
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) // Au relâchement du touchpad, il y a téléportation
        {
            Teleport();
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerPressAction();
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) // Offset d'hauteur : l'utilisateur peut être placé 5 mètres au dessus de la scène
        {
            up = !up;
            if (up)
            {
                cameraRigTransform.position = new Vector3(cameraRigTransform.position.x, cameraRigTransform.position.y + 5, cameraRigTransform.position.z);
                plateform.SetActive(true);
            }
            else
            {
                cameraRigTransform.position = new Vector3(cameraRigTransform.position.x, cameraRigTransform.position.y - 5, cameraRigTransform.position.z);
                plateform.SetActive(false);
            }
            OnTeleportation();

        }
    }

    private void Teleport()
    {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = hitPoint + difference;
        if (up)
            cameraRigTransform.position = new Vector3(cameraRigTransform.position.x, cameraRigTransform.position.y + 5, cameraRigTransform.position.z);
        OnTeleportation();
    }
}