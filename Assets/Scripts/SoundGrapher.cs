using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGrapher : MonoBehaviour
{
	// public

	[Range(0.1f, 4f)]
	public float factor = 1;
	public AudioClip sound;	



	// private

	int frameLength = 2;
	float[] samples;
	ParticleSystem ps;
    TextMesh text;
    float currentTime;
	AudioSource audioSource;



	// Basic Methods

	void Start () {	
		ps = GetComponentInChildren<ParticleSystem> ();
        text = GetComponentInChildren<TextMesh>();
		audioSource = GetComponent<AudioSource> ();

        SetNewSound("Sounds/03_Saxophone");
    }

	void Update () {	
		if (sound != null && currentTime <= sound.length) {
			currentTime += Time.deltaTime;
			text.text = currentTime.ToString ("F2") + " / " + sound.length.ToString("F2");
			CreatePoints ();
		}
	}



	// Custom Methods

	void CreatePoints () {
		ParticleSystem.Particle[] points = new ParticleSystem.Particle[frameLength * sound.frequency];

		float increment = 1f / (points.Length - 1) * frameLength;
		int sampleOffset = Mathf.RoundToInt(sound.frequency * currentTime);
		int sampleIndex = 0;
		for (int i = 0; i < points.Length; i++) {
			float x = i * increment;
			sampleIndex = i - (frameLength * sound.frequency / 2) + sampleOffset;
			float y = (sampleIndex < 0 || sampleIndex > samples.Length / 2) ? 0 : samples[sampleIndex] * factor;
			points [i].position = new Vector3 (x, y, 0f);
			points [i].startColor = new Color (x, y, 0f);
			points [i].startSize = 0.01f;
		}

		ps.SetParticles (points, points.Length);
	}	

	public void SetNewSound (string soundName) {
		audioSource.Stop ();
		sound = Resources.Load<AudioClip> (soundName);

		if (sound != null) {
			samples = new float[sound.samples * sound.channels];
			sound.GetData (samples, 0);

			currentTime = 0;
			audioSource.PlayOneShot (sound);
		}
	}
}
