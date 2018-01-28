using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoView : PFEElement {

    public Transform Wall;
    public ParticleSystem particle;
    public AnimationCurve curve;

    private float minValue = -10;
    private float maxValue = -2;
	
	void Update () {
        Vector3 clampedPosition = Wall.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minValue, maxValue);
        Wall.localPosition = clampedPosition;

        float clampedValue = curve.Evaluate((clampedPosition.x - maxValue) / (minValue - maxValue));
        ParticleSystem.MainModule module = particle.main;
        module.maxParticles = (int)(clampedValue * 1000); // between 0 and 1000 particles
        float delay = clampedValue * 5000; // between 0 and 5000 ms

        app.controller.OnEchoChange(delay, 0.5f);
    }
}
