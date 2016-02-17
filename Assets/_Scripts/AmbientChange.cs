using UnityEngine;
using System.Collections;
using SoundController;

// Used to trigger change of background ambience. Still a work in progress.
public class AmbientChange : MonoBehaviour {

	[Header("Input the Index Of The Ambient Sound to Switch In.")]
	public int ambientIndex;
	new public string name;

	private SoundManager _soundManager;
	private AudioClip ambTrack;

	// Use this for initialization
	void Start () {
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		ambTrack= _soundManager.GetAmbientSound(ambientIndex);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit(Collider other)
	{
		if (other.name == "Player")
		{
			_soundManager.SetAmbience(ambTrack);
		}
	}
}
