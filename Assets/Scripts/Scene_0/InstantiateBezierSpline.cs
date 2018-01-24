using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBezierSpline : MonoBehaviour {

    public int radius;
    private BezierSpline bezierSpline;

	void Awake () {
        float length = radius * 0.552284749f;
        bezierSpline = this.GetComponent<BezierSpline>();
        bezierSpline.Loop = false;
        Vector3 coord1 = new Vector3(-radius, 0, 0);
        Vector3 coord2 = new Vector3(0,0,radius);
        Vector3 coord3 = new Vector3(radius, 0, 0);
        bezierSpline.SetControlPoint(0, coord1);
        bezierSpline.SetControlPoint(1, TangenteOfPoint(coord1, length));
        bezierSpline.SetControlPoint(3, coord2);
        bezierSpline.SetControlPoint(2, TangenteOfPoint(coord2, -length));
        bezierSpline.AddCurve();
        bezierSpline.SetControlPoint(4, TangenteOfPoint(coord2, length));
        bezierSpline.SetControlPoint(6, coord3);
        bezierSpline.SetControlPoint(5, TangenteOfPoint(coord3, length));
	}
	

    private Vector3 TangenteOfPoint(Vector3 point, float length)
    {
        float x, z;
        float pointToLookAtY = 0;
        x = z = 0;

        x = point.x;
        z = point.z;

        float c = point.x * point.x + point.z * point.z;
        if (z != 0)
        {
            pointToLookAtY = (x * length - c) / -z;
            return new Vector3(length, 0, pointToLookAtY);
        }
        else
        {
            pointToLookAtY = (-c / -x);
            return new Vector3(pointToLookAtY, 0, length);
        }
    }

}
