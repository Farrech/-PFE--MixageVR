using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistorsionView : PFEElement {

    public Transform movingCylinder;
    public Electricity electricity;
    public AnimationCurve curve;

    float minLocalZ = -1;
    float maxLocalZ = 1;
	
	void Update () {
		Vector3 clampedPosition = movingCylinder.localPosition;
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minLocalZ, maxLocalZ);
        movingCylinder.localPosition = clampedPosition;

        electricity.intensity = curve.Evaluate((clampedPosition.z - minLocalZ) / (maxLocalZ - minLocalZ)); // on se sert d'une courbe personnalisée pour relier la valeur apparente et la valeur réelle

        app.controller.OnDistorsionChange(electricity.intensity);
    }
}
