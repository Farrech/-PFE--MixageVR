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
        rightWall = room.Find("RightWall");
        leftWall = room.Find("LeftWall");
        frontWall = room.Find("FrontWall");
        backWall = room.Find("BackWall");
        InitialiseRoom();
    }

    public void InitialiseRoom()
    {
        float limit = 2;
        foreach (Transform t in pipeContainer)
        {
            limit = t.GetComponent<Pipe>().curveRadius + 1 > limit ? t.GetComponent<Pipe>().curveRadius + 1 : limit;
        }
        frontWall.transform.position = new Vector3(frontWall.transform.position.x, frontWall.transform.position.y, limit);
        leftWall.transform.position = new Vector3(-limit*2, frontWall.transform.position.y, frontWall.transform.position.z);
        rightWall.transform.position = new Vector3(limit * 2, frontWall.transform.position.y, frontWall.transform.position.z);
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
}
