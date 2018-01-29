using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PFEModel : PFEElement {

    public List<int> listeningSources = new List<int>();
    public List<int> modifyingSources = new List<int>();
    public float maxDuration = 0;
    public List<GameObject> walls = new List<GameObject>();
}
