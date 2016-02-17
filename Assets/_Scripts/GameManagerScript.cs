using UnityEngine;
using System.Collections;
using System;
using NashTools;
using System.Collections.Generic;
using CheckpointClass;
using LoadingScreenNamespace;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	private static GameManagerScript g_Instance = null;
	
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static GameManagerScript instance 
	{
		get 
		{
			if (g_Instance == null) 
			{
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				g_Instance =  FindObjectOfType(typeof (GameManagerScript)) as GameManagerScript;
			}
			
			// If it is still null, create a new instance
			if (g_Instance == null) 
			{
				GameObject obj = new GameObject("GameManager");
				g_Instance = obj.AddComponent(typeof (GameManagerScript)) as GameManagerScript;
				Debug.Log ("Could not locate an A Manager object. GameManager was Generated Automaticly.");
			}
			
			return g_Instance;
		}
	}
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() 
	{
		g_Instance = null;
	}
	
	
	public static bool isPaused;
	public static bool isAlive;
	private int gate;
	CursorLockMode wantedMode;
	private Text _ammoHud;


	//Any event we want to track goes here!
	// The event manager itself uses ints in order to remain generic, so you will have to cast your EventName enum values to ints
	public enum GameEvents
	{
		Paused,
		Inventory,
		keyPickedUp,
		DamagePlayer,
		Dead,
		Victory,
		Checkpoint,
		LoadLastCheckpointPressed,
		LoadlastCheckpoint,
		StonePlaced,
		ResetBoss,
		UnlockDoor,
		Ladder,
		Option
	}
	
	//Make a checkpoint list.
	public List<CheckpointClass.Checkpoint.CheckpointData> Checkpoints = new List<CheckpointClass.Checkpoint.CheckpointData>();
	private CheckpointClass.Checkpoint.CheckpointData _lastCheckpoint;


	// Use this for initialization
	void Start () 
	{
		
		isPaused = false;
		isAlive = true;
		Time.timeScale = 1f;

		EventManager.AddListener((int) GameEvents.Checkpoint, OnCheckpointReached);
		EventManager.AddListener((int) GameEvents.LoadLastCheckpointPressed, LoadLastCheckpointPressed);
		EventManager.AddListener ((int)GameEvents.Dead, OnPlayerDead);

		_ammoHud = GameObject.Find("AmmoCount").GetComponent<Text>();	
	}


	void OnLevelWasLoaded()
	{

	}
	
	// Update is called once per frame
	
	void Update () 
	{

		if(Input.GetButtonDown("Pause"))
		{
			LockSwitch((int)GameEvents.Paused);
		}
		else if (Input.GetButtonDown("Inventory"))
		{
			LockSwitch((int)GameEvents.Inventory);
		}


		//If the player clicks on the screen and the game isn't paused we should probably lock the cursor and hide it.
		//If the player alt+tabs out and it's not paused, clicking back into the window the mouse should lock yeah.
		// Also if the mouse is outside of the screen or if it's in a menu, the gun shouldn't fire.
		if(Input.GetButtonDown("Fire1") && isPaused == false)
		{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

		}

		if(isPaused == true || !isAlive)
		{
			_ammoHud.enabled = false;
		}
		else
			_ammoHud.enabled = true;
	}
	
	public static void LockSwitch(int gameEvent = -1)
	{
		if(isPaused == true)
		{
			isPaused = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			// If gameEvent paramater is given, post notification
			if (gameEvent != -1)
				EventManager.PostNotification(gameEvent, instance, isPaused);

			Time.timeScale = 1f;
		}
		else if(isPaused == false)
		{
			isPaused = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			
			// If gameEvent paramater is given, post notification
			if (gameEvent != -1)
				EventManager.PostNotification(gameEvent, instance, isPaused);

			Time.timeScale = 0f;
		}
	
	}

	private void OnPlayerDead(Component poster, object pausedState)
	{
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	
	//When the player reaches a checkpoint
	private void OnCheckpointReached(Component poster, object newCheckpoint)
	{
		//Add a new checkpoint struct to the list.
		Checkpoints.Add((CheckpointClass.Checkpoint.CheckpointData)  newCheckpoint);
		//Set the last checkpoint to the new checkpoint.
		_lastCheckpoint =  Checkpoints[(Checkpoints.Count)-1];
		
	}

	
	//Send the checkpointdata out to everything that needs it.
	private void LoadLastCheckpointPressed(Component poster, object checkpointData)
	{
		EventManager.PostNotification((int) GameEvents.LoadlastCheckpoint, this, _lastCheckpoint);

		Checkpoints.Clear();
	

		if(isPaused)
			LockSwitch((int)GameEvents.Paused);
		else Time.timeScale = 1f;
	
		//If checkpoint size is 6 we hit the final boss room so we need to send out a reset to all the final boss things.
		EventManager.PostNotification((int) GameEvents.ResetBoss, this, _lastCheckpoint);
		if (Checkpoints.Count == 6)
		{

		}
	}
	public int getGate() {
		return gate;
	}
	public void setGate(int i) {
		gate = i;
	}
}
