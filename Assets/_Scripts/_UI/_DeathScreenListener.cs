using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NashTools;
using SoundController;


//This script show's and hides what it's attached to when the game is paused.

public class _DeathScreenListener : MonoBehaviour 
{
	[Header ("Check if Not Hidden at Start")]
	public bool ShownAtStart;
	
	[Header("Check if you want to disable all children of this game object when you hide this object")]
	public bool DisableChildren;
	
	private Image _myImage;

	private SoundManager _soundManager;

	private PlayerStats _playerStats;

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

		_playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
	
		EventManager.AddListener((int) GameManagerScript.GameEvents.LoadLastCheckpointPressed, OnLastCheckpointPressed);
	}

	void Update()
	{

		if(_playerStats.GetHealth() <= 0f)
		{
			OnDead(true);
		}

	}

	void OnLastCheckpointPressed(Component poster, object Nothing)
	{
		setImageTo(false);
	}

	void OnDead(bool pausedState)
	{
		if(pausedState != ShownAtStart)
		{
			setImageTo(true);
			_soundManager.StopAllAudio();

			GameManagerScript.isAlive = false;
		}
		else
		{
			_soundManager.ContinueAllAudio();
			setImageTo(false);
		}
	}

	void setImageTo(bool state)
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
}