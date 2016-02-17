using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NashTools;


//This script show's and hides what it's attached to when the game is paused.

public class _TextPauseListener : MonoBehaviour {

	[Header ("Check if Not Hidden at Start")]
	public bool ShownAtStart;

	[Header("Check if you want to disable all children of this game object when you hide this object")]
	public bool DisableChildren;

	private Image _myImage;

	void Start () 
	{
		_myImage =gameObject.GetComponent<Image>();

		if(ShownAtStart)
		{
			_myImage.enabled = true;
		}
		else
		{
			_myImage.enabled = false;
		}

		EventManager.AddListener((int) GameManagerScript.GameEvents.Paused, OnPaused);
	}


	void OnPaused(Component poster, object pausedState)
	{
		if((bool) pausedState == true && ShownAtStart == false)
		{
			_myImage.enabled = true;
		
			if(DisableChildren)
			{
				foreach (Transform child in transform)     
				{  
					child.gameObject.SetActive(true);   
				}  
			}

		}
		else if (ShownAtStart == false)
		{
			_myImage.enabled = false;

			if(DisableChildren)
			{
				foreach (Transform child in transform)     
				{  
					child.gameObject.SetActive(false);   
				}  
			}
		}
		
		if((bool) pausedState == true && ShownAtStart == true)
		{
			_myImage.enabled = false;

			if(DisableChildren)
			{
				foreach (Transform child in transform)     
				{  
					child.gameObject.SetActive(false);   
				}  
			}
		}
		else if (ShownAtStart == true)
		{
			_myImage.enabled = true;
			if(DisableChildren)
			{
				foreach (Transform child in transform)     
				{  
					child.gameObject.SetActive(true);   
				}  
			}
		}
	}
}
