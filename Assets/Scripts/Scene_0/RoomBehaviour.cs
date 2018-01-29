using UnityEngine;

public class RoomBehaviour : MonoBehaviour {

    private Transform room;
    private Transform rightWall;
    private Transform leftWall;
    private Transform frontWall;
    private Transform backWall;
    private ResonanceAudioRoom audioRoom;

    public Transform pipeContainer;
    public AnimationCurve curve;

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
        audioRoom = this.GetComponentInChildren<ResonanceAudioRoom>();
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
        frontWall.transform.position = new Vector3(0, 0, limit);
        leftWall.transform.position = new Vector3(-limit, 0, 0);
        rightWall.transform.position = new Vector3(limit, 0, 0);
        audioRoom.size.x = rightWall.transform.position.x * 2;
        audioRoom.size.z = frontWall.transform.position.z;
        audioRoom.transform.position = new Vector3(0, 5f, audioRoom.size.z / 2f);
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
        audioRoom.size.x = rightWall.transform.position.x * 2;
        audioRoom.size.z = frontWall.transform.position.z;
        audioRoom.transform.position = new Vector3(0, 5f, audioRoom.size.z / 2f);

        audioRoom.reverbTime = curve.Evaluate((audioRoom.size.z - 2f) / 98f); // max size of 100
    }
}
