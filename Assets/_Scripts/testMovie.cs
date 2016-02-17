using UnityEngine;
using System.Collections;
using NashTools;

public class testMovie : MonoBehaviour {

	private MovieTexture mov;

	// Use this for initialization
	void Start () {
		mov = ((MovieTexture) GetComponent<Renderer>().material.mainTexture);
		mov.loop = true;
		mov.Play();

		EventManager.AddListener((int) GameManagerScript.GameEvents.Paused, OnPaused);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPaused(Component poster, object pausedState)
	{
		if ( (bool)pausedState)
		{
			mov.Pause();
		}
		else
		{
			mov.Play();
		}
	}
}
