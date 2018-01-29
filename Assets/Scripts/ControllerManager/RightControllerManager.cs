using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightControllerManager : MonoBehaviour
{

    // Création d'évènement 
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


    private GameObject lastHitGo; // Garde en mémoire le dernier objet selectionné à l'aide du laser

    public Transform bezierSplinesContainer;
    bool showAll; // si vrai, toutes les courbes de bézier sont visibles

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

    private void Update()
    {
        if (!menuView.menuState) // Si le menu est désactivé 
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))  // Appui du trigger = selection d'une source / listener
            {
                var hitGo = this.GetComponentInParent<LaserPointer>().hitGo;
                if (hitGo && hitGo.tag == "AudioSource")  // si l'objet visée est un AudioSource
                {

                    if (lastHitGo == null) // s'il n'y a pas d'objet précédent en mémoire, on stock le nouvel objet visé
                    {
                        lastHitGo = hitGo;
                        lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                        if (!showAll) 
                            OnTriggerPressAction(lastHitGo); // Lance l'event OnTriggerPressAction : mise en évidence de la courbe de bézier
                    }
                    else if (lastHitGo != null && lastHitGo.tag != "AudioListener" && hitGo.GetComponent<AudioSourceSript>().index == lastHitGo.GetComponent<AudioSourceSript>().index) // si une même source sonore est visée, c'est une déselection
                    {
                        lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                        if (!showAll)
                            OnTriggerPressAction(lastHitGo);
                        lastHitGo = null;
                    }
                    else if (lastHitGo != null && lastHitGo.tag != "AudioListener" && hitGo.GetComponent<AudioSourceSript>().index != lastHitGo.GetComponent<AudioSourceSript>().index) // si une autre source sonore est visée, c'est une nouvelle sélection
                    {
                        lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                        if (!showAll)
                            OnTriggerPressAction(lastHitGo);
                        lastHitGo = hitGo;
                        lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                        if (!showAll)
                            OnTriggerPressAction(lastHitGo);
                    }
                    else if (lastHitGo != null && lastHitGo.tag == "AudioListener") // Si un point d'écoute est sélectionné, nouvelle sélection
                    {
                        lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                        lastHitGo = hitGo;
                        lastHitGo.GetComponent<Renderer>().material.color = Color.gray;
                        if (!showAll)
                            OnTriggerPressAction(lastHitGo);
                    }
                }
                else if (hitGo && hitGo.tag == "AudioListener" && lastHitGo == null) // Sélection d'un point d'écoute
                {
                    lastHitGo = hitGo;
                    lastHitGo.GetComponent<Renderer>().material.color = Color.green;
                }
                else if (hitGo && hitGo.tag == "AudioListener" && lastHitGo.tag == "AudioSource") // nouvelle sélection d'une audioSource
                {
                    lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                    if (!showAll)
                        OnTriggerPressAction(lastHitGo);
                    lastHitGo = hitGo;
                    lastHitGo.GetComponent<Renderer>().material.color = Color.green;
                }
                else if (hitGo && hitGo.tag == "AudioListener" && hitGo == lastHitGo) // Déselection d'un point d'écoute
                {
                    lastHitGo.GetComponent<Renderer>().material.color = Color.white;
                    lastHitGo = null;
                }
            }
            else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)) // Si le Trigger reste appuyé, et qu'un point d'écoute est séléctionné, l'appui du touchpad entrâine une rotation
            {
                if (lastHitGo && Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && lastHitGo.tag == "AudioListener") 
                {
                    Debug.Log("Rotation");
                    var touchpad = (Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                    if (touchpad.x > 0.2f)
                    {
                        OnTouchPadAndTriggerPress(true);
                    }
                    else if (touchpad.x < -0.2f)
                    {
                        OnTouchPadAndTriggerPress(false);
                    }
                    Debug.Log("touchpad x " + touchpad.x);
                }
            }
            else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) // A l'appui du touchpad, s'il n'ya aucun objet sélectionné, les murs sont déplacés 
            {
                if (lastHitGo  && lastHitGo.tag != "Untagged" && OnTouchpadPressAction != null)
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
                    if (lastHitGo == null && OnTouchpadPressAction != null)
                    {
                        if (touchpad.y > 0.7f)
                        {
                            OnTouchpadPressAction2(true); // déplacement des murs
                        }
                        else if (touchpad.y < -0.7f)
                        {
                            OnTouchpadPressAction2(false);
                        }
                    }
                }

            }
            else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) // A l'appui du grip, si une audioSource est séléctionnée, (dés)activation de la manette
            {
                if (lastHitGo && OnGripPressAction != null && lastHitGo.tag== "AudioSource")
                {
                    lastHitGo.GetComponent<AudioSourceSript>().anchored = !lastHitGo.GetComponent<AudioSourceSript>().anchored;
                    if (!showAll)
                        OnGripPressAction(lastHitGo);
                }
            }
            else if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))// A l'appui du bouton menu, toutes les courbes de béziers sont montrées ou cachées
            {
                showAll = !showAll;
                foreach (Transform t in bezierSplinesContainer)
                {
                    t.GetComponent<InteractivePipe>().visible = showAll ? true : false;

                    if (this.lastHitGo != null  && this.lastHitGo.GetComponent<AudioSourceSript>() && this.lastHitGo.GetComponent<AudioSourceSript>().index == t.GetComponent<InteractivePipe>().audioSource.GetComponent<AudioSourceSript>().index && lastHitGo.GetComponent<AudioSourceSript>().anchored)
                        t.GetComponent<InteractivePipe>().visible = true;
                    t.GetComponent<InteractivePipe>().UpdateCurve(false);
                }
            }
    }

    public void OnTriggerStay(Collider other) // Lorsque la manette rencontre une audioSource, l'appui sur le touchpad permet de monter ou descendre le volume
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
