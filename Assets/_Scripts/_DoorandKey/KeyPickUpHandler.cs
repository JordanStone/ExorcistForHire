using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NashTools;
using SoundController;

public class KeyPickUpHandler : MonoBehaviour
{

	public int DoorNumber;

	private SoundManager _soundManager;
	public AudioSource collectSound;

	// Use this for initialization
	void Start () 
	{
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		collectSound.clip = _soundManager.GetSFXSound(1);
	}

	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnDestroy()
	{
		if (collectSound.clip)
		{
			AudioSource.PlayClipAtPoint(collectSound.clip, transform.position);
		}
		EventManager.PostNotification((int) GameManagerScript.GameEvents.keyPickedUp, this, DoorNumber);
	}
}
