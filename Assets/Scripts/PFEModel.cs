using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PFEModel : PFEElement {

    public AudioMixer mixer;
    public List<int> listeningSources = new List<int>();
    public List<int> modifyingSources = new List<int>();
    public int maxDuration = 60 * 6; // 6 minutes at large
}
