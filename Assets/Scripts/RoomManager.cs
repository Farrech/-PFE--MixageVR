using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    static GameObject desktop;
    static GameObject room;
    static AudioSource source;

	void Start () {
        desktop = transform.GetChild(1).gameObject;
        room = transform.GetChild(2).gameObject;
        source = GetComponent<AudioSource>();
    }

    public static void GoToRoom(AudioClip newClip)
    {
        desktop.SetActive(false);
        room.SetActive(true);
        source.clip = newClip;
        source.Play();
    }

    public static void ReturnToDesktop()
    {
        desktop.SetActive(true);
        room.SetActive(false);
        source.Stop();
    }
}
