using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Base class for all elements in this application.
public class PFEElement : MonoBehaviour
{
    // Gives access to the application and all instances.
    public PFEApplication app { get { return GameObject.FindObjectOfType<PFEApplication>(); } }
}

// Entry Point.
public class PFEApplication : MonoBehaviour
{
    // Reference to the root instances of the MVC.
    public PFEModel model;
    public PFEView view;
    public PFEController controller;

    public List<GameObject> audioSourcesL;
    public List<InteractiveBezier> bezierSplineL;

    public bool loadFromResources = false;
    public GameObject audioPrefab;
    public GameObject bezierSplinePrefab;
    public GameObject pipePrefab;
    public Transform audioSourcesContainer, bezierSplinescontainer, pipesContainer;
    public Transform canvasInfo;
    public GameObject textInfoPrefab;
    public Transform frontWall;

    // Init things here
    void Awake() {
        if (loadFromResources)
        {
            Object[] clips = Resources.LoadAll("Sounds", typeof(AudioClip));
            AudioSource source;
            var progress = 1f / (clips.Length + 1);

            for (int i = 0; i < clips.Length; i++)
            {                
                model.listeningSources.Add(i);
                model.modifyingSources.Add(i);
                var audioGo = Instantiate(audioPrefab, model.transform);
                source = audioGo.GetComponent<AudioSource>();
                source.clip = (AudioClip)clips[i];
                source.name = source.clip.name;
                audioGo.GetComponent<AudioSourceSript>().index = i;
                var textInfo = Instantiate(textInfoPrefab, canvasInfo);
                textInfo.GetComponent<Text>().text = clips[i].name;
                var pipeGo = Instantiate(pipePrefab, pipesContainer);
                var bezierGo = Instantiate(bezierSplinePrefab, bezierSplinescontainer);
                var interactivePipe = bezierGo.GetComponent<InteractivePipe>();
                interactivePipe.audioGo = audioGo;
                interactivePipe.frontWall = frontWall;
                interactivePipe.textInfoGo = textInfo;
                interactivePipe.currentRadius = 15; // En dur à changer
                interactivePipe.pipe = pipeGo.GetComponent<Pipe>();
                interactivePipe.progress = progress * (i + 1);
                interactivePipe.UpdateCurve(true);
                audioSourcesL.Add(audioGo);
                bezierSplineL.Add(bezierGo.GetComponent<InteractiveBezier>());
            }
        }
        view.menu.DisplaySongs();
        view.menu.PlayPause();
        view.menu.SwitchMenuState();
    }
}
