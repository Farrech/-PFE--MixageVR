﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PFEController : PFEElement {

    // The menu button is used to switch the menu canvas on and off
    public VRTK_ControllerEvents[] controllerEvents = new VRTK_ControllerEvents[2];
    void OnEnable()
    {
        foreach (VRTK_ControllerEvents controller in controllerEvents)
            controller.ButtonTwoPressed += ControllerEvents_ButtonTwoReleased;
    }
    void OnDisable()
    {
        foreach (VRTK_ControllerEvents controller in controllerEvents)
            controller.ButtonTwoPressed -= ControllerEvents_ButtonTwoReleased;
    }
    void ControllerEvents_ButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
    {
        app.view.menu.SwitchMenuState();
    }

    // Each time the Distorsion view has changed
    public void OnDistorsionChange(float newDistortionLevel)
    {
        foreach (int sourceIndex in app.model.modifyingSources) //pour toutes les sources concernées par la modification
        {
            Transform child = app.model.transform.GetChild(sourceIndex);
            // soit la nouvelle valeur est faible, et le component est supprimé, soit il est instantié, soit il est mis à jour
            if (newDistortionLevel <= 0.01F)
            {
                Destroy(child.GetComponent<AudioDistortionFilter>());
            }
            else
            {
                AudioDistortionFilter distorsionFilter = child.GetComponent<AudioDistortionFilter>();
                if (!distorsionFilter)
                    distorsionFilter = child.gameObject.AddComponent<AudioDistortionFilter>();
                distorsionFilter.distortionLevel = newDistortionLevel;                
            }
        }
    }

    // Each time the Echo view has changed
    public void OnEchoChange(float delay, float decay)
    {
        foreach (int sourceIndex in app.model.modifyingSources)
        {
            Transform child = app.model.transform.GetChild(sourceIndex);
            if (delay <= 10)
            {
                Destroy(child.GetComponent<AudioEchoFilter>());
            }
            else
            {
                AudioEchoFilter echoFilter = child.GetComponent<AudioEchoFilter>();
                if (!echoFilter)
                    echoFilter = child.gameObject.AddComponent<AudioEchoFilter>();
                echoFilter.delay = delay;
                echoFilter.decayRatio = decay;
            }
        }
    }
}
