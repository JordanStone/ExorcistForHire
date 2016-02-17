using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NashTools;
using SoundController;


//This script show's and hides what it's attached to when the game is paused.

public class _ImagePauseListener : MonoBehaviour {
	[Header ("Check if Not Hidden at Start")]
	public bool ShownAtStart;
	
	[Header("Check if you want to disable all children of this game object when you hide this object")]
	public bool DisableChildren;
	
	private Image _myImage;

	private SoundManager _soundManager;
	

	void Start () 
	{
		_myImage =gameObject.GetComponent<Image>();	
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();

		if(ShownAtStart)
		{
			setImageTo(true);
		}
		else
		{
			setImageTo(false);
		}

		EventManager.AddListener((int) GameManagerScript.GameEvents.Paused, OnPaused);	


	}



	void OnPaused(Component poster, object pausedState)
	{
		if((bool) pausedState != ShownAtStart)
		{
			setImageTo(true);
			_soundManager.PauseAllButAmbience();
		}
		else
		{
			_soundManager.ContinueAllAudio();
			setImageTo(false);
		}
	}

	private void setImageTo(bool state)
	{
		_myImage.enabled = state;
			
		if(DisableChildren)
		{
			foreach (Transform child in transform)     
			{  
				child.gameObject.SetActive(state);
			}  
		}
	}

// Button Functions
	// Restarts current scene
	public void Restart(){
		Application.LoadLevel(Application.loadedLevel);
	}

	// Quits application
	public void Quit(){
		Application.Quit();
	}
}