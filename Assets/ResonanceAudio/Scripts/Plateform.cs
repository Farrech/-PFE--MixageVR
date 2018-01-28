using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateform : MonoBehaviour {

    public GameObject eye;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (this.isActiveAndEnabled)
        {
            this.transform.eulerAngles = Vector3.zero;
            //var offset = 5-this.transform.parent.transform.parent.transform.position.y;
            this.transform.position = new Vector3(eye.transform.position.x, 5, eye.transform.position.z);
        }
    }
}
