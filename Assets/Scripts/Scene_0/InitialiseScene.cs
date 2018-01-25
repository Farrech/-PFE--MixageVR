using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialiseScene : MonoBehaviour {

    public List<GameObject> audioSourcesL;
    public List<InteractiveBezier> bezierSplineL;
    public GameObject audioPrefab;
    public GameObject bezierSplinePrefab;
    public GameObject pipePrefab;
    public Transform audioSourcesContainer, bezierSplinescontainer, pipesContainer;
    public int DIMENSION; // à changer
    public Transform canvasInfo;
    public GameObject textInfoPrefab;


    private void Awake()
    {
        audioSourcesL = new List<GameObject>();
        bezierSplineL = new List<InteractiveBezier>();

        var listTracks = Resources.LoadAll("Sounds");
        var progress = 1f / (listTracks.Length+1);

        for (int i = 0; i < listTracks.Length; i++)
        {
            var audioGo = Instantiate(audioPrefab,audioSourcesContainer);
            audioGo.GetComponent<GvrAudioSource>().clip = listTracks[i] as AudioClip;
            audioGo.GetComponent<AudioSourceSript>().index = i;
            // Présentation en ligne 
            //audioGo.transform.localPosition = new Vector3(-DIMENSION / listTracks.Length + i * DIMENSION / listTracks.Length, 0.5f, 10); // En dur, à changer
            // ou présentation en arc de cercle
            var textInfo = Instantiate(textInfoPrefab, canvasInfo);
            textInfo.GetComponent<Text>().text = listTracks[i].name;
            var pipeGo = Instantiate(pipePrefab, pipesContainer);
            var bezierGo = Instantiate(bezierSplinePrefab,bezierSplinescontainer);
            //bezierGo.transform.localPosition = new Vector3(0, 0.5f, 0);
            bezierGo.GetComponent<InteractivePipe>().audioGo = audioGo;
            bezierGo.GetComponent<InteractivePipe>().textInfoGo = textInfo;
            bezierGo.GetComponent<InteractivePipe>().currentRadius = 15; // En dur à changer
            bezierGo.GetComponent<InteractivePipe>().pipe = pipeGo.GetComponent<Pipe>();
            //bezierGo.GetComponent<InteractivePipe>().CalculNewProgress();
            bezierGo.GetComponent<InteractivePipe>().progress = progress * (i + 1);
            //bezierGo.GetComponent<InteractivePipe>().UpdateCurve(false);
            bezierGo.GetComponent<InteractivePipe>().UpdateCurve(true);
            //textInfo.transform.position = new Vector3(bezierGo.GetComponent<InteractivePipe>().audioGo.transform.position.x, 2, bezierGo.GetComponent<InteractivePipe>().audioGo.transform.position.z);
            audioSourcesL.Add(audioGo);
            bezierSplineL.Add(bezierGo.GetComponent<InteractiveBezier>());
        }
    }


   
    
}
