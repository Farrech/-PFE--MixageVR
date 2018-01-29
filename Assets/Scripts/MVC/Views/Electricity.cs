using UnityEngine;
using System.Collections;

public class Electricity : MonoBehaviour
{

    private LineRenderer lRend;
    private Vector3[] points = new Vector3[5];

    public Transform Beginning;
    public Transform End;
    [Range(0,1)]
    public float intensity = 0;

    float randomPosOffset = 0.075f;
    float randomWithOffsetMin = 0.25f;
    float randomWithOffsetMax = 0.5f;

    private readonly float waitingTime;

    void Start()
    {
        lRend = GetComponent<LineRenderer>();
        StartCoroutine(Beam());
    }

    private IEnumerator Beam()
    {
        yield return new WaitForSeconds(waitingTime);
        ApplyIntensity();
        points[0] = Beginning.position;
        points[4] = End.position;
        CalculateMiddle();
        lRend.SetPositions(points);
        lRend.startWidth = RandomWidthOffset();
        lRend.endWidth = RandomWidthOffset();
        StartCoroutine(Beam());
    }

    private void ApplyIntensity()
    {
        randomPosOffset = Mathf.Lerp(0, 0.075f, intensity);
        randomWithOffsetMin = Mathf.Lerp(0, 0.25f, intensity);
        randomWithOffsetMax = Mathf.Lerp(0, 0.5f, intensity);
    }

    private float RandomWidthOffset()
    {
        return Random.Range(randomWithOffsetMin, randomWithOffsetMax);
    }

    private void CalculateMiddle()
    {
        Vector3 center = GetMiddleWithRandomness(Beginning.position, End.position);

        points[2] = center;
        points[1] = GetMiddleWithRandomness(Beginning.position, center);
        points[3] = GetMiddleWithRandomness(center, End.position);
    }

    // pour créer les arcs éléctriques, il faut introduire de l'aléatoire pour chaque section de l'éclair
    private Vector3 GetMiddleWithRandomness(Vector3 point1, Vector3 point2)
    {
        float x = (point1.x + point2.x) / 2;
        float finalX = Random.Range(x - randomPosOffset, x + randomPosOffset);
        float y = (point1.y + point2.y) / 2;
        float finalY = Random.Range(y - randomPosOffset, y + randomPosOffset);
        float z = (point1.z + point2.z) / 2;
        float finalZ = Random.Range(z - randomPosOffset, z + randomPosOffset);

        return new Vector3(finalX, finalY, finalZ);
    }
}