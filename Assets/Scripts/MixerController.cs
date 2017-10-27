using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour {
    public AudioMixer masterMixer;
    public float volume;
    public float reverbDecay;
    public float reverbRoom;

    void Start()
    {
       masterMixer.GetFloat("MasterVolume", out volume);
        masterMixer.GetFloat("ReverbDecayTime", out reverbDecay);
        masterMixer.GetFloat("ReverbRoom", out reverbRoom);
    }

    public void SetMusicVol(float f)
    {
        masterMixer.SetFloat("MasterVolume", f);
        volume = f;
    }

    public float GetMusicVol()
    {
        float f;
        masterMixer.GetFloat("Masterfume", out f);
        volume = f;
        return f;
    }

    public void SetReverbDecayTime(float f)
    {
        masterMixer.SetFloat("ReverbDecayTime", f);
        reverbDecay = f;
    }

    public float GetReverbDecayTime()
    {
        float f;
        masterMixer.GetFloat("ReverbDecayTime", out f);
        reverbDecay = f;
        return f;
    }

    public void SetReverbRoom(float f)
    {
        masterMixer.SetFloat("ReverbRoom", f);
        reverbRoom = f;
    }

    public float GetReverbRoom()
    {
        float f;
        masterMixer.GetFloat("ReverbRoom", out f);
        reverbRoom = f;
        return f;
    }

}
