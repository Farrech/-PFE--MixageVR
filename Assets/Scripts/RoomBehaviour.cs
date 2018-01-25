using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour {

    private Transform room;
    private Transform rightWall;
    private Transform leftWall;
    private Transform frontWall;
    private Transform backWall;
    public Transform pipeContainer;

    public void OnEnable()
    {
        RightControllerManager.OnTouchpadPressAction2 += MoveWall;
    }

    public void OnDisable()
    {
        RightControllerManager.OnTouchpadPressAction2 -= MoveWall;
    }
    void Start()
    {
        room = this.transform;
        rightWall = room.Find("RightWall"); // à changer
        leftWall = room.Find("LeftWall");
        frontWall = room.Find("FrontWall");
        backWall = room.Find("BackWall");
    }

    void MoveWall(bool up)
    {
        float limit = 2;
        foreach(Transform t in pipeContainer)
        {
            limit = t.GetComponent<Pipe>().curveRadius+1 > limit ? t.GetComponent<Pipe>().curveRadius+1 : limit;
        }

        if (up && frontWall.position.z<500)
        {
            rightWall.position += Vector3.right / 10;
            leftWall.position += Vector3.left / 10;
            frontWall.position += Vector3.forward / 10;
        }
        else if (frontWall.position.z>limit)
        {
            rightWall.position += Vector3.left / 10;
            leftWall.position += Vector3.right / 10;
            frontWall.position += Vector3.back / 10;
        }


    }

    void Update()
    {

        //    if (Input.GetAxis("Vertical") > 0)
        //    {
        //        rightWall.position += Vector3.right/10;
        //        leftWall.position += Vector3.left/10;
        //        frontWall.position += Vector3.forward/10;
        //    }
        //    else if (Input.GetAxis("Vertical") < 0)
        //    {
        //        rightWall.position += Vector3.left/10;
        //        leftWall.position += Vector3.right/10;
        //        frontWall.position += Vector3.back/10;
        //    }

        //    rightWall.position = new Vector3(
        //        Mathf.Clamp(rightWall.position.x, 2.5f, 500),
        //        rightWall.position.y,
        //        rightWall.position.z);

        //    leftWall.position = new Vector3(
        //        Mathf.Clamp(leftWall.position.x, -500, -2.5f),
        //        leftWall.position.y,
        //        leftWall.position.z);

        //    frontWall.position = new Vector3(
        //        frontWall.position.x,
        //        frontWall.position.y,
        //        Mathf.Clamp(frontWall.position.z, 2, 500));
        //}
    }
}
