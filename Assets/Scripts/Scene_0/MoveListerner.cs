using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveListerner : MonoBehaviour {

    public GameObject frontWall;

    private void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction += Move;
    }

    private void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction -= Move;
    }


    void Move (GameObject hitGo, Vector3 direction) { // Déplacement du listener
        if (hitGo && hitGo.tag == "AudioListener") 
        {
            if (direction == Vector3.up && this.transform.position.z < frontWall.transform.position.z - 0.5f)
            {
                this.transform.localPosition = new Vector3(this.transform.position.x, -1, this.transform.position.z + 1f * Time.deltaTime);
            }
            else if (direction == Vector3.down && this.transform.localPosition.z >= 0)
            {
                this.transform.localPosition = new Vector3(this.transform.position.x, -1, this.transform.position.z - 1f * Time.deltaTime);
            }
            else if (direction == Vector3.right && this.transform.position.x < frontWall.transform.position.z * 2 - 0.5f)
            {
                this.transform.localPosition = new Vector3(this.transform.position.x + 1f * Time.deltaTime, -1, this.transform.position.z);
            }
            else if (direction == Vector3.left && this.transform.position.x > -frontWall.transform.position.z * 2 + 0.5f)
            {
                this.transform.localPosition = new Vector3(this.transform.position.x - 1f * Time.deltaTime, -1, this.transform.position.z);
            }
        }

    }
}
