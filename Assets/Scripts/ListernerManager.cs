using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListernerManager : MonoBehaviour {

    public GameObject ears;
    public Transform head;
    public Transform listenerGo;
    private GameObject listGo;
    public Transform refListener;

    private void OnEnable()
    {
        LeftControllerManager.OnTriggerPressAction += ChangeListener;
        RightControllerManager.OnTouchPadAndTriggerPress += ChangeListenerOrientation;

    }

    private void OnDisable()
    {
        LeftControllerManager.OnTriggerPressAction -= ChangeListener;
        RightControllerManager.OnTouchPadAndTriggerPress -= ChangeListenerOrientation;
    }

    private void Awake()
    {
        listGo = this.transform.GetChild(0).gameObject;
        listGo.SetActive(false);
        refListener.gameObject.SetActive(false);
    }

    void ChangeListener()
    {
        if (ears.transform.parent == listenerGo) // listenerGo to head
        {
            ears.transform.parent = head;
            listGo.SetActive(false);
        }
        else if(ears.transform.parent == head) //head to RefListener
        {
            ears.transform.parent = refListener;
            refListener.gameObject.SetActive(true);
        }
        else if (ears.transform.parent == refListener) // refListener to listernerGo
        {
            ears.transform.parent = listenerGo;
            listGo.SetActive(true);
            refListener.gameObject.SetActive(false);
        }

        ears.transform.localPosition = Vector3.zero;
        ears.transform.rotation = ears.transform.parent.transform.rotation;
    }

    void ChangeListenerOrientation(bool dir)
    {
        if (ears.transform.parent == listenerGo)
        {
            if (dir)
            {
                listenerGo.transform.Rotate(Vector3.up * Time.deltaTime *15f);
            }
            else
            {
                listenerGo.transform.Rotate(Vector3.down * Time.deltaTime*15f);
            }
        }
    }
}
