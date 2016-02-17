using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NashTools;
using GunController;
using SoundController;

public class _ImageInventoryListener : MonoBehaviour {

[Header ("Check if Not Hidden at Start")]
	public bool ShownAtStart;
	
	[Header("Check if you want to disable all children of this game object when you hide this object")]
	public bool DisableChildren;
	
	private Image _myImage;

	private int bulletCount;

//	[Header("Keys")]
	// KEYS
	public GameObject[] keyList;
	
//	[Header("Notes")]
	// NOTES
	public GameObject[] noteList;

	private SoundManager _soundManager;

	// Child Transforms
	private Transform keys;
	private Transform noteButtons;

	void Start () 
	{
		_myImage =gameObject.GetComponent<Image>();
		_soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();

		keys = gameObject.transform.Find("Keys");
		noteButtons = gameObject.transform.Find("NoteButtons");

		if(ShownAtStart)
		{
			setImageTo(true);
			_soundManager.PauseAllButAmbience();
		}
		else
		{
			setImageTo(false);
		}
		
		EventManager.AddListener((int) GameManagerScript.GameEvents.Inventory, OnInventory);
	}
	
	
	void OnInventory(Component poster, object pausedState)
	{
		// If pausedState is false and ShownAtStart is true or vice versa, pause. If both false or both true, unpause.
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
				if (child.name == "Keys")
				{
					// Check each key
					int i = 0;
					foreach (Transform key in keys)
					{
						if (keyList[i] == null && state)
							key.gameObject.SetActive(true);
						else
							key.gameObject.SetActive(false);
						i++;
					}
				}
				else if (child.name == "NoteButtons")
				{
					// Check each collected note
					int i = 0;
					foreach (Transform note in noteButtons)
					{
						if (noteList[i] == null && state)
							note.gameObject.SetActive(true);
						else
							note.gameObject.SetActive(false);
						i++;
					}
				}
				else if (child.name == "BulletNumber")
				{
					Text t = child.GetComponent<Text>();
					t.text = Gun_Controller.GetBulletCount().ToString();
					child.gameObject.SetActive(state);
				}
				else
					child.gameObject.SetActive(state);
			}  
		}
	}

	public void TurnOffNote(GameObject note)
	{
		note.SetActive(false);
		if(!_myImage.enabled)
		{
			GameManagerScript.LockSwitch();
		}

		note.SetActive(false);
	}
}
