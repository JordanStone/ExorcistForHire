using UnityEngine;
using System.Collections;
using NashTools;

public class TvSpook : MonoBehaviour {

	public float fowardRayDistance = 2.5f;
	public float MAXSECONDS = 3f;
	public float screamBeforeDeathTime = 2f;
	public Material spookMat;

	private float secondsCounter = 0f;

	private Transform myTransform;
	private Material tvMat;
	private RaycastHit hit;
	private AudioSource screamSound;
	private bool hasSpooked = false;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		tvMat = GetComponent<Renderer>().material;
		screamSound = GetComponents<AudioSource>()[1];

		EventManager.AddListener((int) GameManagerScript.GameEvents.ResetBoss, OnReset);
	}
	

	void FixedUpdate () {
		if (Physics.Raycast (myTransform.position, -myTransform.right, out hit, fowardRayDistance) && hit.transform.name == "Player"){
			// Ooh, the player. Let's spook em
			secondsCounter += Time.deltaTime;
//			print("Time: " + secondsCounter);

			if (secondsCounter >= MAXSECONDS && !hasSpooked) // You stayed in front of the TV too long so now you die
			{
				GetComponent<Renderer>().material = spookMat;
				screamSound.Play();
				StartCoroutine(WaitForSpook(screamBeforeDeathTime));
				hasSpooked = true;
			}
		}
		else if (secondsCounter != 0) // Reset counter if the player moves away
		{
			secondsCounter = 0;
		}
	}

	IEnumerator WaitForSpook(float time)
	{
		yield return new WaitForSeconds(time);
		GameManagerScript.LockSwitch((int) GameManagerScript.GameEvents.Dead);
	}

	void OnReset(Component poster, object checkpointData)
	{
		// Make sure the TV doesn't just automatically kill you when you reset
		secondsCounter = 0f;
		GetComponent<Renderer>().material = tvMat;
		hasSpooked = false;
	}
}
