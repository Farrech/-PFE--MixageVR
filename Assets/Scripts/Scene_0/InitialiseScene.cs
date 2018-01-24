using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialiseScene : MonoBehaviour {

    public List<GameObject> audioSourcesL;
    public List<InteractiveBezier> bezierSplineL;
    public GameObject audioPrefab;
    public GameObject bezierSplinePrefab;
    public GameObject pipePrefab;
    public Transform audioSourcesContainer, bezierSplinescontainer, pipesContainer;
    public int DIMENSION; // à changer



    private void Awake()
    {
        audioSourcesL = new List<GameObject>();
        bezierSplineL = new List<InteractiveBezier>();

        var listTracks = Resources.LoadAll("Sounds");
        var progress = 1f / (listTracks.Length+1);
        Debug.Log(listTracks.Length + " - " + progress);

        for (int i = 0; i < listTracks.Length; i++)
        {
            var audioGo = Instantiate(audioPrefab,audioSourcesContainer);
            audioGo.GetComponent<GvrAudioSource>().clip = listTracks[i] as AudioClip;
            audioGo.GetComponent<AudioSourceSript>().index = i;
            // Présentation en ligne 
            //audioGo.transform.localPosition = new Vector3(-DIMENSION / listTracks.Length + i * DIMENSION / listTracks.Length, 0.5f, 10); // En dur, à changer
            // ou présentation en arc de cercle
            var pipeGo = Instantiate(pipePrefab, pipesContainer);
            var bezierGo = Instantiate(bezierSplinePrefab,bezierSplinescontainer);
            bezierGo.transform.localPosition = new Vector3(0, 0.5f, 0);
            bezierGo.GetComponent<InteractivePipe>().audioGo = audioGo;
            bezierGo.GetComponent<InteractivePipe>().currentRadius = 15; // En dur à changer
            bezierGo.GetComponent<InteractivePipe>().pipe = pipeGo.GetComponent<Pipe>();
            //bezierGo.GetComponent<InteractivePipe>().CalculNewProgress();
            bezierGo.GetComponent<InteractivePipe>().progress = progress * (i + 1);
            //bezierGo.GetComponent<InteractivePipe>().UpdateCurve(false);
            bezierGo.GetComponent<InteractivePipe>().UpdateCurve(true);

            audioSourcesL.Add(audioGo);
            bezierSplineL.Add(bezierGo.GetComponent<InteractiveBezier>());
        }
    }


   
    
}
