using UnityEngine;
using System.Collections;

public class DoorCreak : MonoBehaviour {

	public float minPitch = 0.7f;
	public float maxPitch = 1.4f;
	public float minDoorSpeed = 0.4f;
	private AudioSource creakNoise;

	// Use this for initialization
	void Start () {
		creakNoise = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		float speed = GetComponent<Rigidbody>().velocity.magnitude;

		if (Mathf.Abs(speed) > minDoorSpeed && !creakNoise.isPlaying && !GameManagerScript.isPaused)
		{
			float pitchLevel = Random.Range(minPitch, maxPitch);

			creakNoise.pitch = pitchLevel;
			creakNoise.Play();
		}
	}
}
