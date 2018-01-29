using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : PFEElement
{
    public Text playPauseText;
    public Slider mixSlider;
    public GameObject soundPanelPrefab;
    public GameObject mixPanelPrefab;
    public GameObject textureButtonPrefab;
    public Transform texturePanel;
    public Transform soundPanel;
    public Transform mixPanel;
    public Transform head;
    public ResonanceAudioRoom rar;
    public bool menuState;
    public GameObject listenerGo;
    public Transform bezierSplineContainer;

    List<Toggle> toggles = new List<Toggle>();
    float currentTime = 0;
    bool isPlaying = false;
    
    void Update()
    {
        // mise à jour du curseur de temps
        if (currentTime < app.model.maxDuration && isPlaying)
        {
            currentTime += Time.deltaTime;
            mixSlider.value = currentTime / app.model.maxDuration;
            mixSlider.GetComponentInChildren<Text>().text = Mathf.Floor(currentTime / 60) + ":" + Mathf.RoundToInt(currentTime % 60);
        }
    }

    public void SwitchMenuState()
    {
        menuState = !menuState;
        gameObject.SetActive(menuState);
        if(menuState)
        {
            // chaque fois que le menu est ouvert, il est placé en avant de l'utilisateur et reste fixe
            transform.position = head.position + head.forward * 5;
            if (transform.position.y < 2)
                transform.position = new Vector3(transform.position.x, 2, transform.position.z);
            transform.LookAt(head);
            transform.Rotate(Vector3.up * 180);
        }
    }

    public void DisplaySongs()
    {
        GameObject newPanel;
        int i = 0;
        foreach (Transform child in app.model.transform)
        {
            // Afficher les titres des pistes ainsi que les cases à cocher "écouter" et "modifier"
            newPanel = Instantiate(soundPanelPrefab, soundPanel);
            newPanel.GetComponentInChildren<Text>().text = child.name;
            Toggle[] newToggles = newPanel.GetComponentsInChildren<Toggle>();
            int tempI = i;
            newToggles[0].onValueChanged.AddListener(delegate { OnModifyChange(tempI); });
            newToggles[1].onValueChanged.AddListener(delegate { OnListenChange(tempI); });
            toggles.AddRange(newToggles);

            // Afficher la longueur des pistes dans la partie de droite, où se trouve le slider
            newPanel = Instantiate(mixPanelPrefab, mixPanel);
            newPanel.GetComponent<LayoutElement>().preferredWidth = app.model.maxDuration;
            float duration = child.GetComponent<AudioSource>().clip.length;
            newPanel.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, duration);

            i++;
        }
    }

    public void DisplayTextures()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        GameObject newPanel;

        foreach (Sprite sprite in sprites)
        {
            // Afficher les boutons permettant de changer les texture des murs
            newPanel = Instantiate(textureButtonPrefab, texturePanel);
            newPanel.name = sprite.name;
            newPanel.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
            newPanel.GetComponentInChildren<Text>().text = sprite.name;
            Button button = newPanel.GetComponent<Button>();
            button.interactable = !(sprite.name == rar.leftWall.ToString());
            button.onClick.AddListener(delegate { OnTextureButtonClick(sprite.name); });
        }
    }

    public void OnTextureButtonClick(string textureName)
    {
        // Chaque bouton cesse d'être interactable lorsqu'il est cliqué, puisqu'il représente déjà la surface utilisée
        foreach(Transform child in texturePanel)
        {
            Button button = child.GetComponent<Button>();
            button.interactable = !(button.name == textureName);
        }

        ResonanceAudioRoomManager.SurfaceMaterial surface = (ResonanceAudioRoomManager.SurfaceMaterial)Enum.Parse(typeof(ResonanceAudioRoomManager.SurfaceMaterial), textureName);
        rar.leftWall = surface;
        rar.rightWall = surface;
        rar.frontWall = surface;
        rar.backWall = surface;

        foreach (GameObject wall in app.model.walls)
        {
            wall.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/" + textureName);
        }
    }

    public void OnModifyChange(int soundIndex)
    {
        if (app.model.modifyingSources.Contains(soundIndex))
            app.model.modifyingSources.Remove(soundIndex);
        else
            app.model.modifyingSources.Add(soundIndex);
    }

    public void OnListenChange(int soundIndex)
    {
        if (app.model.listeningSources.Contains(soundIndex))
            app.model.listeningSources.Remove(soundIndex);
        else
            app.model.listeningSources.Add(soundIndex);
    }

    public void Export() // permet d'exporter les paramètres de chaque pistes vers un fichier texte
    {
        string exp = String.Empty;
        int i = 0;

        foreach(Transform c in app.model.transform)
        {
            var panoramic = bezierSplineContainer.GetChild(i).GetComponent<InteractivePipe>().progress;
            var volume = c.GetComponent<AudioSource>().volume;
            var delay = bezierSplineContainer.GetChild(i).GetComponent<InteractivePipe>().currentRadius/340.29f;
            exp += "Piste : " + c.name + "\n    panoramique = " + panoramic + "\n      volume = " + volume + "\n     delay = " + delay + "\n     Reverb = " + rar.size.ToString() +  " \n\n";
            i++;
        }
        File.WriteAllText("Export.txt", exp);
    }

    public void PlayPause()
    {
        isPlaying = !isPlaying;
        playPauseText.text = isPlaying ? "Pause" : "Play";
        if (isPlaying)
        {
            foreach (int index in app.model.listeningSources)
            {
                AudioSource source = app.model.transform.GetChild(index).GetComponent<AudioSource>();
                source.time = (currentTime < source.clip.length ? currentTime : source.clip.length - 1); // on déplace le début de la lecture en fonction du slider
                source.Play();
            }
        }
        else
            foreach (Transform child in app.model.transform)
                child.GetComponent<AudioSource>().Stop();        
        foreach (Toggle toggle in toggles)
            toggle.interactable = !isPlaying;
    }

    public void SetListener() // place l'objet lisener à la position et à la même rotation que la position de l'utilisateur
    {
        listenerGo.transform.position = new Vector3(head.transform.position.x, listenerGo.transform.position.y, head.position.z);
        listenerGo.transform.rotation = head.transform.rotation;
    }

    public void ResetMix()
    {
        isPlaying = false;
        playPauseText.text = "Play";
        foreach (Transform child in app.model.transform)        
            child.GetComponent<AudioSource>().Stop();
        foreach (Toggle toggle in toggles)
        {
            toggle.interactable = true;
            toggle.isOn = false;
        }
        currentTime = 0;
        mixSlider.value = 0;
        mixSlider.GetComponentInChildren<Text>().text = "00:00";
    }

    public void SliderValueChanged()
    {
        currentTime = mixSlider.value * app.model.maxDuration;
    }
}
