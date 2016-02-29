using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoundController;

public class NotePickUpHandler : MonoBehaviour
{	
	public GameObject note;

	private SoundManager _soundManager;
	public AudioSource collectSound;

	void Start () 
	{
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		collectSound.clip = _soundManager.GetSFXSound(0);
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

		if (note)
		{
		//	note.SetActive(true);
		//	GameManagerScript.LockSwitch();
		}
	}


}
