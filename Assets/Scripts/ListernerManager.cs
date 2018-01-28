using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListernerManager : MonoBehaviour {

    public GameObject ears;
    public Transform head;
    public Transform listenerGo;
    private GameObject listGo;

    private void OnEnable()
    {
        LeftControllerTelepotation.OnTriggerPressAction += ChangeListener;
        RightControllerManager.OnTouchPadAndTriggerPress += ChangeListenerOrientation;

    }

    private void OnDisable()
    {
        LeftControllerTelepotation.OnTriggerPressAction -= ChangeListener;
        RightControllerManager.OnTouchPadAndTriggerPress -= ChangeListenerOrientation;
    }

    private void Awake()
    {
        listGo = this.transform.GetChild(0).gameObject;
        listGo.SetActive(false);
    }

    void ChangeListener()
    {
        if (ears.transform.parent == listenerGo)
        {
            ears.transform.parent = head;
            listGo.SetActive(false);
        }
        else
        {
            ears.transform.parent = listenerGo;
            listGo.SetActive(true);
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
                listenerGo.transform.Rotate(0, 1f * Time.deltaTime, 0);
            }
            else
            {
                listenerGo.transform.Rotate(0, -1f * Time.deltaTime, 0);
            }
        }
    }
}
